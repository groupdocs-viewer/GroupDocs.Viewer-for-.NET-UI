## GroupDocs.Viewer.UI.Api.Distributed.Cache

IDistributedCache-based file cache provider for GroupDocs.Viewer.UI. Supports Redis, SQL Server, NCache, and any other IDistributedCache implementation.

### Usage

```csharp
builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddLocalStorage("./Files")
    .AddDistributedCache(options =>
    {
        options.SlidingExpiration = TimeSpan.FromMinutes(30);
    });

// Register an IDistributedCache implementation, e.g. Redis:
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});
```
