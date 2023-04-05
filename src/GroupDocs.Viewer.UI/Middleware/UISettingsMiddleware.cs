using GroupDocs.Viewer.UI.Api;
using GroupDocs.Viewer.UI.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Middleware
{
    internal class UISettingsMiddleware
    {
        private static Config Config { get; set; }
        private static IUIConfigProvider ConfigProvider { get; set; }

        private readonly JsonSerializerSettings _jsonSerializationSettings;

        public UISettingsMiddleware(RequestDelegate next,
            IOptions<Config> settings,
            IUIConfigProvider uIConfigProvider)
        {
            _ = settings ?? throw new ArgumentNullException(nameof(settings));
            Config = settings.Value;
            ConfigProvider = uIConfigProvider;

            _jsonSerializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            ConfigProvider.ConfigureUI(Config);
            string content = JsonConvert.SerializeObject(GetUIOutputSettings(), _jsonSerializationSettings);
            context.Response.ContentType = Keys.DEFAULT_RESPONSE_CONTENT_TYPE;

            await context.Response.WriteAsync(content);
        }

        private static dynamic GetUIOutputSettings()
        {
            return new
            {
                Config.PageSelector,
                Config.Download,
                Config.Upload,
                Config.Print,
                Config.Browse,
                Config.Rewrite,
                Config.EnableRightClick,
                Config.DefaultDocument,
                Config.PreloadPageCount,
                Config.Zoom,
                Config.Search,
                Config.Thumbnails,
                Config.HtmlMode,
                Config.PrintAllowed,
                Config.Rotate,
                Config.SaveRotateState,
                Config.ShowLanguageMenu,
                Config.DefaultLanguage,
                Config.SupportedLanguages
            };
        }
    }
}
