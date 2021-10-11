using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroupDocs.Viewer.Cloud.Sdk.Api;
using GroupDocs.Viewer.Cloud.Sdk.Model;
using GroupDocs.Viewer.Cloud.Sdk.Model.Requests;
using GroupDocs.Viewer.UI.Api.Caching;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using Microsoft.Extensions.Options;
using PageInfo = GroupDocs.Viewer.UI.Core.Entities.PageInfo;

namespace GroupDocs.Viewer.UI.Cloud.Api.Viewers
{
    internal class PngViewer : IViewer
    {
        private readonly Config _config;
        private readonly IFileStorage _fileStorage;
        private readonly IFileCache _fileCache;

        public PngViewer(
            IOptions<Config> config,
            IFileStorage fileStorage,
            IFileCache fileCache)
        {
            _config = config.Value;
            _fileStorage = fileStorage;
            _fileCache = fileCache;
        }

        public Task<DocumentInfo> GetDocumentInfoAsync(string filePath, string password) =>
            _fileCache.GetValueAsync(CacheKeys.FILE_INFO_CACHE_KEY, filePath, async () =>
            {
                await UploadFileIfNotExistsAsync(filePath);

                var response = GetInfoResult(filePath, password);

                var documentInfo = ToDocumentDescription(response);
                return documentInfo;
            });

        private InfoResult GetInfoResult(string filePath, string password)
        {
            var infoApi = CreateInfoApi();
            var viewOptions = new ViewOptions
            {
                FileInfo = new Viewer.Cloud.Sdk.Model.FileInfo
                {
                    FilePath = filePath,
                    Password = password,
                    StorageName = _config.StorageName
                }
            };
            var request = new GetInfoRequest(viewOptions);
            var response = infoApi.GetInfo(request);
            return response;
        }

        public Task<byte[]> CreatePdfAsync(string filePath, string password) =>
            _fileCache.GetValueAsync(CacheKeys.PDF_FILE_CACHE_KEY, filePath, async () =>
            {
                await UploadFileIfNotExistsAsync(filePath);

                var viewApi = CreateViewApi();
                var viewOptions = new ViewOptions
                {
                    FileInfo = new Viewer.Cloud.Sdk.Model.FileInfo
                    {
                        FilePath = filePath,
                        Password = password,
                        StorageName = _config.StorageName
                    },
                    ViewFormat = ViewOptions.ViewFormatEnum.PDF
                };

                var response = viewApi.CreateView(new CreateViewRequest(viewOptions));

                byte[] bytes = DownloadFile(response.File.Path);
                return bytes;
            });

        public Task<byte[]> GetPageResourceAsync(string filePath, string password, int pageNumber,
            string resourceName) =>
            Task.FromResult(Array.Empty<byte>());

        public async Task<Pages> RenderPagesAsync(string filePath, string password, int[] pageNumbers)
        {
            await UploadFileIfNotExistsAsync(filePath);

            var pages = new List<Page>();
            foreach (var pageNumber in pageNumbers)
            {
                string cacheKey = CacheKeys.GetPngPageCacheKey(pageNumber);
                byte[] data = _fileCache.GetValue(cacheKey, filePath,
                    () =>
                    {
                        var viewApi = CreateViewApi();
                        var viewOptions = new ViewOptions
                        {
                            FileInfo = new Viewer.Cloud.Sdk.Model.FileInfo
                            {
                                FilePath = filePath,
                                Password = password,
                                StorageName = _config.StorageName
                            },
                            ViewFormat = ViewOptions.ViewFormatEnum.PNG,
                            RenderOptions = new RenderOptions
                            {
                                PagesToRender = new List<int?> { pageNumber }
                            }
                        };

                        var request = new CreateViewRequest(viewOptions);
                        var response = viewApi.CreateView(request);

                        var page = response.Pages.Single();

                        byte[] bytes = DownloadFile(page.Path);
                        return bytes;
                    });

                string base64 = "data:image/png;base64," + Convert.ToBase64String(data);

                pages.Add(new Page(pageNumber, base64));
            }

            return new Pages(pages);
        }

        private static DocumentInfo ToDocumentDescription(InfoResult infoResult)
        {
            var printAllowed = true;
            if (infoResult.PdfViewInfo != null)
                printAllowed = infoResult.PdfViewInfo.PrintingAllowed.GetValueOrDefault();

            return new DocumentInfo
            {
                PrintAllowed = printAllowed,
                Pages = infoResult.Pages.Select(page => new PageInfo
                {
                    Number = page.Number.GetValueOrDefault(),
                    Width = page.Width.GetValueOrDefault(),
                    Height = page.Height.GetValueOrDefault(),
                })
            };
        }

        private byte[] DownloadFile(string filePath)
        {
            var fileApi = CreateFileApi();
            var request = new DownloadFileRequest(filePath, _config.StorageName);

            using var stream = fileApi.DownloadFile(request);
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            return memoryStream.ToArray();
        }

        private async Task UploadFileIfNotExistsAsync(string filePath)
        {
            var storageApi = CreateStorageApi();
            var objectExistRequest = new ObjectExistsRequest(filePath, _config.StorageName);
            var objectExistResponse = storageApi.ObjectExists(objectExistRequest);

            if (!objectExistResponse.Exists.GetValueOrDefault())
            {
                var fileApi = CreateFileApi();
                var fileBytes = await _fileStorage.ReadFileAsync(filePath);
                var uploadFileRequest =
                    new UploadFileRequest(filePath, new MemoryStream(fileBytes), _config.StorageName);

                fileApi.UploadFile(uploadFileRequest);
            }
        }

        private Viewer.Cloud.Sdk.Client.Configuration CreateApiConfiguration() =>
            new(_config.ClientId, _config.ClientSecret);

        private StorageApi CreateStorageApi()
        {
            var configuration = CreateApiConfiguration();
            var api = new StorageApi(configuration);
            return api;
        }

        private FileApi CreateFileApi()
        {
            var configuration = CreateApiConfiguration();
            var apiInstance = new FileApi(configuration);
            return apiInstance;
        }

        private InfoApi CreateInfoApi()
        {
            var configuration = CreateApiConfiguration();
            var apiInstance = new InfoApi(configuration);
            return apiInstance;
        }

        private ViewApi CreateViewApi()
        {
            var configuration = CreateApiConfiguration();
            var apiInstance = new ViewApi(configuration);
            return apiInstance;
        }
    }
}