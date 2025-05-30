﻿using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Configuration;

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

ViewerType viewerType = ViewerType.HtmlWithEmbeddedResources;

// This how you can keep UI config in sync with API
builder.Services
    .AddOptions<Config>()
    .Configure<IConfiguration>((config, configuration) =>
    {
        config.PreloadPages = 10;
    });

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config =>
    {
        config.SetViewerType(viewerType);

        //Trial limitations https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/
        //Temporary license can be requested at https://purchase.groupdocs.com/temporary-license
        //config.SetLicensePath("c://Licenses//GroupDocs.Viewer.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'
    })
    .AddLocalStorage("./Files")
    .AddLocalCache("./Cache");

var app = builder.Build();

app.UseCors();

app
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("Viewer API can be accessed at '/document-viewer-api' endpoint.");
        });

        endpoints.MapGroupDocsViewerApi(options =>
        {
            options.ApiPath = "/document-viewer-api";
            options.ApiDomain = "https://localhost:5001";
        });
    });

app.Run();