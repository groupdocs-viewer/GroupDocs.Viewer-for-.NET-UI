namespace GroupDocs.Viewer.UI.SelfHost.Api.Configuration
{
    /// <summary>
    /// This class contains options for internal objects caching.
    /// </summary>
    public class InternalCacheOptions
    {
        public static readonly InternalCacheOptions CacheForFiveMinutes =
            new InternalCacheOptions { IsCacheEnabled = true, CacheEntryExpirationTimeoutMinutes = 5 };

        internal bool IsCacheEnabled { get; private set; }

        internal bool IsCacheDisabled => !IsCacheEnabled;

        internal int CacheEntryExpirationTimeoutMinutes { get; private set; }

        /// <summary>
        /// Turn of internal caching.
        /// By default caching is enabled.
        /// </summary>
        /// <returns>This instance.</returns>
        public InternalCacheOptions DisableInternalCache()
        {
            IsCacheEnabled = false;
            return this;
        }

        /// <summary>
        /// Set the sliding expiration timeout of each cache entry in minutes.
        /// The default value is 5 minutes.
        /// </summary>
        /// <param name="cacheEntryExpirationTimeoutMinutes">The expiration timeout in minutes.</param>
        /// <returns>This instance.</returns>
        public InternalCacheOptions SetCacheEntryExpirationTimeoutMinutes(int cacheEntryExpirationTimeoutMinutes)
        {
            CacheEntryExpirationTimeoutMinutes = cacheEntryExpirationTimeoutMinutes;
            return this;
        }
    }
}