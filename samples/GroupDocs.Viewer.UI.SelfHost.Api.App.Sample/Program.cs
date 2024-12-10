using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Configuration;

var builder = WebApplication.CreateBuilder(args);

var viewerType = ViewerType.HtmlWithEmbeddedResources;

builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.RenderingMode = viewerType.ToRenderingMode();

        config.PreloadPages = 3; // Number of pages to create on first request
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
            options.UIPath = "/viewer";
            options.ApiEndpoint = "https://localhost:5001/viewer-api";
        });
    });

await app.RunAsync();