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
            services
                .AddGroupDocsViewerUI(config => 
                    config.SetViewerType(ViewerType.Png));

            services
                .AddControllers()
                .AddGroupDocsViewerCloudApi(config => 
                    config
                        .SetClientId("84a73795-4d06-4912-99b4-d0d7ea060181")
                        .SetClientSecret("73170b909ffe114a5f163e5d0e78d84e")
                    )
                .AddLocalStorage("./Files")
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
