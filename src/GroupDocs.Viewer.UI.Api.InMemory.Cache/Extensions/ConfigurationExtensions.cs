using GroupDocs.Viewer.UI.Api.InMemory.Cache;
using GroupDocs.Viewer.UI.Api.InMemory.Cache.Configuration;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public static Config BindInMemoryCacheSettings
            (this IConfiguration configuration, Config config)
        {
            configuration
                .GetSection(Keys.GROUPDOCS_VIEWER_UI_API_INMEMORY_CACHE_SETTINGS_KEY)
                .Bind(config, c => c.BindNonPublicProperties = true);

            return config;
        }
    }
}