using GroupDocs.Viewer.UI.SelfHost.Api;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;

namespace Microsoft.Extensions.Configuration
{
    public static class ConfigurationExtensions
    {
        public static Config BindSelfHostApiSettings
            (this IConfiguration configuration, Config config)
        {
            configuration
                .GetSection(Keys.SELF_HOST_API_SECTION_SETTING_KEY)
                .Bind(config, c => c.BindNonPublicProperties = true);

            return config;
        }
    }
}