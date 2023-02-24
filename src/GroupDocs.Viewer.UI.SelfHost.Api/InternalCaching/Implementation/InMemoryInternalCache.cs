using System;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using Microsoft.Extensions.Caching.Memory;

// ReSharper disable once CheckNamespace
namespace GroupDocs.Viewer.UI.SelfHost.Api.InternalCaching
{
    public class InMemoryInternalCache : IInternalCache
    {
        private readonly IMemoryCache _cache;
        private readonly InternalCacheOptions _options;

        public InMemoryInternalCache(IMemoryCache cache, InternalCacheOptions options)
        {
            _cache = cache;
            _options = options;
        }

        public bool TryGet(FileCredentials fileCredentials, out Viewer viewer)
        {
            var key = GetKey(fileCredentials);
            if (_cache.TryGetValue(key, out object obj))
            {
                viewer = (Viewer)obj;
                return true;
            }

            viewer = null;
            return false;
        }

        public void Set(FileCredentials fileCredentials, Viewer entry)
        {
            var entryKey = GetKey(fileCredentials);
            if (!_cache.TryGetValue(entryKey, out var obj))
            {
                var entryOptions = CreateCacheEntryOptions();
                _cache.Set(entryKey, entry, entryOptions);
            }
        }

        private string GetKey(FileCredentials fileCredentials) => 
            $"{fileCredentials.FilePath}_{fileCredentials.Password}__VC";

        private MemoryCacheEntryOptions CreateCacheEntryOptions()
        {
            var entryOptions = new MemoryCacheEntryOptions();

            if (_options.CacheEntryExpirationTimeoutMinutes > 0)
                entryOptions.SlidingExpiration = TimeSpan.FromMinutes(_options.CacheEntryExpirationTimeoutMinutes);

            entryOptions.RegisterPostEvictionCallback(
                callback: (key, value, evictionReason, state) =>
                {
                    if (value is Viewer viewer)
                        viewer.Dispose();
                });

            return entryOptions;
        }
    }
}