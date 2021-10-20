using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Service.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader();
                    });
            });

            services
                .AddControllers()
                .AddGroupDocsViewerSelfHostApi(config =>
                {
                    //Trial limitations https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/
                    //Temporary license can be requested at https://purchase.groupdocs.com/temporary-license
                    //config.SetLicensePath("c:\\licenses\\GroupDocs.Viewer.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'
                })
                .AddLocalStorage("./Files")
                .AddLocalCache("./Cache");
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors();

            app
                .UseRouting()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapGet("/", async context =>
                    {
                        await context.Response.WriteAsync("Viewer API can be accessed at '/viewer' endpoint.");
                    });

                    endpoints.MapGroupDocsViewerApi(options =>
                    {
                        options.ApiPath = "/viewer";
                    });
                });
        }
    }
}
