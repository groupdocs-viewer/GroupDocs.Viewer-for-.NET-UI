using GroupDocs.Viewer.UI.Api.Configuration;
using GroupDocs.Viewer.UI.Api.Extensions;
using GroupDocs.Viewer.UI.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policyBuilder =>
        {
            policyBuilder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});
builder.Services
    .AddViewerConfiguration(builder.Configuration)
    .AddGroupDocsViewerUI(config =>
    {
        //Preload first three pages
        config.SetPreloadPageCount(3);
    });

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config =>
    {
        //Trial limitations https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/
        //Temporary license can be requested at https://purchase.groupdocs.com/temporary-license
        config.SetLicensePath("c:\\licenses\\GroupDocs.Viewer.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'
    })
    .AddLocalStorage("./Files")
    .AddLocalCache("./Cache");
builder.Services.Configure<Options>(builder.Configuration.GetSection("Options"));

var app = builder.Build();
app.UseCors();
// Use the embedded resource middleware to serve static files
app.UseMiddleware<EmbeddedResourceMiddleware>("viewer");
app
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapGroupDocsViewerApi();
    });

app.Run();