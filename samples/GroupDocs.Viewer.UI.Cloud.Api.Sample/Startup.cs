using GroupDocs.Viewer.UI.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GroupDocs.Viewer.UI.Cloud.Api.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            ViewerType viewerType = ViewerType.HtmlWithEmbeddedResources;

            services
                .AddGroupDocsViewerUI(config => 
                    config.SetViewerType(viewerType));

            // Get your Client ID and Client Secret at https://dashboard.groupdocs.cloud/applications
            var clientId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
            var clientSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

            services
                .AddControllers()
                .AddGroupDocsViewerCloudApi(config => 
                    config
                        .SetClientId(clientId)
                        .SetClientSecret(clientSecret)
                        .SetViewerType(viewerType)
                )
                .AddCloudStorage(config => 
                    config
                        .SetClientId(clientId)
                        .SetClientSecret(clientSecret)
                )
                .AddLocalCache("./Cache");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapGroupDocsViewerUI(options =>
                    {
                        options.UIPath = "/viewer";
                        options.APIEndpoint = "/viewer-api";
                    });

                    endpoints.MapGroupDocsViewerApi(options =>
                    {
                        options.ApiPath = "/viewer-api";
                    });
                });
        }
    }
}
