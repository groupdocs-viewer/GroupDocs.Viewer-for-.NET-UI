using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GroupDocs.Viewer.Interfaces;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.Results;
using GroupDocs.Viewer.UI.Api.Caching;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using GroupDocs.Viewer.UI.SelfHost.Api.Licensing;
using GroupDocs.Viewer.UI.SelfHost.Api.Viewers.Extensions;
using Microsoft.Extensions.Options;
using Page = GroupDocs.Viewer.UI.Core.Entities.Page;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Viewers
{
    internal class HtmlWithExternalResourcesViewer : IViewer
    {
        private readonly Config _config;
        private readonly IFileStorage _fileStorage;
        private readonly IFileCache _fileCache;
        private readonly UI.Api.Configuration.Options _options;

        public HtmlWithExternalResourcesViewer(
            IOptions<Config> config,
            IOptions<UI.Api.Configuration.Options> options,
            IViewerLicenser licenser,
            IFileStorage fileStorage,
            IFileCache fileCache)
        {
            _config = config.Value;
            _options = options.Value;
            _fileStorage = fileStorage;
            _fileCache = fileCache;

            licenser.SetLicense();
        }

        public Task<DocumentInfo> GetDocumentInfoAsync(string filePath, string password) =>
            _fileCache.GetValueAsync(CacheKeys.FILE_INFO_CACHE_KEY, filePath, async () =>
            {
                using var viewer = await InitViewerAsync(filePath, password);
                var viewInfoOptions = CreateViewInfoOptions();
                var viewInfo = viewer.GetViewInfo(viewInfoOptions);

                var documentInfo = ToDocumentDescription(viewInfo);
                return documentInfo;
            });

        public Task<byte[]> CreatePdfAsync(string filePath, string password) =>
            _fileCache.GetValueAsync(CacheKeys.PDF_FILE_CACHE_KEY, filePath, async () =>
            {
                var pdfStream = new MemoryStream();
                var viewOptions = CreatePdfViewOptions(pdfStream);

                using var viewer = await InitViewerAsync(filePath, password);
                viewer.View(viewOptions);

                return pdfStream.ToArray();
            });

        //NOTE: Return empty result when there is no cache. Cache required in this case 
        public Task<byte[]> GetPageResourceAsync(string filePath, string password, int pageNumber, string resourceName)
        {
            string cacheKey = CacheKeys.GetHtmlPageResourceCacheKey(pageNumber, resourceName);
            return _fileCache.GetValueAsync(cacheKey, filePath, () => Task.FromResult(new byte[0]));
        }

        public async Task<Pages> RenderPagesAsync(string filePath, string password, int[] pageNumbers)
        {
            Viewer viewer = null;
            List<Page> pages = new List<Page>();

            foreach (var pageNumber in pageNumbers)
            {
                string cacheKey = CacheKeys.GetHtmlPageCacheKey(pageNumber);
                byte[] data = await _fileCache.GetValueAsync<byte[]>(cacheKey, filePath, async() =>
                    {
                        viewer ??= await InitViewerAsync(filePath, password);

                        var page = RenderPage(viewer, filePath, pageNumber);

                        foreach (var resource in page.Resources)
                           await _fileCache.SetAsync($"p{pageNumber}_{resource.Key}", filePath, resource.Value);

                        return page.GetPageData();
                    });

                string html = Encoding.UTF8.GetString(data);

                pages.Add(new Core.Entities.Page(pageNumber, html));
            }

            viewer?.Dispose();

            return new Pages(pages);
        }

        private async Task<Viewer> InitViewerAsync(string filePath, string password)
        {
            var fileStream = await GetFileStreamAsync(filePath);
            var loadOptions = CreateLoadOptions(filePath, password);
            var viewer = new Viewer(fileStream, loadOptions);

            return viewer;
        }

        private PageContents RenderPage(Viewer viewer, string filePath, int pageNumber)
        {
            var basePath = _options.ApiPath;
            var actionName = UI.Api.Keys.LOAD_DOCUMENT_PAGE_RESOURCE_ACTION_NAME;

            var streamFactory = new MemoryPageStreamFactory(basePath, actionName, filePath);
            var viewOptions = HtmlViewOptions.ForExternalResources(streamFactory, streamFactory);
            viewOptions.CopyViewOptions(_config.HtmlViewOptions);
            viewer.View(viewOptions, pageNumber);

            var pageData = streamFactory.GetPageContents();
            return pageData;
        }

        private async Task<MemoryStream> GetFileStreamAsync(string filePath)
        {
            byte[] bytes = await _fileStorage.ReadFileAsync(filePath);
            MemoryStream memoryStream = new MemoryStream(bytes);
            return memoryStream;
        }

        private ViewInfoOptions CreateViewInfoOptions() =>
            ViewInfoOptions.FromHtmlViewOptions(_config.HtmlViewOptions);

        private PdfViewOptions CreatePdfViewOptions(MemoryStream pdfStream)
        {
            var viewOptions = new PdfViewOptions(() => pdfStream, _ => { /* NOTE: nothing to do here */ });

            viewOptions.CopyViewOptions(_config.PdfViewOptions);

            return viewOptions;
        }

        private static LoadOptions CreateLoadOptions(string filePath, string password)
        {
            string extension = Path.GetExtension(filePath);
            LoadOptions loadOptions = new LoadOptions
            {
                FileType = FileType.FromExtension(extension),
                Password = password,
                ResourceLoadingTimeout = TimeSpan.FromSeconds(3)
            };
            return loadOptions;
        }

        private static DocumentInfo ToDocumentDescription(ViewInfo viewInfo)
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

        private class MemoryPageStreamFactory : IPageStreamFactory, IResourceStreamFactory
        {
            private readonly string _basePath;
            private readonly string _actionName;
            private readonly string _filePath;
            private readonly PageContents _pageContents;

            public MemoryPageStreamFactory(string basePath, string actionName, string filePath)
            {
                _basePath = basePath;
                _actionName = actionName;
                _filePath = WebUtility.UrlEncode(filePath);
                _pageContents = new PageContents();
            }

            public PageContents GetPageContents() =>
                _pageContents;

            public Stream CreatePageStream(int pageNumber) =>
                _pageContents.GetPageStream();

            public void ReleasePageStream(int pageNumber, Stream pageStream) { }

            public Stream CreateResourceStream(int pageNumber, Resource resource) =>
                _pageContents.GetResourceStream(resource.FileName);

            public string CreateResourceUrl(int pageNumber, Resource resource) =>
                $"{_basePath}/{_actionName}?guid={_filePath}&pageNumber={pageNumber}&resourceName={resource.FileName}";

            public void ReleaseResourceStream(int pageNumber, Resource resource, Stream resourceStream) { }
        }

        private class PageContents
        {
            private MemoryStream PageStream { get; } = new MemoryStream();

            public Dictionary<string, MemoryStream> Resources { get; } = new Dictionary<string, MemoryStream>();

            public byte[] GetPageData() => PageStream.ToArray();

            public byte[] GetResourceData(string resourceName) => Resources[resourceName].ToArray();

            public Stream GetPageStream() => PageStream;

            public Stream GetResourceStream(string fileName)
            {
                var resourceStream = new MemoryStream();
                Resources.Add(fileName, resourceStream);
                return resourceStream;
            }
        }
    }
}