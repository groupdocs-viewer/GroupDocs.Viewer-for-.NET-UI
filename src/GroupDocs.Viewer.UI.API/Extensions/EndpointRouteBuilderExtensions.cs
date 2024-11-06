using GroupDocs.Viewer.UI.Api;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using Options = GroupDocs.Viewer.UI.Api.Configuration.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapGroupDocsViewerApi(this IEndpointRouteBuilder builder,
            Action<Options> setupOptions = null)
        {
            var optionsMonitor = builder.ServiceProvider.GetRequiredService<IOptionsMonitor<Options>>();

            // Use the provided setupOptions or the current value from optionsMonitor
            Options options = new Options();

            if (setupOptions != null)
            {
                setupOptions(options);
            }
            else
            {
                options = optionsMonitor.CurrentValue;
            }

            EnsureValidApiOptions(options);
            MapControllerRoutes(builder, options);

            return new GroupDocsViewerApiConventionBuilder(Array.Empty<IEndpointConventionBuilder>());
        }

        private static void MapControllerRoutes(IEndpointRouteBuilder builder, Options options)
        {
            var relativeApiPath = options.ApiPath.AsRelativeResource();

            var actions = new[]
            {
                Constants.LOAD_CONFIG_ACTION_NAME,
                Constants.LOAD_FILE_TREE_ACTION_NAME,
                Constants.DOWNLOAD_DOCUMENT_ACTION_NAME,
                Constants.UPLOAD_DOCUMENT_ACTION_NAME,
                Constants.LOAD_DOCUMENT_DESCRIPTION_ACTION_NAME,
                Constants.LOAD_DOCUMENT_PAGES_ACTION_NAME,
                Constants.LOAD_DOCUMENT_PAGE_ACTION_NAME,
                Constants.LOAD_DOCUMENT_PAGE_RESOURCE_ACTION_NAME,
                Constants.LOAD_THUMBNAILS_ACTION_NAME,
                Constants.PRINT_PDF_ACTION_NAME,
                Constants.CREATE_PDF_ACTION_NAME,
            };

            foreach (var action in actions)
            {
                builder.MapControllerRoute(
                    name: action, $"{relativeApiPath}/{action}",
                    new { controller = Constants.CONTROLLER_NAME, action = action });
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