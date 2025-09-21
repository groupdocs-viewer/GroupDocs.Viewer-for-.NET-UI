using GroupDocs.Viewer.UI.Api.Local.Storage;
using GroupDocs.Viewer.UI.Api.Models;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Caching.Implementation;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.Core.PageFormatting;
using GroupDocs.Viewer.UI.SelfHost.Api;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using GroupDocs.Viewer.UI.SelfHost.Api.InternalCaching;
using GroupDocs.Viewer.UI.SelfHost.Api.Licensing;
using GroupDocs.Viewer.UI.SelfHost.Api.Viewers;
using Microsoft.Extensions.Options;
using GroupDocs.Viewer.UI.Api.Utils;

namespace GroupDocs.Viewer.UI.Sample.StaticContentMode.Generator
{
    internal class Program
    {
        private const ViewerType VIEWER_TYPE = ViewerType.HtmlWithEmbeddedResources;
        private const string STORAGE_PATH = "./Files";
        private const string API_ENDPOINT = "/";
        private const string CONTENT_FOLDER = "Content";

        //NOTE: Thumbnails are only used when rendering to HTML
        private static bool CreateThumbnails => VIEWER_TYPE == ViewerType.HtmlWithEmbeddedResources
            || VIEWER_TYPE == ViewerType.HtmlWithExternalResources;

        static async Task Main(string[] args)
        {
            Config apiConfig = new Config();
            apiConfig.SetViewerType(VIEWER_TYPE);
            //apiConfig.SetLicensePath("GroupDocs.Viewer.lic");

            IFileStorage fileStorage = new LocalFileStorage(STORAGE_PATH);
            IApiUrlBuilder urlBuilder = new StaticUrlBuilder(API_ENDPOINT);

            List<FileSystemEntry> files = await CreateListDirAsync(fileStorage);

            foreach (var file in files)
            {
                Console.WriteLine($"Processing file: {file.FilePath}");

                IViewer viewer = CreateViewer(apiConfig, fileStorage, urlBuilder);
                
                string extension = Path.GetExtension(file.FilePath);
                string password = file.FileName.StartsWith("password") ? "12345" : string.Empty;
                FileCredentials fileCredentials = new FileCredentials(file.FilePath, extension, password);

                int[] pageNumbers = await CreateViewData(viewer, fileCredentials, urlBuilder);
               
                await CreatePagesAsync(viewer, fileCredentials, pageNumbers);

                if(CreateThumbnails)
                {
                    await CreateThumbsAsync(viewer, fileCredentials, pageNumbers);
                }

                await CreatePdfAsync(viewer, fileCredentials);

                Console.WriteLine(" Done.");
                Console.WriteLine();
            }
        }

        private static IViewer CreateViewer(Config apiConfig, IFileStorage fileStorage, IApiUrlBuilder urlBuilder)
        {
            IOptions<Config> configOptions = new OptionsWrapper<Config>(apiConfig);
            IAsyncLock asyncLock = new AsyncDuplicateLock();
            IViewerLicenseManager licenseManager = new ViewerLicenseManager(configOptions);
            IInternalCache internalCache = new NoopInternalCache();
            IFileTypeResolver fileTypeResolver = new FileExtensionFileTypeResolver();
            IPageFormatter pageFormatter = new NoopPageFormatter();

            IViewer viewer;
            switch (VIEWER_TYPE)
            {
                case ViewerType.HtmlWithEmbeddedResources:
                    viewer = new HtmlWithEmbeddedResourcesViewer(
                        configOptions,
                        asyncLock,
                        licenseManager,
                        internalCache,
                        fileStorage,
                        fileTypeResolver,
                        pageFormatter
                    );
                    break;
                case ViewerType.HtmlWithExternalResources:
                    viewer = new HtmlWithExternalResourcesViewer(
                        configOptions,
                        asyncLock,
                        licenseManager,
                        internalCache,
                        fileStorage,
                        fileTypeResolver,
                        pageFormatter,
                        urlBuilder
                    );
                    break;
                case ViewerType.Jpg:
                    viewer = new JpgViewer(
                        configOptions,
                        asyncLock,
                        licenseManager,
                        internalCache,
                        fileStorage,
                        fileTypeResolver,
                        new NoopPageFormatter()
                    );
                    break;
                case ViewerType.Png:
                    viewer = new PngViewer(
                        configOptions,
                        asyncLock,
                        licenseManager,
                        internalCache,
                        fileStorage,
                        fileTypeResolver,
                        new NoopPageFormatter()
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return viewer;
        }

        private static async Task<List<FileSystemEntry>> CreateListDirAsync(IFileStorage fileStorage)
        {
            List<FileSystemEntry> filesAndDirs = await ListFilesAndDirs(fileStorage);
            List<FileSystemEntry> files = filesAndDirs.Where(f => !f.IsDirectory).ToList();

            await SaveListDirAsync(filesAndDirs);
            return files;
        }

        private static async Task CreatePdfAsync(IViewer viewer, FileCredentials fileCredentials)
        {
            byte[] pdfBytes = await viewer.GetPdfAsync(fileCredentials);

            await SavePdfAsync(fileCredentials, pdfBytes);
        }

        private static async Task CreateThumbsAsync(IViewer viewer, FileCredentials fileCredentials, int[] pageNumbers)
        {
            Thumbs thumbs =
                await viewer.GetThumbsAsync(fileCredentials, pageNumbers);

            await SaveThumbsAsync(fileCredentials, thumbs);
        }

        private static async Task CreatePagesAsync(IViewer viewer, FileCredentials fileCredentials, int[] pageNumbers)
        {
            Pages pages =
                await viewer.GetPagesAsync(fileCredentials, pageNumbers);

            await SavePagesAsync(fileCredentials, pages);
        }

        private static async Task<int[]> CreateViewData(IViewer viewer, FileCredentials fileCredentials, IApiUrlBuilder urlBuilder)
        {
            DocumentInfo documentInfo =
                await viewer.GetDocumentInfoAsync(fileCredentials);

            var pages = new List<PageData>();
            foreach (PageInfo page in documentInfo.Pages)
            {
                var pageUrl = urlBuilder.BuildPageUrl(fileCredentials.FilePath, page.Number, viewer.PageExtension);

                var thumbUrl = CreateThumbnails
                    ? urlBuilder.BuildThumbUrl(fileCredentials.FilePath, page.Number, viewer.ThumbExtension)
                    : null;

                var pageData = new PageData(page.Number, page.Width, page.Height, pageUrl, thumbUrl);

                pages.Add(pageData);
            }

            await SaveViewDataAsync(fileCredentials, documentInfo, pages);

            Console.WriteLine($" FileType: {documentInfo.FileType}; Pages count: {documentInfo.Pages.Count()}");

            int[] pageNumbers = documentInfo.Pages.Select(p => p.Number).ToArray();
            return pageNumbers;
        }

        private static async Task<List<FileSystemEntry>> ListFilesAndDirs(IFileStorage fileStorage)
        {
            List<FileSystemEntry> filesAndDirs =
                (await fileStorage.ListDirsAndFilesAsync(".")).ToList();
            return filesAndDirs;
        }

        private static async Task SaveListDirAsync(IEnumerable<FileSystemEntry> filesAndDirs)
        {
            var fsEntries = filesAndDirs
                .Select(entity => new FileSystemItem(entity.FilePath, entity.FilePath, entity.IsDirectory, entity.Size))
                .ToArray();

            var json = Utils.SerializeToJson(fsEntries);
            var filePath = Path.Combine(CONTENT_FOLDER, Constants.LIST_DIR_FILE_NAME);

            await Utils.SaveFileAsync(filePath, json);
        }

        private static async Task SaveViewDataAsync(FileCredentials file, DocumentInfo docInfo, List<PageData> pages)
        {
            var viewDataResponse = new ViewDataResponse
            {
                File = file.FilePath,
                FileType = docInfo.FileType,
                CanPrint = true,
                SearchTerm = null,
                Pages = pages
            };

            var json = Utils.SerializeToJson(viewDataResponse);
            var filePath = Path.Combine(CONTENT_FOLDER, file.FilePath, Constants.VIEW_DATA_FILE_NAME);

            await Utils.SaveFileAsync(filePath, json);
        }

        private static async Task SavePagesAsync(FileCredentials file, Pages pages)
        {
            foreach (var page in pages)
            {
                var pageFileName = string.Format(Constants.PAGE_FILE_NAME_TEMPLATE, page.PageNumber, page.Extension);
                var pageFilePath = Path.Combine(CONTENT_FOLDER, file.FilePath, pageFileName);

                await Utils.SaveFileAsync(pageFilePath, page.PageData);

                foreach (var resource in page.Resources)
                {
                    var resourceFilePath = Path.Combine(CONTENT_FOLDER, file.FilePath, page.PageNumber.ToString(),
                        resource.ResourceName);
                    
                    await Utils.SaveFileAsync(resourceFilePath, resource.Data);
                }
            }
        }

        private static async Task SaveThumbsAsync(FileCredentials file, Thumbs thumbs)
        {
            foreach (var thumb in thumbs)
            {
                var thumbFileName = string.Format(Constants.THUMB_FILE_NAME_TEMPLATE, thumb.PageNumber, thumb.Extension);
                var thumbFilePath = Path.Combine(CONTENT_FOLDER, file.FilePath, thumbFileName);

                await Utils.SaveFileAsync(thumbFilePath, thumb.ThumbData);
            }
        }

        private static Task SavePdfAsync(FileCredentials file, byte[] bytes)
        {
            var pdfFilePath = Path.Combine(CONTENT_FOLDER, file.FilePath, Constants.PDF_FILE_NAME);

            return Utils.SaveFileAsync(pdfFilePath, bytes);
        }
    }
}