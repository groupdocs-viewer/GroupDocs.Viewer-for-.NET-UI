# GroupDocs.Viewer.UI.Api.Local.Cache

`GroupDocs.Viewer.UI.Api.Local.Cache` is a local file system caching implementation that can be used with `GroupDocs.Viewer.UI.Api`. It provides persistent caching for document rendering results in your `GroupDocs.Viewer.UI` application.

## Installation

To use Local Cache in your ASP.NET Core project:

1. Add the required package to your project:

```bash
dotnet add package GroupDocs.Viewer.UI.Api.Local.Cache
```

2. Configure Local Cache in your `Startup` class:

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGroupDocsViewerUI();

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddLocalStorage("./Files")
    .AddLocalCache("./Cache"); // Path to your cache directory

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

The Local Cache implementation requires a single configuration option:

- `cachePath` (Required): The path to the directory where cache files will be stored. This can be:
  - An absolute path (e.g., `"C:/Cache"`)
  - A relative path (e.g., `"./Cache"`)
  - A path relative to the application's root directory

## Technical Implementation

The Local Cache implementation stores cached data in the local file system. Here's how it works:

### Cache Structure
- Cache entries are stored in a hierarchical directory structure
- Each file's cache entries are stored in a separate subdirectory
- Subdirectory names are derived from the file path (with invalid characters replaced)
- Cache entries are stored as individual files within these subdirectories

### Data Serialization
- Uses `System.Text.Json` for JSON serialization
- Automatically detects the data type and uses appropriate serialization method

### File Operations
- Implements file locking mechanism for thread safety
- Uses retry logic with configurable timeout for file operations
- Default wait timeout is 100ms for file operations
- Handles concurrent access through file sharing modes

### Cache Key Management
- Cache keys are used as filenames within the cache directory
- File paths are sanitized to create valid directory names
- Maintains a clean directory structure for easy management

## Features

- Seamless integration with GroupDocs.Viewer UI
- Persistent caching across application restarts
- Thread-safe file operations
- Automatic retry mechanism for file access
- Support for different data types (bytes, streams, objects)
- Efficient file system organization
- No external dependencies required

## Performance Considerations

When using Local Cache:

1. Cache size is limited by available disk space
2. Cache persists between application restarts
3. Best suited for:
   - Applications requiring persistent caching
   - Environments with sufficient disk space
   - Scenarios where memory caching is not suitable
4. Performance depends on:
   - Disk I/O speed
   - File system performance
   - Available disk space

## Security Considerations

When using Local Cache:

1. Ensure the cache directory is not accessible directly through the web server
2. Set appropriate file system permissions
3. Consider implementing additional security measures if caching sensitive data
4. Regularly monitor cache directory size
5. Implement cache cleanup strategy if needed

## License

This project is licensed under the MIT License - see the [LICENSE.txt](../../LICENSE.txt) file for details. 