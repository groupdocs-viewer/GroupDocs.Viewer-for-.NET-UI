using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GroupDocs.Viewer.UI.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddGroupDocsViewerUI();

            services
                .AddControllers()
                .AddGroupDocsViewerSelfHostApi(config =>
                {
                    //Trial limitations https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/
                    //Temporary license can be requested at https://purchase.groupdocs.com/temporary-license
                    //config.SetLicensePath("c:\\licenses\\GroupDocs.Viewer.lic");
                })
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
