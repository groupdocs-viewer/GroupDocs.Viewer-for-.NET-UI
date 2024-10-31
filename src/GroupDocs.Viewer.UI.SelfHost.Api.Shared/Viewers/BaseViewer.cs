using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.Results;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using GroupDocs.Viewer.UI.SelfHost.Api.InternalCaching;
using GroupDocs.Viewer.UI.SelfHost.Api.Licensing;
using GroupDocs.Viewer.UI.SelfHost.Api.Viewers.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Page = GroupDocs.Viewer.UI.Core.Entities.Page;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Viewers
{
    public abstract class BaseViewer : IViewer, IDisposable
    {
        private readonly Config _config;
        private readonly IAsyncLock _asyncLock;
        private readonly IViewerLicenser _viewerLicenser;
        private readonly IInternalCache _viewerCache;
        private readonly InternalCacheOptions _internalCacheOptions;
        private readonly IFileStorage _fileStorage;
        private readonly IFileTypeResolver _fileTypeResolver;
        private readonly IPageFormatter _pageFormatter;
        private Viewer _viewer;

        protected BaseViewer(
            IOptions<Config> config,
            IAsyncLock asyncLock,
            IViewerLicenser viewerLicenser,
            IInternalCache viewerCache,
            IFileStorage fileStorage,
            IFileTypeResolver fileTypeResolver,
            IPageFormatter pageFormatter)
        {
            _config = config.Value;
            _asyncLock = asyncLock;
            _viewerLicenser = viewerLicenser;
            _viewerCache = viewerCache;
            _internalCacheOptions = config.Value.InternalCacheOptions;
            _fileStorage = fileStorage;
            _fileTypeResolver = fileTypeResolver;
            _pageFormatter = pageFormatter;
        }

        public abstract string PageExtension { get; }

        public abstract Page CreatePage(int pageNumber, byte[] data);

        protected abstract Page RenderPage(Viewer viewer, string filePath, int pageNumber);

        protected abstract ViewInfoOptions CreateViewInfoOptions();

        public async Task<DocumentInfo> GetDocumentInfoAsync(FileCredentials fileCredentials)
        {
            var viewer = await InitViewerAsync(fileCredentials);
            var viewInfoOptions = CreateViewInfoOptions();
            var viewInfo = viewer.GetViewInfo(viewInfoOptions);

            var documentInfo = ToDocumentInfo(viewInfo);
            return documentInfo;
        }

        public async Task<Page> GetPageAsync(FileCredentials fileCredentials, int pageNumber)
        {
            var viewer = await InitViewerAsync(fileCredentials);
            var page = await RenderPageInternalAsync(viewer, fileCredentials, pageNumber);
            return page;
        }

        public async Task<Pages> GetPagesAsync(FileCredentials fileCredentials, int[] pageNumbers)
        {
            var viewer = await InitViewerAsync(fileCredentials);

            var pages = new Pages();
            foreach (var pageNumber in pageNumbers)
            {
                var page = await RenderPageInternalAsync(viewer, fileCredentials, pageNumber);
                pages.Add(page);
            }

            return pages;
        }

        public async Task<byte[]> GetPdfAsync(FileCredentials fileCredentials)
        {
            var pdfStream = new MemoryStream();
            var viewOptions = CreatePdfViewOptions(pdfStream);

            var viewer = await InitViewerAsync(fileCredentials);
            viewer.View(viewOptions);

            return pdfStream.ToArray();
        }

        public abstract Task<byte[]> GetPageResourceAsync(FileCredentials fileCredentials, int pageNumber, string resourceName);

        private PdfViewOptions CreatePdfViewOptions(MemoryStream pdfStream)
        {
            var viewOptions = new PdfViewOptions(() => pdfStream, _ => { /* NOTE: nothing to do here */ });

            viewOptions.CopyViewOptions(_config.PdfViewOptions);

            return viewOptions;
        }

        private async Task<Viewer> InitViewerAsync(FileCredentials fileCredentials)
        {
            if (_viewer != null)
                return _viewer;

            _viewerLicenser.SetLicense();

            if (_internalCacheOptions.IsCacheDisabled)
            {
                _viewer = await CreateViewer(fileCredentials);
                return _viewer;
            }

            var key = $"VI__{fileCredentials.FilePath}";
            using (await _asyncLock.LockAsync(key))
            {
                if (_viewerCache.TryGet(fileCredentials, out var viewer))
                {
                    _viewer = viewer;
                }
                else
                {
                    _viewer = await CreateViewer(fileCredentials);
                    _viewerCache.Set(fileCredentials, _viewer);
                }
            }

            return _viewer;
        }

        private async Task<Viewer> CreateViewer(FileCredentials fileCredentials)
        {
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
            if (loadFileType == FileType.Unknown)
                loadFileType = await _fileTypeResolver.ResolveFileTypeAsync(fileCredentials.FilePath);

            LoadOptions loadOptions = new LoadOptions
            {
                FileType = FileType.FromExtension(loadFileType.Extension),
                Password = fileCredentials.Password,
                ResourceLoadingTimeout = TimeSpan.FromSeconds(3)
            };
            return loadOptions;
        }

        private async Task<Page> RenderPageInternalAsync(
            Viewer viewer, FileCredentials fileCredentials, int pageNumber)
        {
            var page = RenderPage(viewer, fileCredentials.FilePath, pageNumber);
            page = await _pageFormatter.FormatAsync(fileCredentials, page);

            return page;
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
                PrintingAllowed = printAllowed,
                Pages = viewInfo.Pages.Select(page => new PageInfo
                {
                    PageNumber = page.Number,
                    Width = page.Width,
                    Height = page.Height,
                    PageName = page.Name
                })
            };
        }

        public void Dispose()
        {
            // NOTE: dispose when we're not going to reuse the object
            if (_internalCacheOptions.IsCacheDisabled)
            {
                _viewer?.Dispose();
                _viewer = null;
            }
        }
    }
}