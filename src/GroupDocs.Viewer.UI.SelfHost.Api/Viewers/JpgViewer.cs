using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.Results;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using GroupDocs.Viewer.UI.SelfHost.Api.Licensing;
using GroupDocs.Viewer.UI.SelfHost.Api.Viewers.Caching;
using GroupDocs.Viewer.UI.SelfHost.Api.Viewers.Extensions;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Viewers
{
    internal class JpgViewer : IViewer
    {
        private readonly Config _config;
        private readonly IFileStorage _fileStorage;
        private readonly IFileCache _fileCache;

        public JpgViewer(
            IOptions<Config> config,
            IViewerLicenser licenser,
            IFileStorage fileStorage,
            IFileCache fileCache)
        {
            _config = config.Value;
            _fileStorage = fileStorage;
            _fileCache = fileCache;

            licenser.SetLicense();
        }

        public Task<DocumentInfo> GetDocumentInfoAsync(string filePath, string password) =>
            _fileCache.GetValueAsync(CacheKeys.FILE_INFO_CACHE_KEY, filePath, async () =>
            {
                var viewer = await InitViewerAsync(filePath, password);
                var viewInfoOptions = CreateViewInfoOptions();
                var viewInfo = viewer.GetViewInfo(viewInfoOptions);

                var documentInfo = ToDocumentDescription(viewInfo);
                return documentInfo;
            });

        private async Task<Viewer> InitViewerAsync(string filePath, string password)
        {
            var fileStream = await GetFileStreamAsync(filePath);
            var loadOptions = CreateLoadOptions(filePath, password);
            var viewer = new Viewer(fileStream, loadOptions);
            return viewer;
        }

        public Task<byte[]> CreatePdfAsync(string filePath, string password) =>
            _fileCache.GetValueAsync(CacheKeys.PDF_FILE_CACHE_KEY, filePath, async () =>
            {
                var pdfStream = new MemoryStream();
                var viewOptions = CreatePdfViewOptions(pdfStream);

                var viewer = await InitViewerAsync(filePath, password);
                viewer.View(viewOptions);

                return pdfStream.ToArray();
            });

        public Task<byte[]> GetPageResourceAsync(string filePath, string password, int pageNumber, string resourceName) =>
            Task.FromResult(new byte[0]);

        public async Task<Pages> RenderPagesAsync(string filePath, string password, int[] pageNumbers)
        {
            Viewer viewer = null;
            var pages = new List<Core.Entities.Page>();

            foreach (var pageNumber in pageNumbers)
            {
                string cacheKey = CacheKeys.GetJpgPageCacheKey(pageNumber);
                byte[] data = await _fileCache.GetValueAsync(cacheKey, filePath,
                    async () =>
                    {
                        viewer ??= await InitViewerAsync(filePath, password);
                        return RenderPage(viewer, pageNumber);
                    });

                string base64 = "data:image/jpeg;base64," + Convert.ToBase64String(data);

                pages.Add(new Core.Entities.Page(pageNumber, base64));
            }

            viewer?.Dispose();

            return new Pages(pages);
        }

        private byte[] RenderPage(Viewer viewer, int pageNumber)
        {
            var pageStream = new MemoryStream();
            
            var viewOptions = CreateJpgViewOptions(pageStream);
            
            viewer.View(viewOptions, pageNumber);

            return pageStream.ToArray();
        }

        private async Task<MemoryStream> GetFileStreamAsync(string filePath)
        {
            byte[] bytes = await _fileStorage.ReadFileAsync(filePath);
            MemoryStream memoryStream = new MemoryStream(bytes);
            return memoryStream;
        }

        private ViewInfoOptions CreateViewInfoOptions() =>
            ViewInfoOptions.FromJpgViewOptions(_config.JpgViewOptions);

        private JpgViewOptions CreateJpgViewOptions(MemoryStream pageStream)
        {
            var viewOptions = new JpgViewOptions(_ => pageStream,
                (_, __) => { /*NOTE: Do nothing here*/ });

            viewOptions.CopyViewOptions(_config.JpgViewOptions);

            return viewOptions;
        }

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
    }
}