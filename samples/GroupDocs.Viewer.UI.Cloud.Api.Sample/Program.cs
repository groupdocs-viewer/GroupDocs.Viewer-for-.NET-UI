using GroupDocs.Viewer.UI.Core;

var builder = WebApplication.CreateBuilder(args);

ViewerType viewerType = ViewerType.HtmlWithEmbeddedResources;

builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.RenderingMode = viewerType.ToRenderingMode();
    });

// Get your Client ID and Client Secret at https://dashboard.groupdocs.cloud/applications
var clientId = "f9d1e22a-1d7a-45c9-a269-a23b3a4297b7";
var clientSecret = "1c28c19ae53ac040d21b539e6a373638";

builder.Services
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