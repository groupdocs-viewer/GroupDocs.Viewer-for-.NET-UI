using GroupDocs.Viewer.UI.Core.Configuration;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.RenderingMode = RenderingMode.Html;
        config.StaticContentMode = true;
    });

var app = builder.Build();

// Configure static files server middleware 
var staticFilesPath = Path.Combine(Directory.GetCurrentDirectory(), "Content");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(staticFilesPath)
});

// Map GroupDocs.Viewer UI
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
            options.ApiEndpoint = "/";
        });
    });

await app.RunAsync();