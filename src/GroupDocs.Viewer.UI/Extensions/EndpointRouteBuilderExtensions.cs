using GroupDocs.Viewer.UI;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Middleware;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
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

            var settingsDelegate = builder.CreateApplicationBuilder()
                .UseMiddleware<UISettingsMiddleware>()
                .Build();

            var embeddedResourcesAssembly = typeof(UIResource).Assembly;

            var resourcesEndpoints =
                new UIEndpointsResourceMapper(new UIEmbeddedResourcesReader(embeddedResourcesAssembly))
                    .Map(builder, options);

            var settingsEndpoint =
                builder.Map(options.UIConfigEndpoint, settingsDelegate);

            var endpointConventionBuilders =
                new List<IEndpointConventionBuilder>(
                    new[] { settingsEndpoint }.Union(resourcesEndpoints));

            return new GroupDocsViewerUIConventionBuilder(endpointConventionBuilders);
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
                    throw new ArgumentException(
                        "The value can't be null or empty.", argument);
                }
            };


            ensureValidPath(options.UIPath, nameof(Options.UIPath));
            ensureNotEmpty(options.APIEndpoint, nameof(Options.APIEndpoint));
        }
    }
}