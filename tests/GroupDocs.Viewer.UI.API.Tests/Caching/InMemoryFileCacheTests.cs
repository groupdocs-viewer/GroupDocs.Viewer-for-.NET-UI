using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.InMemory.Cache;
using GroupDocs.Viewer.UI.Api.InMemory.Cache.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Caching
{
    public class InMemoryFileCacheTests : IDisposable
    {
        private readonly MemoryCache _memoryCache;
        private readonly InMemoryFileCache _cache;

        public InMemoryFileCacheTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            var config = new Config()
                .SetCacheEntryExpirationTimeoutMinutes(5)
                .SetGroupCacheEntriesByFile(true);
            _cache = new InMemoryFileCache(_memoryCache, MSOptions.Create(config));
        }

        public void Dispose()
        {
            _memoryCache.Dispose();
        }

        [Fact]
        public void Set_And_TryGetValue_ShouldRoundtrip()
        {
            var data = new byte[] { 1, 2, 3 };
            _cache.Set("page.png", "doc.pdf", data);

            var result = _cache.TryGetValue<byte[]>("page.png", "doc.pdf");

            Assert.Equal(data, result);
        }

        [Fact]
        public async Task SetAsync_And_TryGetValueAsync_ShouldRoundtrip()
        {
            var data = new byte[] { 4, 5, 6 };
            await _cache.SetAsync("page.png", "doc.pdf", data);

            var result = await _cache.TryGetValueAsync<byte[]>("page.png", "doc.pdf");

            Assert.Equal(data, result);
        }

        [Fact]
        public void TryGetValue_WhenNotCached_ShouldReturnDefault()
        {
            var result = _cache.TryGetValue<byte[]>("missing.png", "doc.pdf");
            Assert.Null(result);
        }

        [Fact]
        public async Task RemoveAsync_ShouldEvictGroupedEntries()
        {
            _cache.Set("page1.png", "doc.pdf", new byte[] { 1 });
            _cache.Set("page2.png", "doc.pdf", new byte[] { 2 });

            await _cache.RemoveAsync("doc.pdf");

            var result1 = _cache.TryGetValue<byte[]>("page1.png", "doc.pdf");
            var result2 = _cache.TryGetValue<byte[]>("page2.png", "doc.pdf");
            Assert.Null(result1);
            Assert.Null(result2);
        }

        [Fact]
        public async Task RemoveAsync_ShouldNotAffectOtherFiles()
        {
            var data = new byte[] { 1, 2, 3 };
            _cache.Set("page.png", "doc1.pdf", data);
            _cache.Set("page.png", "doc2.pdf", new byte[] { 9 });

            await _cache.RemoveAsync("doc2.pdf");

            var result = _cache.TryGetValue<byte[]>("page.png", "doc1.pdf");
            Assert.Equal(data, result);
        }

        [Fact]
        public async Task RemoveAsync_WhenNothingCached_ShouldNotThrow()
        {
            await _cache.RemoveAsync("nonexistent.pdf");
        }

        [Fact]
        public void Set_WithSizeLimit_ShouldAcceptEntries()
        {
            using var memoryCache = new MemoryCache(new MemoryCacheOptions { SizeLimit = 100 });
            var config = new Config().SetSizeLimit(100);
            var cache = new InMemoryFileCache(memoryCache, MSOptions.Create(config));

            cache.Set("page1.png", "doc.pdf", new byte[] { 1 });
            cache.Set("page2.png", "doc.pdf", new byte[] { 2 });

            Assert.NotNull(cache.TryGetValue<byte[]>("page1.png", "doc.pdf"));
            Assert.NotNull(cache.TryGetValue<byte[]>("page2.png", "doc.pdf"));
        }
    }
}
