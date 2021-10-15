using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.Cloud.Sdk.Model;
using GroupDocs.Viewer.UI.Api.Caching;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.Cloud.Api.Viewers
{
    internal class PngViewer : IViewer
    {
        private readonly Config _config;
        private readonly IFileCache _fileCache;
        private readonly IFileStorage _fileStorage;
        private readonly IViewerApiConnect _viewerApiConnect;
        private readonly AsyncDuplicateLock _asyncDuplicateLock = new AsyncDuplicateLock();

        public PngViewer(
            IOptions<Config> config,
            IFileCache fileCache,
            IFileStorage fileStorage,
            IViewerApiConnect viewerApiConnect)
        {
            _config = config.Value;
            _fileCache = fileCache;
            _fileStorage = fileStorage;
            _viewerApiConnect = viewerApiConnect;
        }

        public async Task<Pages> GetPagesAsync(string filePath, string password, int[] pageNumbers)
        {
            using (await _asyncDuplicateLock.LockAsync(filePath))
            {
                var pages = new List<Page>();
                var pagesToCreate = new List<int>();

                foreach (var pageNumber in pageNumbers)
                {
                    string cacheKey = CacheKeys.GetPngPageCacheKey(pageNumber);
                    var cachedPageData = await _fileCache.TryGetValueAsync<byte[]>(cacheKey, filePath);

                    if (cachedPageData != default)
                        pages.Add(new PngPage(pageNumber, cachedPageData));
                    else
                        pagesToCreate.Add(pageNumber);
                }

                if (pagesToCreate.Any())
                {
                    await UploadFileIfNotExists(filePath);

                    var createdPages = await CreatePagesAsync(filePath, password, pagesToCreate);
                    pages.AddRange(createdPages);
                }

                return new Pages(pages);
            }
        }

        private async Task<List<Page>> CreatePagesAsync(string filePath, string password, List<int> pagesToCreate)
        {
            var pages = new List<Page>();

            var fileInfo = new FileInfo
            {
                FilePath = filePath,
                Password = password,
                StorageName = _config.StorageName
            };

            var createPagesResult = await _viewerApiConnect.CreatePagesAsync(
                pagesToCreate.ToArray(), ViewOptions.ViewFormatEnum.PNG, fileInfo);
            if (createPagesResult.IsFailure)
                throw new Exception(createPagesResult.Message);

            foreach (var pageView in createPagesResult.Value.Pages)
            {
                var pageNumber = pageView.Number.GetValueOrDefault();
                var downloadResult = await _viewerApiConnect.DownloadResourceAsync(pageView, _config.StorageName);
                if (downloadResult.IsFailure)
                    throw new Exception(downloadResult.Message);

                var bytes = downloadResult.Value;
                string cacheKey = CacheKeys.GetPngPageCacheKey(pageNumber);

                await _fileCache.SetAsync(cacheKey, filePath, bytes);

                pages.Add(new PngPage(pageNumber, bytes));
            }

            return pages;
        }

        public Task<DocumentInfo> GetDocumentInfoAsync(string filePath, string password) =>
            _fileCache.GetValueAsync(CacheKeys.FILE_INFO_CACHE_KEY, filePath, async () =>
            {
                var fileInfo = new FileInfo
                {
                    FilePath = filePath,
                    Password = password,
                    StorageName = _config.StorageName
                };

                await UploadFileIfNotExists(filePath);

                var result = await _viewerApiConnect.GetDocumentInfoAsync(ViewOptions.ViewFormatEnum.PNG, fileInfo);

                if (result.IsFailure)
                    throw new Exception(result.Message);

                return result.Value;
            });

        public Task<byte[]> GetPdfAsync(string filePath, string password) =>
            _fileCache.GetValueAsync(CacheKeys.PDF_FILE_CACHE_KEY, filePath, async () =>
            {
                var fileInfo = new FileInfo
                {
                    FilePath = filePath,
                    Password = password,
                    StorageName = _config.StorageName
                };

                await UploadFileIfNotExists(filePath);

                var result = await _viewerApiConnect
                    .GetPdfFileAsync(fileInfo);

                if (result.IsFailure)
                    throw new Exception(result.Message);

                return result.Value;
            });

        public Task<byte[]> GetPageResourceAsync(string filePath, string password, int pageNumber,
            string resourceName) => throw new NotImplementedException("This method is not supported.");

        private async Task UploadFileIfNotExists(string filePath)
        {
            using (await _asyncDuplicateLock.LockAsync(filePath))
            {
                var existsResult = await _viewerApiConnect.CheckFileExistsAsync(filePath, _config.StorageName);
                if (existsResult.IsFailure)
                    throw new Exception(existsResult.Message);

                if (!existsResult.Value)
                {
                    var bytes = await _fileStorage.ReadFileAsync(filePath);
                    var uploadResult = await _viewerApiConnect.UploadFileAsync(filePath, _config.StorageName, bytes);
                    if (uploadResult.IsFailure)
                        throw new Exception(uploadResult.Message);
                }
            }
        }
    }

    public sealed class AsyncDuplicateLock
    {
        private sealed class RefCounted<T>
        {
            public RefCounted(T value)
            {
                RefCount = 1;
                Value = value;
            }

            public int RefCount { get; set; }
            public T Value { get; private set; }
        }

        private static readonly Dictionary<object, RefCounted<SemaphoreSlim>> SemaphoreSlims
            = new Dictionary<object, RefCounted<SemaphoreSlim>>();

        private SemaphoreSlim GetOrCreate(object key)
        {
            RefCounted<SemaphoreSlim> item;
            lock (SemaphoreSlims)
            {
                if (SemaphoreSlims.TryGetValue(key, out item))
                {
                    ++item.RefCount;
                }
                else
                {
                    item = new RefCounted<SemaphoreSlim>(new SemaphoreSlim(1, 1));
                    SemaphoreSlims[key] = item;
                }
            }
            return item.Value;
        }

        public IDisposable Lock(object key)
        {
            GetOrCreate(key).Wait();
            return new Releaser { Key = key };
        }

        public async Task<IDisposable> LockAsync(object key)
        {
            await GetOrCreate(key).WaitAsync().ConfigureAwait(false);
            return new Releaser { Key = key };
        }

        private sealed class Releaser : IDisposable
        {
            public object Key { get; set; }

            public void Dispose()
            {
                RefCounted<SemaphoreSlim> item;
                lock (SemaphoreSlims)
                {
                    item = SemaphoreSlims[Key];
                    --item.RefCount;
                    if (item.RefCount == 0)
                        SemaphoreSlims.Remove(Key);
                }
                item.Value.Release();
            }
        }
    }
}