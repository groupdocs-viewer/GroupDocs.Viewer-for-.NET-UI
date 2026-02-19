using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Distributed.Cache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Caching
{
    public class DistributedFileCacheTests
    {
        private readonly DistributedFileCache _cache;

        public DistributedFileCacheTests()
        {
            var distributedCache = new MemoryDistributedCache(
                MSOptions.Create(new MemoryDistributedCacheOptions()));
            _cache = new DistributedFileCache(distributedCache);
        }

        [Fact]
        public void Set_And_TryGetValue_ShouldRoundtripBytes()
        {
            var data = new byte[] { 1, 2, 3 };
            _cache.Set("page.png", "doc.pdf", data);

            var result = _cache.TryGetValue<byte[]>("page.png", "doc.pdf");

            Assert.Equal(data, result);
        }

        [Fact]
        public async Task SetAsync_And_TryGetValueAsync_ShouldRoundtripBytes()
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
        public async Task RemoveAsync_ShouldNotThrow()
        {
            await _cache.SetAsync("page.png", "doc.pdf", new byte[] { 1 });
            await _cache.RemoveAsync("doc.pdf");
        }

        [Fact]
        public async Task SetAsync_And_TryGetValueAsync_ShouldRoundtripJsonObjects()
        {
            var data = new TestData { Name = "test", Value = 42 };
            await _cache.SetAsync("info.json", "doc.pdf", data);

            var result = await _cache.TryGetValueAsync<TestData>("info.json", "doc.pdf");

            Assert.NotNull(result);
            Assert.Equal("test", result.Name);
            Assert.Equal(42, result.Value);
        }

        [Fact]
        public async Task TryGetValueAsync_SupportsCancellationToken()
        {
            await _cache.SetAsync("page.png", "doc.pdf", new byte[] { 1 });
            using var cts = new CancellationTokenSource();

            var result = await _cache.TryGetValueAsync<byte[]>("page.png", "doc.pdf", cts.Token);
            Assert.NotNull(result);
        }

        private class TestData
        {
            public string? Name { get; set; }
            public int Value { get; set; }
        }
    }
}
