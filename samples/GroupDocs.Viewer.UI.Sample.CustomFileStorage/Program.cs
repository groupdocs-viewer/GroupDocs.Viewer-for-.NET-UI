using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;

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
        //config.SetLicensePath("GroupDocs.Viewer.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'
    })
    .AddLocalCache("./Cache");

//NOTE: registered after AddGroupDocsViewerSelfHostApi() so it overrides any default IFileStorage
builder.Services.AddSingleton<IFileStorage>(_ => new DatabaseFileStorage("./Files"));

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

await app.RunAsync();

/// <summary>
/// IFileStorage implementation that simulates a database-backed BLOB store.
/// In a real app, the dictionary would be replaced by a SQL VARBINARY(MAX)
/// column (or similar). Here it is seeded from a folder on disk at startup so
/// the sample is runnable without any database setup.
///
/// Starting with GroupDocs.Viewer.UI 26.3.0, the SelfHost viewer calls
/// <see cref="ReadFileStreamAsync"/> instead of <see cref="ReadFileAsync"/>.
/// There is no setting to revert this — any custom IFileStorage MUST implement
/// ReadFileStreamAsync.
/// </summary>
class DatabaseFileStorage : IFileStorage
{
    private readonly Dictionary<string, byte[]> _files = new(StringComparer.OrdinalIgnoreCase);

    public DatabaseFileStorage(string seedDirectory)
    {
        if (!Directory.Exists(seedDirectory))
            return;

        foreach (var path in Directory.GetFiles(seedDirectory))
            _files[Path.GetFileName(path)] = File.ReadAllBytes(path);
    }

    public Task<IEnumerable<FileSystemEntry>> ListDirsAndFilesAsync(
        string dirPath, CancellationToken cancellationToken = default)
    {
        var entries = _files.Select(kvp =>
            FileSystemEntry.File(kvp.Key, kvp.Key, kvp.Value.Length));

        return Task.FromResult(entries);
    }

    public Task<byte[]> ReadFileAsync(
        string filePath, CancellationToken cancellationToken = default)
    {
        if (!_files.TryGetValue(filePath, out var bytes))
            throw new FileNotFoundException($"'{filePath}' not found in the database.", filePath);

        return Task.FromResult(bytes);
    }

    /// <summary>
    /// Called by the viewer in 26.3.0+. The caller disposes the returned stream
    /// (see IFileStorage.cs in the Core project).
    ///
    /// For an in-memory or byte[]-based store, MemoryStream is the idiomatic
    /// implementation. For a true SQL Server BLOB column you would use ADO.NET's
    /// SqlDataReader.GetStream(...) with CommandBehavior.SequentialAccess so the
    /// bytes are streamed instead of fully buffered.
    /// </summary>
    public Task<Stream> ReadFileStreamAsync(
        string filePath, CancellationToken cancellationToken = default)
    {
        if (!_files.TryGetValue(filePath, out var bytes))
            throw new FileNotFoundException($"'{filePath}' not found in the database.", filePath);

        Stream stream = new MemoryStream(bytes, writable: false);
        return Task.FromResult(stream);
    }

    public Task<string> WriteFileAsync(
        string fileName, byte[] bytes, bool rewrite, CancellationToken cancellationToken = default)
    {
        var targetName = fileName;

        if (!rewrite)
        {
            var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            var i = 1;
            while (_files.ContainsKey(targetName))
                targetName = $"{nameWithoutExt} ({i++}){ext}";
        }

        _files[targetName] = bytes;
        return Task.FromResult(targetName);
    }
}
