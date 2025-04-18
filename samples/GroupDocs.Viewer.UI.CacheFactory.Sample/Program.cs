﻿using GroupDocs.Viewer.UI.Api.Local.Cache;
using GroupDocs.Viewer.UI.Api.Local.Storage;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Caching;
using GroupDocs.Viewer.UI.Core.Caching.Implementation;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.Core.PageFormatting;
using GroupDocs.Viewer.UI.SelfHost.Api;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using GroupDocs.Viewer.UI.SelfHost.Api.InternalCaching;
using GroupDocs.Viewer.UI.SelfHost.Api.Licensing;
using GroupDocs.Viewer.UI.SelfHost.Api.Viewers;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.CacheFactory.Sample
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string storagePath = "./Files";
            string cachePath = "./Cache";
            Config config = new Config();

            //Trial limitations https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/
            //Temporary license can be requested at https://purchase.groupdocs.com/temporary-license
            //config.SetLicensePath("GroupDocs.Viewer.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'

            // Build up the services
            IOptions<Config> configOptions = 
                new OptionsWrapper<Config>(config);
            IAsyncLock asyncLock = new AsyncDuplicateLock();
            IViewerLicenseManager licenseManager = new ViewerLicenseManager(configOptions);
            IInternalCache internalCache = new NoopInternalCache();
            IFileStorage fileStorage = new LocalFileStorage(storagePath);
            IFileTypeResolver fileTypeResolver = new FileExtensionFileTypeResolver();
            IPageFormatter pageFormatter = new NoopPageFormatter();
            IFileCache cache = new LocalFileCache(cachePath);

            IEnumerable<FileSystemEntry> filesAndDirs = 
                await fileStorage.ListDirsAndFilesAsync(".");

            foreach (var fileSystemEntry in filesAndDirs)
            {
                Console.WriteLine($"Processing file: {fileSystemEntry.FilePath}");

                if (!fileSystemEntry.IsDirectory)
                {
                    // Instantiate Viewer here as it created per-file
                    IViewer htmlViewer = new HtmlWithEmbeddedResourcesViewer(
                        configOptions,
                        asyncLock,
                        licenseManager,
                        internalCache,
                        fileStorage,
                        fileTypeResolver,
                        pageFormatter
                    );

                    IViewer cachingViewer = new CachingViewer(htmlViewer, cache, asyncLock);

                    string extension = Path.GetExtension(fileSystemEntry.FilePath);
                    string password = fileSystemEntry.FileName.StartsWith("password") ? "12345" : string.Empty;
                    FileCredentials fileCredentials =
                        new FileCredentials(fileSystemEntry.FilePath, extension, password);
                    
                    // Create document info
                    DocumentInfo documentInfo =
                        await cachingViewer.GetDocumentInfoAsync(fileCredentials);

                    Console.WriteLine($" FileType: {documentInfo.FileType}; Pages count: {documentInfo.Pages.Count()}");

                    int[] pageNumbers = documentInfo.Pages.Select(p => p.Number).ToArray();

                    // Create pages
                    Pages pages = 
                        await cachingViewer.GetPagesAsync(fileCredentials, pageNumbers);

                    // Create thumbnails
                    Thumbs thumbs = 
                        await cachingViewer.GetThumbsAsync(fileCredentials, pageNumbers);

                    // Create PDF file
                    await cachingViewer.GetPdfAsync(fileCredentials);

                    Console.WriteLine($" Created pages: {pages.Count()}");
                    Console.WriteLine();
                }
            }
        }
    }
}