# GroupDocs.Viewer.UI.Api.InMemory.Cache

`GroupDocs.Viewer.UI.Api.InMemory.Cache` is an in-memory caching implementation that can be used with `GroupDocs.Viewer.UI.Api`. It provides fast, in-memory caching for document rendering results in your `GroupDocs.Viewer.UI` application.

## Installation

To use InMemory Cache in your ASP.NET Core project:

1. Add the required package to your project:

```bash
dotnet add package GroupDocs.Viewer.UI.Api.InMemory.Cache
```

2. Configure InMemory Cache in your `Startup` class:

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGroupDocsViewerUI();

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddLocalStorage("./Files")
    .AddInMemoryCache(config =>
    {
        config.SetCacheEntryExpirationTimeoutMinutes(60); // Cache entries expire after 60 minutes
        config.SetGroupCacheEntriesByFile(true); // Group cache entries by file
    });

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

The InMemory Cache implementation supports the following configuration options:

- `CacheEntryExpirationTimeoutMinutes` (Optional): The expiration timeout of each cache entry in minutes. Default is 0, which means cache entries never expire.
- `GroupCacheEntriesByFile` (Optional): When enabled, eviction of any cache entry leads to eviction of all cache entries for that file. This setting only takes effect when `CacheEntryExpirationTimeoutMinutes` is greater than zero.

You can also configure these settings through `appsettings.json`:

```json
{
  "GroupDocsViewerUIApiInMemoryCache": {
    "CacheEntryExpirationTimeoutMinutes": 60,
    "GroupCacheEntriesByFile": true
  }
}
```

## Technical Implementation

The InMemory Cache implementation uses ASP.NET Core's built-in `IMemoryCache` for storing cached data. Here's how it works:

### Cache Key Structure
- Each cache entry is identified by a composite key: `{filePath}_{cacheKey}`
- The `filePath` represents the document being cached
- The `cacheKey` represents the specific cached item (e.g., page thumbnail, page content)

### Cache Entry Management
- Cache entries are stored in the application's process memory
- Uses `MemoryCacheEntryOptions` for controlling entry behavior
- Supports both synchronous and asynchronous operations through `IFileCache` interface

### Expiration Mechanism
When `CacheEntryExpirationTimeoutMinutes` is set:
1. A `CancellationTokenSource` is created for each cache entry
2. The token source is configured to cancel after the specified timeout
3. Cache entries are linked to their token source through `CancellationChangeToken`
4. When the token is cancelled, the cache entry is automatically evicted

### File-based Grouping
When `GroupCacheEntriesByFile` is enabled:
1. A single `CancellationTokenSource` is shared among all cache entries for a file
2. The token source key is `{filePath}__CTS`
3. When any cache entry expires, all entries for that file are evicted together
4. This ensures consistency when a file's cache entries need to be invalidated

### Memory Management
- Uses ASP.NET Core's built-in memory management
- Automatically handles memory pressure through the framework's eviction policies
- No explicit memory limits are set, relying on the system's available memory
- Cache entries are not persisted between application restarts

## Performance Considerations

When using InMemory Cache:

1. Cache size is limited by available system memory
2. Cache is cleared when the application restarts
3. Best suited for:
   - Small to medium-sized document sets
   - Applications with moderate concurrent users
   - Development and testing environments
4. Not recommended for:
   - Large document sets
   - High-traffic production environments
   - Distributed applications

## License

This project is licensed under the MIT License - see the [LICENSE.txt](../../LICENSE.txt) file for details. 