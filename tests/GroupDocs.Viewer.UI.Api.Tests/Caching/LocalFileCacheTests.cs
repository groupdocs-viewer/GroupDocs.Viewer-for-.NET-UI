using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Local.Cache;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Caching
{
    public class LocalFileCacheTests : IDisposable
    {
        private readonly string _cachePath;
        private readonly LocalFileCache _cache;

        public LocalFileCacheTests()
        {
            _cachePath = Path.Combine(Path.GetTempPath(), $"viewer_cache_test_{Guid.NewGuid():N}");
            Directory.CreateDirectory(_cachePath);
            _cache = new LocalFileCache(_cachePath);
        }

        public void Dispose()
        {
            if (Directory.Exists(_cachePath))
                Directory.Delete(_cachePath, recursive: true);
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
        public async Task RemoveAsync_ShouldEvictAllEntriesForFile()
        {
            await _cache.SetAsync("page1.png", "doc.pdf", new byte[] { 1 });
            await _cache.SetAsync("page2.png", "doc.pdf", new byte[] { 2 });

            await _cache.RemoveAsync("doc.pdf");

            var result1 = await _cache.TryGetValueAsync<byte[]>("page1.png", "doc.pdf");
            var result2 = await _cache.TryGetValueAsync<byte[]>("page2.png", "doc.pdf");
            Assert.Null(result1);
            Assert.Null(result2);
        }

        [Fact]
        public async Task RemoveAsync_ShouldNotAffectOtherFiles()
        {
            var data = new byte[] { 1, 2, 3 };
            await _cache.SetAsync("page.png", "doc1.pdf", data);
            await _cache.SetAsync("page.png", "doc2.pdf", new byte[] { 9 });

            await _cache.RemoveAsync("doc2.pdf");

            var result = await _cache.TryGetValueAsync<byte[]>("page.png", "doc1.pdf");
            Assert.Equal(data, result);
        }

        [Fact]
        public async Task RemoveAsync_WhenNothingCached_ShouldNotThrow()
        {
            await _cache.RemoveAsync("nonexistent.pdf");
        }

        [Fact]
        public async Task TryGetValueAsync_SupportsCancellation()
        {
            await _cache.SetAsync("page.png", "doc.pdf", new byte[] { 1 });
            using var cts = new CancellationTokenSource();

            var result = await _cache.TryGetValueAsync<byte[]>("page.png", "doc.pdf", cts.Token);
            Assert.NotNull(result);
        }
    }
}
