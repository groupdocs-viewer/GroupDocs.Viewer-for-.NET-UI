using GroupDocs.Viewer.UI.Core;

var builder = WebApplication.CreateBuilder(args);

var viewerType = ViewerType.Jpg;

builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.RenderingMode = viewerType.ToRenderingMode();
        config.PreloadPages = 3; // Number of pages to create on first request
    });

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config =>
    {
        config.SetViewerType(viewerType);

        //Trial limitations https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/
        //Temporary license can be requested at https://purchase.groupdocs.com/temporary-license
        //config.SetLicensePath("GroupDocs.Viewer.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'
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
            await context.Response.SendFileAsync("index.html");
        });
        endpoints.MapGroupDocsViewerUI(options =>
        {
            options.UIPath = "/viewer";
            options.ApiEndpoint = "/viewer-api";
        });
        endpoints.MapGroupDocsViewerApi(options =>
        {
            options.ApiPath = "/viewer-api";
            // By default, UseAbsoluteUrls is false, which generates relative URLs.
            // This ensures URLs respect the browser's domain, which is critical for Azure App Service with custom domains.
            // 
            // To simulate the "before fix" scenario, uncomment the following lines:
            // options.UseAbsoluteUrls = true;
            // options.ApiDomain = "https://app-name.azurewebsites.net";
        });
    });

await app.RunAsync();
