using GroupDocs.Viewer.UI.Api.Cloud.Storage;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.Configuration;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public static Config BindCloudApiSettings
            (this IConfiguration configuration, Config config)
        {
            configuration
                .GetSection(Keys.GROUPDOCSVIEWERUI_API_CLOUD_STORAGE_SECTION_SETTING_KEY)
                .Bind(config, c => c.BindNonPublicProperties = true);

            return config;
        }
    }
}