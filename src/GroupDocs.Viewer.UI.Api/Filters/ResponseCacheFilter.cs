using GroupDocs.Viewer.UI.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.Api.Filters
{
    public class ResponseCacheFilter : IActionFilter
    {
        private readonly Config _config;

        public ResponseCacheFilter(IOptions<Config> config)
        {
            _config = config.Value;
        }

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (_config.ResponseCacheDurationSeconds <= 0)
                return;

            if (context.HttpContext.Request.Method != "GET")
                return;

            var headers = context.HttpContext.Response.Headers;
            headers["Cache-Control"] = $"public, max-age={_config.ResponseCacheDurationSeconds}";
        }
    }
}
