namespace GroupDocs.Viewer.UI.Api.InMemory.Cache.Configuration
{
    public class Config
    {
        internal int CacheEntryExpirationTimeoutMinutes { get; private set; }

        internal long? SizeLimit { get; private set; }

        public bool GroupCacheEntriesByFile { get; private set; }

        /// <summary>
        /// Set the expiration timeout of each cache entry in minutes.
        /// The default value is 0 which means that a cache entry never expires.
        /// </summary>
        /// <param name="cacheEntryExpirationTimeoutMinutes">The expiration timeout in minutes.</param>
        /// <returns>This instance.</returns>
        public Config SetCacheEntryExpirationTimeoutMinutes(int cacheEntryExpirationTimeoutMinutes)
        {
            CacheEntryExpirationTimeoutMinutes = cacheEntryExpirationTimeoutMinutes;
            return this;
        }

        /// <summary>
        /// Set grouping cache entries by file. When enabled eviction of any cache entry leads to eviction of all this file cache entries.
        /// This setting has effect only when <see cref="CacheEntryExpirationTimeoutMinutes"/> is greater than zero.
        /// </summary>
        /// <param name="groupCacheEntriesByFile"><value>True</value> to enable grouping.</param>
        /// <returns>This instance.</returns>
        /// <summary>
        /// Set the maximum number of entries the cache can hold.
        /// When the limit is reached, the cache evicts entries based on the built-in eviction policy.
        /// The default value is null (no size limit).
        /// </summary>
        /// <param name="sizeLimit">The maximum number of cache entries.</param>
        /// <returns>This instance.</returns>
        public Config SetSizeLimit(long sizeLimit)
        {
            SizeLimit = sizeLimit;
            return this;
        }

        public Config SetGroupCacheEntriesByFile(bool groupCacheEntriesByFile)
        {
            GroupCacheEntriesByFile = groupCacheEntriesByFile;
            return this;
        }
    }
}