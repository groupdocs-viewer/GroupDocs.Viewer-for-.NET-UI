using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.Results;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using GroupDocs.Viewer.UI.SelfHost.Api.Licensing;
using GroupDocs.Viewer.UI.SelfHost.Api.Viewers.Extensions;
using Microsoft.Extensions.Options;
using Page = GroupDocs.Viewer.UI.Core.Entities.Page;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Viewers
{
    internal abstract class BaseViewer : IViewer
    {
        private readonly IOptions<Config> _config;
        private readonly IViewerLicenser _viewerLicenser;
        private readonly IFileStorage _fileStorage;
        private readonly IFileTypeResolver _fileTypeResolver;

        protected BaseViewer(IOptions<Config> config, 
            IViewerLicenser viewerLicenser, IFileStorage fileStorage, IFileTypeResolver fileTypeResolver)
        {
            _config = config;
            _viewerLicenser = viewerLicenser;
            _fileStorage = fileStorage;
            _fileTypeResolver = fileTypeResolver;
        }

        public abstract string PageExtension { get; }

        public abstract Page CreatePage(int pageNumber, byte[] data);

        protected abstract Page RenderPage(Viewer viewer, string filePath, int pageNumber);

        protected abstract ViewInfoOptions CreateViewInfoOptions();

        public async Task<DocumentInfo> GetDocumentInfoAsync(FileCredentials fileCredentials)
        {
            using var viewer = await InitViewerAsync(fileCredentials);
            var viewInfoOptions = CreateViewInfoOptions();
            var viewInfo = viewer.GetViewInfo(viewInfoOptions);

            var documentInfo = ToDocumentInfo(viewInfo);
            return documentInfo;
        }

        public async Task<Page> GetPageAsync(FileCredentials fileCredentials, int pageNumber)
        {
            using var viewer = await InitViewerAsync(fileCredentials);
            var page = RenderPage(viewer, fileCredentials.FilePath, pageNumber);

            return page;
        }

        public async Task<Pages> GetPagesAsync(FileCredentials fileCredentials, int[] pageNumbers)
        {
            using var viewer = await InitViewerAsync(fileCredentials);

            var pages = pageNumbers
                .Select(pageNumber => RenderPage(viewer, fileCredentials.FilePath, pageNumber))
                .ToList();

            return new Pages(pages);
        }

        public async Task<byte[]> GetPdfAsync(FileCredentials fileCredentials)
        {
            var pdfStream = new MemoryStream();
            var viewOptions = CreatePdfViewOptions(pdfStream);

            using var viewer = await InitViewerAsync(fileCredentials);
            viewer.View(viewOptions);

            return pdfStream.ToArray();
        }

        public abstract Task<byte[]> GetPageResourceAsync(FileCredentials fileCredentials, int pageNumber, string resourceName);

        private PdfViewOptions CreatePdfViewOptions(MemoryStream pdfStream)
        {
            var viewOptions = new PdfViewOptions(() => pdfStream, _ => { /* NOTE: nothing to do here */ });

            viewOptions.CopyViewOptions(_config.Value.PdfViewOptions);

            return viewOptions;
        }

        private async Task<Viewer> InitViewerAsync(FileCredentials fileCredentials)
        {
            _viewerLicenser.SetLicense();

            var fileStream = await GetFileStreamAsync(fileCredentials.FilePath);
            var loadOptions = await CreateLoadOptionsAsync(fileCredentials);
            var viewer = new Viewer(fileStream, loadOptions);
            return viewer;
        }

        private async Task<MemoryStream> GetFileStreamAsync(string filePath)
        {
            byte[] bytes = await _fileStorage.ReadFileAsync(filePath);
            MemoryStream memoryStream = new MemoryStream(bytes);
            return memoryStream;
        }

        private async Task<LoadOptions> CreateLoadOptionsAsync(FileCredentials fileCredentials)
        {
            FileType loadFileType = FileType.FromExtension(fileCredentials.FileType);
            if(loadFileType == FileType.Unknown)
                  loadFileType = await _fileTypeResolver.ResolveFileTypeAsync(fileCredentials.FilePath);
            
            LoadOptions loadOptions = new LoadOptions
            {
                FileType = FileType.FromExtension(loadFileType.Extension),
                Password = fileCredentials.Password,
                ResourceLoadingTimeout = TimeSpan.FromSeconds(3)
            };
            return loadOptions;
        }

        private static DocumentInfo ToDocumentInfo(ViewInfo viewInfo)
        {
            var printAllowed = true;
            if (viewInfo is PdfViewInfo info)
                printAllowed = info.PrintingAllowed;

            var fileType = viewInfo.FileType.Extension
                .Replace(".", string.Empty);

            return new DocumentInfo
            {
                FileType = fileType,
                PrintAllowed = printAllowed,
                Pages = viewInfo.Pages.Select(page => new PageInfo
                {
                    Number = page.Number,
                    Width = page.Width,
                    Height = page.Height,
                    Name = page.Name
                })
            };
        }
    }
}