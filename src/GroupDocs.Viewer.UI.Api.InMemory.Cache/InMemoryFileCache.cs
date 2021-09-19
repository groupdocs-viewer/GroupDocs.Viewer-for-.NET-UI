using System;
using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.InMemory.Cache.Configuration;
using GroupDocs.Viewer.UI.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace GroupDocs.Viewer.UI.Api.InMemory.Cache
{
    public class InMemoryFileCache : IFileCache
    {
        private readonly IMemoryCache _cache;
        private readonly Config _config;

        public InMemoryFileCache(IMemoryCache cache, IOptions<Config> config)
        {
            _cache = cache;
            _config = config.Value;
        }

        public TEntry TryGetValue<TEntry>(string cacheKey, string filePath)
        {
            string key = $"{filePath}_{cacheKey}";
            if (_cache.TryGetValue(key, out object obj))
                return (TEntry) obj;

            return default;
        }

        public Task<TEntry> TryGetValueAsync<TEntry>(string cacheKey, string filePath)
        {
            TEntry entry = TryGetValue<TEntry>(cacheKey, filePath);
            return Task.FromResult(entry);
        }

        public void Set<TEntry>(string cacheKey, string filePath, TEntry entry)
        {
            MemoryCacheEntryOptions entryOptions;

            if (_config.CacheEntryExpirationTimeoutMinutes > 0)
            {
                var cts = GetOrCreateCancellationTokenSource(cacheKey, filePath);
                entryOptions = CreateCacheEntryOptions(cts);
            }
            else
            {
                entryOptions = CreateCacheEntryOptions();
            }

            string key = $"{filePath}_{cacheKey}";
            _cache.Set(key, entry, entryOptions);
        }

        public Task SetAsync<TEntry>(string cacheKey, string filePath, TEntry entry)
        {
            Set(cacheKey, filePath, entry);
            return Task.CompletedTask;
        }

        private CancellationTokenSource GetOrCreateCancellationTokenSource(string cacheKey, string filePath)
        {
            var ctsCacheKey = _config.GroupCacheEntriesByFile
                ? $"{filePath}__CTS"
                : $"{filePath}_{cacheKey}__CTS";

            var cts = _cache.Get<CancellationTokenSource>(ctsCacheKey);
            if (cts == null || cts.IsCancellationRequested)
            {
                using (var ctsEntry = _cache.CreateEntry(ctsCacheKey))
                {
                    cts = CreateCancellationTokenSource();

                    ctsEntry.Value = cts;
                    ctsEntry.AddExpirationToken(CreateCancellationChangeToken(cts));
                }
            }

            return cts;
        }

        private MemoryCacheEntryOptions CreateCacheEntryOptions(CancellationTokenSource cts)
        {
            var entryOptions = new MemoryCacheEntryOptions();
            entryOptions.AddExpirationToken(CreateCancellationChangeToken(cts));

            return entryOptions;
        }
    
        private MemoryCacheEntryOptions CreateCacheEntryOptions()
        {
            var entryOptions = new MemoryCacheEntryOptions();
            return entryOptions;
        }

        private CancellationChangeToken CreateCancellationChangeToken(CancellationTokenSource cancellationTokenSource) 
            => new CancellationChangeToken(cancellationTokenSource.Token);

        private CancellationTokenSource CreateCancellationTokenSource() =>
            new CancellationTokenSource(TimeSpan.FromMinutes(_config.CacheEntryExpirationTimeoutMinutes));
    }
}