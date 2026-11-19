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
            // GroupDocs.Viewer.UI.SelfHost.Api.Service.Sample serves the API at this URL, see its Properties/launchSettings.json
            options.ApiEndpoint = "https://localhost:5003/document-viewer-api";
        });
    });

await app.RunAsync();