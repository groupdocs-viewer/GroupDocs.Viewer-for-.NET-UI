using System;
using System.Threading;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

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
            MemoryCacheEntryOptions entryOptions;
            string key = GetKey(fileCredentials);

            if (_options.CacheEntryExpirationTimeoutMinutes > 0)
            {
                var cts = GetOrCreateCancellationTokenSource(key);
                entryOptions = CreateCacheEntryOptions(cts);
            }
            else
            {
                entryOptions = CreateCacheEntryOptions();
            }

            _cache.Set(key, entry, entryOptions);
        }

        private string GetKey(FileCredentials fileCredentials) => 
            $"{fileCredentials.FilePath}_{fileCredentials.Password}__VC";

        private CancellationTokenSource GetOrCreateCancellationTokenSource(string key)
        {
            var ctsCacheKey = $"{key}_CTS";

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
            entryOptions.RegisterPostEvictionCallback(
                callback: (key, value, evictionReason, state) =>
                {
                    if (value is Viewer viewer)
                        viewer.Dispose();
                });

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
            new CancellationTokenSource(TimeSpan.FromMinutes(_options.CacheEntryExpirationTimeoutMinutes));
    }
}