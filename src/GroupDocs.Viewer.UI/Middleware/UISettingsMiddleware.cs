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
        private readonly JsonSerializerSettings _jsonSerializationSettings;
        private readonly Lazy<dynamic> _uiOutputSettings = new Lazy<dynamic>(GetUIOutputSettings);
        
        public UISettingsMiddleware(RequestDelegate next, IOptions<Config> settings)
        {
            _ = settings ?? throw new ArgumentNullException(nameof(settings));
            Config = settings.Value;

            _jsonSerializationSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string content = JsonConvert.SerializeObject(_uiOutputSettings.Value, _jsonSerializationSettings);
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
            };
        }
    }
}
