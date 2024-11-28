using System;
using GroupDocs.Viewer.UI;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Configuration;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Options = GroupDocs.Viewer.UI.Configuration.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointConventionBuilder MapGroupDocsViewerUI(this IEndpointRouteBuilder builder,
            Action<Options> setupOptions = null)
        {
            var options = new Options();
            setupOptions?.Invoke(options);

            EnsureValidApiOptions(options);

            var configOptions = builder.ServiceProvider.GetRequiredService<IOptions<Config>>();
            var config = configOptions.Value;

            var embeddedResourcesAssembly = typeof(UIResource).Assembly;

            var resourcesEndpoints =
                new UIEndpointsResourceMapper(new UIEmbeddedResourcesReader(embeddedResourcesAssembly))
                    .Map(builder, options, config);

            return new GroupDocsViewerUIConventionBuilder(resourcesEndpoints);
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

            Action<string, string> ensureNotEmpty = (string endpoint, string argument) =>
            {
                if (string.IsNullOrEmpty(endpoint))
                {
                    throw new ArgumentException("The value can't be null or empty.", argument);
                }
            };


            ensureValidPath(options.UIPath, nameof(Options.UIPath));
            ensureNotEmpty(options.ApiEndpoint, nameof(Options.ApiEndpoint));
        }
    }
}