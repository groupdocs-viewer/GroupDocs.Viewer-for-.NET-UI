using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core;
using Microsoft.Extensions.Caching.Distributed;

namespace GroupDocs.Viewer.UI.Api.Distributed.Cache
{
    public class DistributedFileCache : IFileCache
    {
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _entryOptions;

        public DistributedFileCache(IDistributedCache cache, DistributedCacheEntryOptions entryOptions = null)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _entryOptions = entryOptions ?? new DistributedCacheEntryOptions();
        }

        public TEntry TryGetValue<TEntry>(string cacheKey, string filePath)
        {
            string key = BuildKey(cacheKey, filePath);
            byte[] data = _cache.Get(key);

            if (data == null)
                return default;

            return Deserialize<TEntry>(data);
        }

        public async Task<TEntry> TryGetValueAsync<TEntry>(string cacheKey, string filePath, CancellationToken cancellationToken = default)
        {
            string key = BuildKey(cacheKey, filePath);
            byte[] data = await _cache.GetAsync(key, cancellationToken);

            if (data == null)
                return default;

            return Deserialize<TEntry>(data);
        }

        public void Set<TEntry>(string cacheKey, string filePath, TEntry entry)
        {
            if (entry == null)
                return;

            string key = BuildKey(cacheKey, filePath);
            byte[] data = Serialize(entry);
            _cache.Set(key, data, _entryOptions);
        }

        public async Task SetAsync<TEntry>(string cacheKey, string filePath, TEntry entry, CancellationToken cancellationToken = default)
        {
            if (entry == null)
                return;

            string key = BuildKey(cacheKey, filePath);
            byte[] data = Serialize(entry);
            await _cache.SetAsync(key, data, _entryOptions, cancellationToken);
        }

        public async Task RemoveAsync(string filePath, CancellationToken cancellationToken = default)
        {
            // IDistributedCache doesn't support prefix-based removal.
            // Remove the well-known key pattern used for file path tracking.
            string trackingKey = BuildKey("__keys", filePath);
            await _cache.RemoveAsync(trackingKey, cancellationToken);
        }

        private static string BuildKey(string cacheKey, string filePath)
            => $"gd_viewer:{filePath}:{cacheKey}";

        private static byte[] Serialize<TEntry>(TEntry entry)
        {
            if (entry is byte[] bytes)
                return bytes;

            return JsonSerializer.SerializeToUtf8Bytes(entry);
        }

        private static TEntry Deserialize<TEntry>(byte[] data)
        {
            if (typeof(TEntry) == typeof(byte[]))
                return (TEntry)(object)data;

            return JsonSerializer.Deserialize<TEntry>(data);
        }
    }
}
