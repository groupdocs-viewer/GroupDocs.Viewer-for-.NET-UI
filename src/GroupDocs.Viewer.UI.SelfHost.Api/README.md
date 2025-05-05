# GroupDocs.Viewer.UI.SelfHost.Api

`GroupDocs.Viewer.UI.SelfHost.Api` is a self-hosted API implementation for `GroupDocs.Viewer.UI` that enables document viewing using GroupDocs.Viewer for .NET. It provides a local, self-contained solution for document viewing without requiring cloud services.

This package uses the `GroupDocs.Viewer` NuGet package which relies on System.Drawing.Common and is optimized for Windows environments. For Linux deployments, we recommend using `GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform` instead, which utilizes `GroupDocs.Viewer.CrossPlatform` with its cross-platform graphical engine.

## Installation

To use GroupDocs.Viewer.UI.SelfHost.Api in your ASP.NET Core project:

1. Add the required package to your project:

```bash
dotnet add package GroupDocs.Viewer.UI.SelfHost.Api
```

2. Configure the SelfHost API in your `Startup` class:

```cs
using GroupDocs.Viewer.UI.Core;

var builder = WebApplication.CreateBuilder(args);

var viewerType = ViewerType.HtmlWithEmbeddedResources;

builder.Services
    .AddGroupDocsViewerUI();

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config =>
    {
        config.SetViewerType(viewerType);
        config.SetLicensePath("GroupDocs.Viewer.lic"); // Path to the license file

        // Configure view options
        config.ConfigureHtmlViewOptions(options => {
            // Configure HTML view options
        });

        config.ConfigurePngViewOptions(options => {
            // Configure PNG view options
        });

        config.ConfigureJpgViewOptions(options => {
            // Configure JPG view options
        });

        config.ConfigurePdfViewOptions(options => {
            // Configure PDF view options
        });

        // Configure internal caching
        config.ConfigureInternalCaching(options => {
            // Configure caching options
        });
    })
    .AddLocalStorage("./Files");

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

The SelfHost API implementation provides the following configuration options:

- `LicensePath` (Optional): Path to your GroupDocs.Viewer license file.
- `ViewerType` (Optional): The type of viewer to use. Options include:
  - `HtmlWithEmbeddedResources` (default)
  - `HtmlWithExternalResources`
  - `Png`
  - `Jpg`

### View Options Configuration
You can configure specific view options for different output formats:

```cs
config.ConfigureHtmlViewOptions(options => {
    // Configure HTML view options
});

config.ConfigurePngViewOptions(options => {
    // Configure PNG view options
});

config.ConfigureJpgViewOptions(options => {
    // Configure JPG view options
});

config.ConfigurePdfViewOptions(options => {
    // Configure PDF view options
});
```

### Internal Caching

The internal caching mechanism stores rendered pages and document information in memory to improve performance:

- Uses memory cache to store rendered pages and document info
- Caches are keyed by document path and rendering parameters  
- Default cache duration is 5 minutes (configurable)
- Automatically evicts old entries when memory is constrained
- Significantly improves performance for frequently accessed documents


The cache can be configured via the options shown in the example:

```cs
// Configure internal caching
config.ConfigureInternalCaching(options => {
    // Configure caching options
    options.DisableInternalCache(); // Disables internal caching
    options.SetCacheEntryExpirationTimeoutMinutes(60); // Default values is 5 minutes
});
```

## Performance Considerations

When using GroupDocs.Viewer.UI.SelfHost.Api:

1. Processing is done locally on your server
2. Memory usage depends on document size and complexity
3. Internal caching can improve performance for repeated requests
4. Best suited for:
   - Applications requiring local document processing
   - Environments with limited internet connectivity
   - Scenarios where data privacy is important
5. Performance depends on:
   - Server hardware specifications
   - Document size and complexity
   - Chosen viewer type
   - Caching configuration

## License

This project is licensed under the MIT License - see the [LICENSE.txt](../../LICENSE.txt) file for details. 