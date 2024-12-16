using System.Collections.Concurrent;
using GroupDocs.Viewer.UI.Core;

var builder = WebApplication.CreateBuilder(args);

var viewerType = ViewerType.HtmlWithEmbeddedResources;

builder.Services
    .AddGroupDocsViewerUI(config =>
    {
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
    .AddLocalStorage("./Files");

//NOTE: registered after AddGroupDocsViewerSelfHostApi()
builder.Services.AddSingleton<IFileCache, ConcurrentDictionaryFileCache>();

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

class ConcurrentDictionaryFileCache : IFileCache
{
    private readonly ConcurrentDictionary<string, object> _cache =
        new ConcurrentDictionary<string, object>();

    public TEntry TryGetValue<TEntry>(string cacheKey, string filePath)
    {
        string key = $"{filePath}_{cacheKey}";
        if (_cache.TryGetValue(key, out object? obj))
            return (TEntry)obj;

        return default!;
    }

    public Task<TEntry> TryGetValueAsync<TEntry>(string cacheKey, string filePath)
    {
        TEntry entry = TryGetValue<TEntry>(cacheKey, filePath);
        return Task.FromResult(entry);
    }

    public void Set<TEntry>(string cacheKey, string filePath, TEntry entry)
    {
        if(entry is null)
            throw new ArgumentNullException(nameof(entry));

        string key = $"{filePath}_{cacheKey}";
        _cache.TryAdd(key, entry);
    }

    public Task SetAsync<TEntry>(string cacheKey, string filePath, TEntry entry)
    {
        Set(cacheKey, filePath, entry);
        return Task.CompletedTask;
    }
}
