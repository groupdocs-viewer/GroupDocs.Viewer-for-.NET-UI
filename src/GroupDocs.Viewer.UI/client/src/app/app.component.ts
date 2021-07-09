import { Inject, Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { ViewerAppComponent, ViewerService, ViewerConfigService } from '@groupdocs.examples.angular/viewer';
import { Api, ConfigService, ModalService, UploadFilesService, NavigateService, ZoomService, PagePreloadService, RenderPrintService, PasswordService, WindowService, LoadingMaskService, PageModel, FileCredentials } from '@groupdocs.examples.angular/common-components';

type IFrame = HTMLElement & {contentWindow: Window}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.less', './variables.less']
})
export class AppComponent extends ViewerAppComponent {

    viewerService: ViewerService;
    renderPrintService: RenderPrintService;
    pagesLoading: number[];

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
        private http: HttpClient,
        @Inject(ConfigService) private config: ConfigService,
        @Inject("WINDOW") private window: Window) {

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
            loadingMaskService);

        this.renderPrintService = renderPrintService;
        this.viewerService = viewerService;
        this.pagesLoading = [];
    }

    doPrint (iframe: IFrame) {
        try {
            iframe.focus()
            iframe.contentWindow.document.execCommand('print', false)
        } catch (e) {
            iframe.contentWindow.print()
        } finally {
            // Make invisible
            iframe.style.visibility = 'hidden'
            iframe.style.left = '-1px'
        }
    }

    printFile() {
        if (this.formatDisabled)
            return;
       
        this.viewerService.loadPrintPdf(this.credentials).subscribe((blob: Blob) => {
            const iframeId = 'print-window';
            const pdf = window.URL.createObjectURL(blob)

            // Remove previous iframe if exists
            let iframe = document.getElementById(iframeId)
            if (iframe) {
              iframe.remove()
            }
        
            // Add new iframe
            iframe = document.createElement('iframe')
            iframe.setAttribute('style', 'visibility: hidden; height: 0; width: 0; position: absolute; border: 0');
            iframe.setAttribute('id', iframeId)
            iframe.setAttribute('src', pdf)

            // Append to the document
            document.getElementsByTagName('body')[0].appendChild(iframe);

            // Wait and print
            const iframeElement = document.getElementById(iframeId) as IFrame;
            setTimeout(() => this.doPrint(iframeElement), 1000);
        });
    }

    preloadPages(start: number, end: number) {
        const pagesToLoad = [];
        const isInitialLoad = start === 1;
        const minPagesToLoad = 10;
        this.selectedPageNumber = 1;

        if(isInitialLoad) {
             this.pagesLoading = [];
        }

        for (let i = start; i <= end; i++) {
            if (this.pagesLoading.indexOf(i) === -1) {
                this.pagesLoading.push(i);
                pagesToLoad.push(i);
            }
        }

        if (pagesToLoad.length > 0) {
            const last = pagesToLoad[pagesToLoad.length - 1];
            if (!isInitialLoad && pagesToLoad.length < minPagesToLoad) {
                const addPages = minPagesToLoad - pagesToLoad.length;
                for (let i = last; i <= last + addPages; i++) {
                    const pageNumber = i + 1;
                    if (this.pagesLoading.indexOf(pageNumber) === -1) {
                        pagesToLoad.push(pageNumber);
                        this.pagesLoading.push(pageNumber);
                    }
                }
            }

            this.loadPages(this.credentials, pagesToLoad).subscribe((
                (pages: any) => {
                    if(this.file.thumbnails === undefined) {
                        this.file.thumbnails = new Array<PageModel>(this.countPages);
                        for (let thumbIndex = 0; thumbIndex < this.countPages; thumbIndex++) {
                            const pageNumber = thumbIndex + 1;

                            const thumbnail = new PageModel();
                            thumbnail.number = pageNumber;
                            thumbnail.data = `<div style="height: 100%;display: grid;color:#bfbfbf"><div style="font-size: 10vw;margin:auto;text-align:center;">Click here to load page ${pageNumber}</div></div>`
                            thumbnail.width = 800;
                            thumbnail.height = 800;

                            this.file.thumbnails[thumbIndex] = thumbnail;
                        }
                    }

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
            ));
        }
    }

    loadPages(credentials: FileCredentials, pages: number[]) {
        return this.http.post(this.config.getViewerApiEndpoint() + Api.LOAD_DOCUMENT_PAGE + "s", {
            'guid': credentials.guid,
            'password': credentials.password,
            'pages': pages
        }, Api.httpOptionsJson);
    }
}
