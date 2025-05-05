# GroupDocs.Viewer.UI.Api.Local.Storage

`GroupDocs.Viewer.UI.Api.Local.Storage` is a local file system storage implementation that can be used with `GroupDocs.Viewer.UI.Api`. It provides seamless integration with the local file system for storing and retrieving documents in your `GroupDocs.Viewer.UI` application.

## Installation

To use Local Storage in your ASP.NET Core project:

1. Add the required package to your project:

```bash
dotnet add package GroupDocs.Viewer.UI.Api.Local.Storage
```

2. Configure Local Storage in your `Startup` class:

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGroupDocsViewerUI();

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddLocalStorage("./Files"); // Path to your files directory

var app = builder.Build();

app
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
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
```

## Configuration Options

The Local Storage implementation requires a single configuration option:

- `storagePath` (Required): The path to the directory where your files will be stored. This can be:
  - An absolute path (e.g., `"C:/Documents/Files"`)
  - A relative path (e.g., `"./Files"`)
  - A path relative to the application's root directory

## Security Considerations

When using Local Storage:

1. Ensure the storage directory is not accessible directly through the web server
2. Set appropriate file system permissions
3. Consider implementing additional security measures if storing sensitive documents

## License

This project is licensed under the MIT License - see the [LICENSE.txt](../../LICENSE.txt) file for details. 