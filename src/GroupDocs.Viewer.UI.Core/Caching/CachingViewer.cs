using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Core.Caching
{
    public class CachingViewer : IViewer
    {
        private readonly IViewer _viewer;
        private readonly IFileCache _fileCache;
        private readonly IAsyncLock _asyncLock;

        public string PageExtension =>
            _viewer.PageExtension;

        public Page CreatePage(int pageNumber, byte[] data) =>
            _viewer.CreatePage(pageNumber, data);

        public CachingViewer(IViewer viewer, IFileCache fileCache, IAsyncLock asyncLock)
        {
            _viewer = viewer;
            _fileCache = fileCache;
            _asyncLock = asyncLock;
        }

        public async Task<Pages> GetPagesAsync(string filePath, string password, int[] pageNumbers)
        {
            var pagesOrNulls = GetPagesOrNullsFromCache(filePath, pageNumbers);
            var missingPageNumbers = GetMissingPageNumbers(pagesOrNulls);

            if (missingPageNumbers.Length == 0)
                return ToPages(pagesOrNulls);

            var createdPages = await CreatePages(filePath, password, missingPageNumbers);

            var pages = Combine(pagesOrNulls, createdPages);

            return pages;
        }

        public async Task<Page> GetPageAsync(string filePath, string password, int pageNumber)
        {
            var cacheKey = CacheKeys.GetPageCacheKey(pageNumber, PageExtension);
            var bytes = await _fileCache.GetValueAsync(cacheKey, filePath, async () =>
            {
                using (await _asyncLock.LockAsync(filePath))
                {
                    return await _fileCache.GetValueAsync(cacheKey, filePath, async () =>
                    {
                        var page = await _viewer.GetPageAsync(filePath, password, pageNumber);

                        await SaveResourcesAsync(filePath, page.PageNumber, page.Resources);

                        return page.Data;
                    });
                }
            });

            var page = CreatePage(pageNumber, bytes);
            return page;
        }

        public Task<DocumentInfo> GetDocumentInfoAsync(string filePath, string password)
        {
            var cacheKey = CacheKeys.FILE_INFO_CACHE_KEY;
            return _fileCache.GetValueAsync(cacheKey, filePath, async () =>
            {
                using (await _asyncLock.LockAsync(filePath))
                {
                    return await _fileCache.GetValueAsync(cacheKey, filePath, () =>
                        _viewer.GetDocumentInfoAsync(filePath, password));
                }
            });
        }

        public Task<byte[]> GetPdfAsync(string filePath, string password)
        {
            var cacheKey = CacheKeys.PDF_FILE_CACHE_KEY;
            return _fileCache.GetValueAsync(cacheKey, filePath, async () =>
            {
                using (await _asyncLock.LockAsync(filePath))
                {
                    return await _fileCache.GetValueAsync(cacheKey, filePath, () =>
                        _viewer.GetPdfAsync(filePath, password));
                }
            });
        }

        public Task<byte[]> GetPageResourceAsync(string filePath, string password, int pageNumber,
            string resourceName)
        {
            var cacheKey = CacheKeys.GetHtmlPageResourceCacheKey(pageNumber, resourceName);
            return _fileCache.GetValueAsync(cacheKey, filePath,
                async () =>
                {
                    using (await _asyncLock.LockAsync(filePath))
                    {
                        return await _fileCache.GetValueAsync(cacheKey, filePath, () =>
                            _viewer.GetPageResourceAsync(filePath, password, pageNumber, resourceName));
                    }
                });
        }

        private async Task SaveResourcesAsync(string filePath, int pageNumber, IEnumerable<PageResource> pageResources)
        {
            var tasks = pageResources.Select(resource =>
            {
                var resourceCacheKey =
                    CacheKeys.GetHtmlPageResourceCacheKey(pageNumber, resource.ResourceName);

                return _fileCache.SetAsync(resourceCacheKey, filePath, resource.Data);
            });

            await Task.WhenAll(tasks);
        }

        private async Task<Pages> CreatePages(string filePath, string password, int[] pageNumbers)
        {
            using (await _asyncLock.LockAsync(filePath))
            {
                var pagesOrNulls = GetPagesOrNullsFromCache(filePath, pageNumbers);
                var missingPageNumbers = GetMissingPageNumbers(pagesOrNulls);

                if (missingPageNumbers.Length == 0)
                    return ToPages(pagesOrNulls);

                var createdPages = await _viewer.GetPagesAsync(filePath, password, missingPageNumbers);

                await SaveToCache(filePath, createdPages);

                var pages = Combine(pagesOrNulls, createdPages);

                return pages;
            }   
        }

        private Pages Combine(List<CachedPage> dst, Pages missing)
        {
            var pages = dst
                .Select(pageOrNull =>
                {
                    if (pageOrNull.Data == null)
                    {
                        var page = missing
                            .Where(page => page.PageNumber == pageOrNull.PageNumber)
                            .Select(page => page)
                            .FirstOrDefault();

                        return page;
                    }

                    return CreatePage(pageOrNull.PageNumber, pageOrNull.Data);
                }).ToList();

            return new Pages(pages);
        }

        private Task SaveToCache(string filePath, Pages createdPages)
        {
            var tasks = createdPages
                .SelectMany(page =>
                {
                    var cacheKey = CacheKeys.GetPageCacheKey(page.PageNumber, _viewer.PageExtension);

                    var savePageTask = _fileCache.SetAsync(cacheKey, filePath, page.Data);
                    var saveResourcesTask = SaveResourcesAsync(filePath, page.PageNumber, page.Resources);

                    return new[] {savePageTask, saveResourcesTask};
                });

            return Task.WhenAll(tasks);
        }

        private Pages ToPages(List<CachedPage> pagesOrNulls)
        {
            var pages = pagesOrNulls
                .Select(p => CreatePage(p.PageNumber, p.Data))
                .ToList();

            var result = new Pages(pages);
            return result;
        }

        private int[] GetMissingPageNumbers(List<CachedPage> pagesOrNulls)
        {
            return pagesOrNulls
                .Where(p => p.Data == null)
                .Select(p => p.PageNumber)
                .ToArray();
        }

        private List<CachedPage> GetPagesOrNullsFromCache(string filePath, int[] pageNumbers)
        {
            return pageNumbers
                .Select(pageNumber =>
                    (pageNumber, cacheKey: CacheKeys.GetPageCacheKey(pageNumber, PageExtension)))
                .Select(page =>
                    new CachedPage(page.pageNumber, _fileCache.TryGetValue<byte[]>(page.cacheKey, filePath)))
                .ToList();
        }
    }
}