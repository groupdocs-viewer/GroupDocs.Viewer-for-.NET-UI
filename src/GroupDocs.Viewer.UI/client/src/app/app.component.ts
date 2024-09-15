import { Component, ChangeDetectorRef } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { ViewerAppComponent, ViewerService, ViewerConfigService } from '@groupdocs.examples.angular/viewer';
import { Api, ConfigService, ModalService, UploadFilesService, NavigateService, ZoomService, PagePreloadService, RenderPrintService, PasswordService, WindowService, LoadingMaskService, PageModel, TypedFileCredentials, CommonModals } from '@groupdocs.examples.angular/common-components';

import { TranslateService } from '@ngx-translate/core';

type IFrame = HTMLElement & { contentWindow: Window }

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.less', './variables.less']
})
export class AppComponent extends ViewerAppComponent {

    configService: ConfigService;
    viewerService: ViewerService;
    pagesLoading: number[];
    http: HttpClient;
    printTitle: string;
    printMessage: string;

    constructor(viewerService: ViewerService,
        modalService: ModalService,
        viewerConfigService: ViewerConfigService,
        uploadFilesService: UploadFilesService,
        navigateService: NavigateService,
        zoomService: ZoomService,
        pagePreloadService: PagePreloadService,
        renderPrintService: RenderPrintService,
        passwordService: PasswordService,
        windowService: WindowService,
        loadingMaskService: LoadingMaskService,
        http: HttpClient,
        configService: ConfigService,
        cdr: ChangeDetectorRef,
        translate: TranslateService) {

        super(viewerService,
            modalService,
            viewerConfigService,
            uploadFilesService,
            navigateService,
            zoomService,
            pagePreloadService,
            renderPrintService,
            passwordService,
            windowService,
            loadingMaskService,
            cdr,
            translate);

        this.configService = configService;
        this.viewerService = viewerService;
        this.pagesLoading = [];
        this.http = http;
        this.printTitle = 'Preparing to print';
        this.printMessage = 'Loading pages.';
    }

    preloadPages(start: number, end: number) {
        const pagesToLoad = [];
        const isInitialLoad = start === 1;
        const minPagesToLoad = this.viewerConfig.preloadPageCount;
        const countPages = this.file.pages.length;
        this.selectedPageNumber = 1;

        if (isInitialLoad) {
            this.pagesLoading = [];
        }

        for (let i = start; i <= end; i++) {
            const page = this.file.pages.find(p => p.number === i);
            if (page && page.data) {
                continue;
            }

            if (this.pagesLoading.indexOf(i) === -1) {
                this.pagesLoading.push(i);
                pagesToLoad.push(i);
            }
        }

        if (pagesToLoad.length > 0) {
            const last = pagesToLoad[pagesToLoad.length - 1];
            if (!isInitialLoad && pagesToLoad.length < minPagesToLoad) {
                const addPages = minPagesToLoad - pagesToLoad.length;
                for (let i = last; i < last + addPages; i++) {
                    const pageNumber = i + 1;

                    if (pageNumber <= countPages && this.pagesLoading.indexOf(pageNumber) === -1) {
                        pagesToLoad.push(pageNumber);
                        this.pagesLoading.push(pageNumber);
                    }
                }
            }

            this.loadPages(this.credentials, pagesToLoad).subscribe((
                (pages: any) => {
                    this.setPagesData(pages);
                }
            ));
        }
    }

    loadAllPages(onAllPagesLoaded: Function) {
        let firstNotLoadedPage = this.file.pages.find(p => p.data === undefined);
        if(!firstNotLoadedPage) {
            onAllPagesLoaded();
            return; 
        }
        
        let start = firstNotLoadedPage.number
        let end = this.file.pages.length;
        
        const pagesToLoad = [];

        for (let i = start; i <= end; i++) {
            const page = this.file.pages.find(p => p.number === i);
            if (page && page.data) {
                continue;
            }

            if (this.pagesLoading.indexOf(i) === -1) {
                this.pagesLoading.push(i);
                pagesToLoad.push(i);
            }
        }
        
        this.openPrintModal();

        if (pagesToLoad.length > 0) {
            for (let index = 0; index < pagesToLoad.length; index++) {
                const pageNumber = pagesToLoad[index];
                
                this.loadPage(this.credentials, pageNumber).subscribe((
                    (page: any) => {
                        this.setPagesData([page]);
                        const loadedPages = this.file.pages.filter(p => p.data !== undefined).length;
                        const countPages = this.file.pages.length;
                        this.setPrintMessage(`Loaded ${loadedPages} of ${countPages} pages.`);

                        if(countPages == loadedPages) {
                            this.closePrintModal();
                            onAllPagesLoaded();
                            return; 
                        }
                    }
                ));
            }
        }
    }

    setPrintMessage(message: string) {
        this.printMessage = message;
    }

    openPrintModal() {
        this.printMessage = `Loading pages.`;
        this.openModal("preparing-to-print");
    }

    closePrintModal() {
        this.printMessage = `Loading pages.`;
        this.closeModal("preparing-to-print");
    }

    setPagesData(pages: any) {
        pages.forEach((page: PageModel) => {
            const pageIndex = page.number - 1;
            const currPage = this.file.pages[pageIndex];

            if (currPage) {
                currPage.data = page.data;
                if (this.file.thumbnails[pageIndex]) {
                    this.file.thumbnails[pageIndex].data = page.data;
                    this.file.thumbnails[pageIndex].width = currPage.width;
                    this.file.thumbnails[pageIndex].height = currPage.height;
                }
            }
        });
    }

    loadPage(credentials: TypedFileCredentials, pageNumber: number) {
        return this.http.post(this.configService.getViewerApiEndpoint() + Api.LOAD_DOCUMENT_PAGE, {
            'guid': credentials.guid,
            'fileType': credentials.fileType,
            'password': credentials.password,
            'page': pageNumber
        }, Api.httpOptionsJson);
    }

    loadPages(credentials: TypedFileCredentials, pages: number[]) {
        return this.http.post(this.configService.getViewerApiEndpoint() + Api.LOAD_DOCUMENT_PAGE + "s", {
            'guid': credentials.guid,
            'fileType': credentials.fileType,
            'password': credentials.password,
            'pages': pages
        }, Api.httpOptionsJson);
    }

    printFile() {
        if (this.formatDisabled)
            return;

        if(this.viewerConfig.htmlMode) {
            super.printFile();
        } else {
            this.printPages();
        }
    }

    private printPages() {
        this.loadAllPages(() => {
            const fileName = this.getFileName();
            const pages = this.file.pages;
            this.printInFrame(fileName, pages);
        });
    }

    private printInFrame(fileName: string, pages: PageModel[]) {
        const iframeId = 'print-window';

        // Remove previous iframe if exists
        let iframe = document.getElementById(iframeId)
        if (iframe) {
            iframe.remove()
        }

        // Create new iframe
        iframe = document.createElement('iframe')
        iframe.setAttribute('style', 'visibility: hidden; height: 0; width: 0; position: absolute; border: 0');
        iframe.setAttribute('id', iframeId)

        const firstPage = pages[0];
        const firstPagePortrait = firstPage.width < firstPage.height;
        const pageSize = firstPagePortrait ? 'portrait' : 'landscape';

        let images = "";
        for (let index = 0; index < pages.length; index++) {
            const page = pages[index];
            images += `<img src="${page.data}" alt="Page ${page.number}">`;
        }

        let srcdoc = `
            <html>
              <head>
                <title>Print Preview - ${fileName}</title>
                <style>
                  @media print {
                      @page {
                        size: ${pageSize};
                      }

                      * { 
                          margin: 0 !important; padding: 0 !important; 
                      }
                      
                      html, body {
                          height:100% !important;
                      }
                      
                      body {
                          overflow: visible !important;
                      }
                      
                      body * {
                          display: none !important;
                      }
                      
                      body img {
                          display:block !important;
                          max-width: 100%;
                          max-height: 100%;
                          page-break-after: always;
                      }
                  }
                </style>
              </head>
              <body>
                ${images}
              </body>
            </html>
        `;

        iframe.setAttribute('srcdoc', srcdoc);

        // Append to the document
        document.getElementsByTagName('body')[0].appendChild(iframe);

        // Wait and print
        const iframeElement = document.getElementById(iframeId) as IFrame;
        setTimeout(() => this.printFrame(iframeElement), 1000);
    }

    printFrame(iframe: IFrame) {
        try {
            iframe.focus()
            iframe.contentWindow.document.execCommand('print', false)
        } catch (e) {
            iframe.contentWindow.print()
        } finally {
            // Hide iframe
            iframe.style.visibility = 'hidden'
            iframe.style.left = '-1px'
        }
    }
}
