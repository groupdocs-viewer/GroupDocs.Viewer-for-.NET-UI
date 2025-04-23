using System;
using GroupDocs.Viewer.UI.Api;
using GroupDocs.Viewer.UI.Api.Configuration;
using GroupDocs.Viewer.UI.Api.Extensions;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapGroupDocsViewerApi(this IEndpointRouteBuilder builder,
            Action<Options> setupOptions = null)
        {
            var options = new Options();
            setupOptions?.Invoke(options);

            var service = builder.ServiceProvider.GetService(typeof(IOptionsProvider));
            if (service != null)
            {
                var optionsService = (IOptionsProvider)service;
                {
                    optionsService.SetOptions(options);
                }
            }

            EnsureValidApiOptions(options);

            MapControllerRoutes(builder, options);

            return new GroupDocsViewerApiConventionBuilder(Array.Empty<IEndpointConventionBuilder>());
        }

        private static void MapControllerRoutes(IEndpointRouteBuilder builder, Options options)
        {
            var relativeApiPath = options.ApiPath.AsRelativeResource();

            var apiMethods = new []
            {
                ApiNames.API_METHOD_LIST_DIR,
                ApiNames.API_METHOD_UPLOAD_FILE,
                ApiNames.API_METHOD_VIEW_DATA,
                ApiNames.API_METHOD_CREATE_PAGES,
                ApiNames.API_METHOD_CREATE_PDF,
                ApiNames.API_METHOD_GET_PAGE,
                ApiNames.API_METHOD_GET_THUMB,
                ApiNames.API_METHOD_GET_PDF,
                ApiNames.API_METHOD_GET_RESOURCE,
            };

            foreach (var apiMethod in apiMethods)
            {
                builder.MapControllerRoute(
                    name: apiMethod, $"{relativeApiPath}/{apiMethod}",
                    new { controller = ApiNames.CONTROLLER_NAME, action = apiMethod.ToActionName() });
            }
        }

        private static void EnsureValidApiOptions(Options options)
        {
            Action<string, string> ensureValidPath = (string path, string argument) =>
            {
                if (string.IsNullOrEmpty(path) || !path.StartsWith("/"))
                {
                    throw new ArgumentException(
                        "The value for customized path can't be null and need to start with / character.", argument);
                }
            };

            ensureValidPath(options.ApiPath, nameof(Options.ApiPath));
        }
    }
}