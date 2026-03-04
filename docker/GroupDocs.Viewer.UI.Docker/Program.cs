using GroupDocs.Viewer.UI.Core;

var builder = WebApplication.CreateBuilder(args);

var viewerTypeName = Environment.GetEnvironmentVariable("VIEWER_TYPE") ?? "HtmlWithEmbeddedResources";
var viewerType = Enum.Parse<ViewerType>(viewerTypeName, ignoreCase: true);

var storagePath = Environment.GetEnvironmentVariable("VIEWER_STORAGE_PATH") ?? "/app/Files";
var cachePath = Environment.GetEnvironmentVariable("VIEWER_CACHE_PATH") ?? "/app/Cache";
var uiPath = Environment.GetEnvironmentVariable("VIEWER_UI_PATH") ?? "/";
var apiPath = Environment.GetEnvironmentVariable("VIEWER_API_PATH") ?? "/viewer-api";
var preloadPages = int.TryParse(Environment.GetEnvironmentVariable("VIEWER_PRELOAD_PAGES"), out var pp) ? pp : 3;

builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.RenderingMode = viewerType.ToRenderingMode();
        config.PreloadPages = preloadPages;
    });

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config =>
    {
        config.SetViewerType(viewerType);

        // Set GROUPDOCS_LIC_PATH environment variable or call config.SetLicensePath("path")
        // See https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/
    })
    .AddLocalStorage(storagePath)
    .AddLocalCache(cachePath);

builder.Services.AddHealthChecks();

var app = builder.Build();

app
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapHealthChecks("/health");
        endpoints.MapGroupDocsViewerUI(options =>
        {
            options.UIPath = uiPath;
            options.ApiEndpoint = apiPath;
        });
        endpoints.MapGroupDocsViewerApi(options =>
        {
            options.ApiPath = apiPath;
        });
    });

await app.RunAsync();
