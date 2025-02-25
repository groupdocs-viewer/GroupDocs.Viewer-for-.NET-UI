﻿using GroupDocs.Viewer.UI.Core;

var builder = WebApplication.CreateBuilder(args);

ViewerType viewerType = ViewerType.HtmlWithEmbeddedResources;

builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.RenderingMode = viewerType.ToRenderingMode();
    });

// Get your Client ID and Client Secret at https://dashboard.groupdocs.cloud/applications
var clientId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
var clientSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

builder.Services
    .AddControllers()
    .AddGroupDocsViewerCloudApi(config =>
        config
            .SetClientId(clientId)
            .SetClientSecret(clientSecret)
            .SetViewerType(viewerType)
    )
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
        });
    });

app.Run();