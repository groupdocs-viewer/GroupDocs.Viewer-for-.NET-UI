using GroupDocs.Viewer.UI.Api.Configuration;
using GroupDocs.Viewer.UI.Api.NetFramework.Endpoints;
using System;
using System.Web.Http;

namespace GroupDocs.Viewer.UI.Api.NetFramework.Extensions
{
    /// <summary>
    /// <code>
    /// protected void Application_Start()
    /// {
    /// AreaRegistration.RegisterAllAreas();
    /// RouteConfig.RegisterRoutes(RouteTable.Routes, options =>
    /// {
    ///     options.ApiPath = "/api"; // Example API path customization
    /// });
    /// }
    /// </code>
    /// </summary>
    public static class ViewerApiEndpointRouteBuilder
    {
        public static void RegisterViewerApi(HttpConfiguration config, Action<Options> setupOptions = null)
        {
            var options = new Options();
            setupOptions?.Invoke(options);
            config.MapHttpAttributeRoutes();
            EnsureValidApiOptions(options);
            MapControllerRoutes(config.Routes, options);

        }


        private static void MapControllerRoutes(HttpRouteCollection routes, Options options)
        {
            var relativeApiPath = options.ApiPath.AsRelativeResource();

            var apiMethods = ApiMethodMappings.ApiMethods.Keys;

            foreach (var apiMethod in apiMethods)
            {
                var action = ApiMethodMappings.ApiMethods[apiMethod];
                routes.MapHttpRoute(
                    name: action,
                    routeTemplate: $"{relativeApiPath}/{apiMethod}",
                    defaults: new { controller = ApiNames.CONTROLLER_NAME, action, optionalParam = RouteParameter.Optional }
                );
            }
        }

        private static void EnsureValidApiOptions(Options options)
        {
            Action<string, string> ensureValidPath = (string path, string argument) =>
            {
                if (string.IsNullOrEmpty(path) || !path.StartsWith("/"))
                {
                    throw new ArgumentException(
                        "The value for customized path can't be null and needs to start with '/' character.", argument);
                }
            };

            ensureValidPath(options.ApiPath, nameof(Options.ApiPath));
        }
    }
}
