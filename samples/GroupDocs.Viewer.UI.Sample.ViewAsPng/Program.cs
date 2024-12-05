using GroupDocs.Viewer.UI.Core;

var builder = WebApplication.CreateBuilder(args);

ViewerType viewerType = ViewerType.Png;

builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        //Preload first 3 pages
        config.PreloadPages = 3;
        config.RenderingMode = viewerType.ToRenderingMode();
    });

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config =>
    {
        config.SetViewerType(viewerType);

        //Trial limitations https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/
        //Temporary license can be requested at https://purchase.groupdocs.com/temporary-license
        //config.SetLicensePath("c:\\licenses\\GroupDocs.Viewer.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'
    })
    .AddLocalStorage("./Files")
    .AddLocalCache("./Cache");

var app = builder.Build();

app
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("Viewer UI can be accessed at '/viewer' endpoint.");
        });
        endpoints.MapGroupDocsViewerUI(options =>
        {
            options.UIPath = "/viewer";
            options.ApiEndpoint = "/viewer-api";
        });
        endpoints.MapGroupDocsViewerApi(options =>
        {
            options.ApiPath = "/viewer-api";
        });
    });

app.Run();