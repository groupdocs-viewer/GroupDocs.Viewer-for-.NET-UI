# Custom Cache Provider Sample

This sample demonstrates how to implement and register a custom `IFileCache` provider, replacing the built-in local or in-memory cache with your own caching logic.

## What This Sample Shows

- Implementing the `IFileCache` interface with a custom `ConcurrentDictionaryFileCache`
- Registering the custom cache provider after `AddGroupDocsViewerSelfHostApi()` to override the default
- In-memory caching using `ConcurrentDictionary` as the backing store

## Prerequisites

- .NET 6.0 SDK or later

## Running

```bash
cd samples/GroupDocs.Viewer.UI.Sample.CustomCacheProvider
dotnet run
```

Open your browser at `https://localhost:5001` (or the URL shown in the console output).

## Configuration

The custom cache is registered as a singleton after the self-host API setup:

```csharp
builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config => { ... })
    .AddLocalStorage("./Files");

// Register custom cache AFTER AddGroupDocsViewerSelfHostApi()
builder.Services.AddSingleton<IFileCache, ConcurrentDictionaryFileCache>();
```

**Important:** The custom `IFileCache` registration must come after `AddGroupDocsViewerSelfHostApi()` to override the default cache implementation.

## Implementing IFileCache

The `IFileCache` interface requires four methods:

```csharp
public interface IFileCache
{
    TEntry TryGetValue<TEntry>(string cacheKey, string filePath);
    Task<TEntry> TryGetValueAsync<TEntry>(string cacheKey, string filePath);
    void Set<TEntry>(string cacheKey, string filePath, TEntry entry);
    Task SetAsync<TEntry>(string cacheKey, string filePath, TEntry entry);
}
```

This pattern can be adapted to use Redis, database, or any other caching backend.
