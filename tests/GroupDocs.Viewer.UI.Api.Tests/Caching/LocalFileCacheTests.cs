using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Local.Cache;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Caching
{
    public class LocalFileCacheTests : IDisposable
    {
        private readonly string _basePath;
        private readonly string _cachePath;
        private readonly string _outsidePath;
        private readonly LocalFileCache _cache;

        public LocalFileCacheTests()
        {
            _basePath = Path.Combine(Path.GetTempPath(), $"viewer_cache_test_{Guid.NewGuid():N}");
            _cachePath = Path.Combine(_basePath, "Cache");
            _outsidePath = Path.Combine(_basePath, "Outside");
            Directory.CreateDirectory(_cachePath);
            Directory.CreateDirectory(_outsidePath);
            _cache = new LocalFileCache(_cachePath);
        }

        public void Dispose()
        {
            if (Directory.Exists(_basePath))
                Directory.Delete(_basePath, recursive: true);
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

        [Theory]
        [InlineData("info.json")]
        [InlineData("file.pdf")]
        [InlineData("p1.html")]
        [InlineData("p1_t.png")]
        [InlineData("p1_f0.woff")]
        public async Task SetAsync_WithViewerCacheKey_ShouldRoundtripBytes(string cacheKey)
        {
            var data = new byte[] { 7, 8, 9 };
            await _cache.SetAsync(cacheKey, "doc.pdf", data);

            var result = await _cache.TryGetValueAsync<byte[]>(cacheKey, "doc.pdf");

            Assert.Equal(data, result);
        }

        [Fact]
        public async Task SetAsync_WithNestedFilePath_ShouldKeepCacheFolderLayout()
        {
            await _cache.SetAsync("p1.html", "sub/doc.pdf", new byte[] { 1 });

            Assert.True(File.Exists(Path.Combine(_cachePath, "sub", "doc_pdf", "p1.html")));
        }

        // The cache key of a page resource is "p{page}_{resource}" where the resource name comes from the client
        [Theory]
        [InlineData("p1_../../../../Outside/secret.txt")]
        [InlineData("sub/p1.html")]
        [InlineData("..")]
        [InlineData("")]
        public async Task TryGetValueAsync_WithInvalidCacheKey_ShouldThrow(string cacheKey)
        {
            File.WriteAllBytes(Path.Combine(_outsidePath, "secret.txt"), new byte[] { 42 });

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _cache.TryGetValueAsync<byte[]>(cacheKey, "doc.pdf"));
        }

        [Fact]
        public void TryGetValue_WithTraversalInCacheKey_ShouldThrow()
        {
            File.WriteAllBytes(Path.Combine(_outsidePath, "secret.txt"), new byte[] { 42 });

            Assert.Throws<UnauthorizedAccessException>(
                () => _cache.TryGetValue<byte[]>("p1_../../../../Outside/secret.txt", "doc.pdf"));
        }

        [Fact]
        public async Task TryGetValueAsync_WithBackslashTraversalInCacheKey_ShouldThrow()
        {
            // Backslash is a plain file name character on Linux and macOS
            if (!OperatingSystem.IsWindows())
                return;

            File.WriteAllBytes(Path.Combine(_outsidePath, "secret.txt"), new byte[] { 42 });

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _cache.TryGetValueAsync<byte[]>(@"p1_..\..\..\..\Outside\secret.txt", "doc.pdf"));
        }

        [Theory]
        [InlineData("p1_../../../../Outside/evil.txt")]
        [InlineData("sub/p1.html")]
        public async Task SetAsync_WithInvalidCacheKey_ShouldThrowAndNotWriteFile(string cacheKey)
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _cache.SetAsync(cacheKey, "doc.pdf", new byte[] { 1 }));

            Assert.False(File.Exists(Path.Combine(_outsidePath, "evil.txt")));
        }

        [Fact]
        public void Set_WithTraversalInCacheKey_ShouldThrowAndNotWriteFile()
        {
            Assert.Throws<UnauthorizedAccessException>(
                () => _cache.Set("p1_../../../../Outside/evil.txt", "doc.pdf", new byte[] { 1 }));

            Assert.False(File.Exists(Path.Combine(_outsidePath, "evil.txt")));
        }

        [Fact]
        public async Task SetAsync_WithRootedFilePath_ShouldStoreEntryInsideCacheFolder()
        {
            var filePath = Path.Combine(_outsidePath, "doc.pdf");
            var data = new byte[] { 1, 2, 3 };

            await _cache.SetAsync("p1.html", filePath, data);
            var result = await _cache.TryGetValueAsync<byte[]>("p1.html", filePath);

            Assert.Equal(data, result);
            Assert.Empty(Directory.EnumerateFileSystemEntries(_outsidePath));
            Assert.Single(Directory.EnumerateFiles(_cachePath, "p1.html", SearchOption.AllDirectories));
        }

        [Fact]
        public async Task RemoveAsync_WithRootedFilePath_ShouldNotDeleteOutsideCacheFolder()
        {
            var outsideFilePath = Path.Combine(_outsidePath, "keep.txt");
            File.WriteAllText(outsideFilePath, "keep");

            await _cache.RemoveAsync(_outsidePath);

            Assert.True(File.Exists(outsideFilePath));
        }
    }
}
