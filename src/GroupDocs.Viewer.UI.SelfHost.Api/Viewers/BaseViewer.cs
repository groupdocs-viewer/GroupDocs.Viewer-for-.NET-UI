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

        public async Task<Page> GetPageAsync(string filePath, string password, int pageNumber)
        {
            using var viewer = await InitViewerAsync(filePath, password);
            var page = RenderPage(viewer, filePath, pageNumber);

            return page;
        }

        public async Task<Pages> GetPagesAsync(string filePath, string password, int[] pageNumbers)
        {
            using var viewer = await InitViewerAsync(filePath, password);

            var pages = pageNumbers
                .Select(pageNumber => RenderPage(viewer, filePath, pageNumber))
                .ToList();

            return new Pages(pages);
        }

        public async Task<DocumentInfo> GetDocumentInfoAsync(string filePath, string password)
        {
            using var viewer = await InitViewerAsync(filePath, password);
            var viewInfoOptions = CreateViewInfoOptions();
            var viewInfo = viewer.GetViewInfo(viewInfoOptions);

            var documentInfo = ToDocumentInfo(viewInfo);
            return documentInfo;
        }

        public async Task<byte[]> GetPdfAsync(string filePath, string password)
        {
            var pdfStream = new MemoryStream();
            var viewOptions = CreatePdfViewOptions(pdfStream);

            using var viewer = await InitViewerAsync(filePath, password);
            viewer.View(viewOptions);

            return pdfStream.ToArray();
        }

        public abstract Task<byte[]> GetPageResourceAsync(string filePath, string password, int pageNumber,
            string resourceName);

        private PdfViewOptions CreatePdfViewOptions(MemoryStream pdfStream)
        {
            var viewOptions = new PdfViewOptions(() => pdfStream, _ => { /* NOTE: nothing to do here */ });

            viewOptions.CopyViewOptions(_config.Value.PdfViewOptions);

            return viewOptions;
        }

        private async Task<Viewer> InitViewerAsync(string filePath, string password)
        {
            _viewerLicenser.SetLicense();

            var fileStream = await GetFileStreamAsync(filePath);
            var loadOptions = await CreateLoadOptionsAsync(filePath, password);
            var viewer = new Viewer(fileStream, loadOptions);
            return viewer;
        }

        private async Task<MemoryStream> GetFileStreamAsync(string filePath)
        {
            byte[] bytes = await _fileStorage.ReadFileAsync(filePath);
            MemoryStream memoryStream = new MemoryStream(bytes);
            return memoryStream;
        }

        private async Task<LoadOptions> CreateLoadOptionsAsync(string filePath, string password)
        {
            FileType fileType = await _fileTypeResolver.ResolveFileTypeAsync(filePath);
            LoadOptions loadOptions = new LoadOptions
            {
                FileType = FileType.FromExtension(fileType.Extension),
                Password = password,
                ResourceLoadingTimeout = TimeSpan.FromSeconds(3)
            };
            return loadOptions;
        }

        private static DocumentInfo ToDocumentInfo(ViewInfo viewInfo)
        {
            var printAllowed = true;
            if (viewInfo is PdfViewInfo info)
                printAllowed = info.PrintingAllowed;

            return new DocumentInfo
            {
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