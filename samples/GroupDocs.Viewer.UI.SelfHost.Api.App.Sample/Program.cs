using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

var viewerType = ViewerType.HtmlWithEmbeddedResources;

builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.RenderingMode = viewerType.ToRenderingMode();

        config.PreloadPages = 10; // Keep this in sync, see Program.cs in GroupDocs.Viewer.UI.SelfHost.Api.Service.Sample project
        config.DefaultLanguage = LanguageCode.English;
        config.SupportedLanguages = new[] { LanguageCode.English, LanguageCode.French, LanguageCode.Italian };
    });

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
            options.UIPath = "/document-viewer";
            options.ApiEndpoint = "https://localhost:5001/document-viewer-api";
        });
    });

await app.RunAsync();