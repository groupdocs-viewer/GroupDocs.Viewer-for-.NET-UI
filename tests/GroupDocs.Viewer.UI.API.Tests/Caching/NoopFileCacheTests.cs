using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core.FileCaching;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Caching
{
    public class NoopFileCacheTests
    {
        private readonly NoopFileCache _cache = new();

        [Fact]
        public void TryGetValue_ShouldAlwaysReturnDefault()
        {
            var result = _cache.TryGetValue<byte[]>("key", "file");
            Assert.Null(result);
        }

        [Fact]
        public async Task TryGetValueAsync_ShouldAlwaysReturnDefault()
        {
            var result = await _cache.TryGetValueAsync<byte[]>("key", "file");
            Assert.Null(result);
        }

        [Fact]
        public void Set_ShouldNotThrow()
        {
            _cache.Set("key", "file", new byte[] { 1 });
        }

        [Fact]
        public async Task RemoveAsync_ShouldNotThrow()
        {
            await _cache.RemoveAsync("file");
        }
    }
}
