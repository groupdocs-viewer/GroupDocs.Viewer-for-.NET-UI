using GroupDocs.Viewer.UI.Cloud.Api;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public static Config BindCloudApiSettings
            (this IConfiguration configuration, Config config)
        {
            configuration
                .GetSection(Keys.GROUPDOCSVIEWERUI_CLOUD_API_SECTION_SETTING_KEY)
                .Bind(config, c => c.BindNonPublicProperties = true);

            return config;
        }
    }
}