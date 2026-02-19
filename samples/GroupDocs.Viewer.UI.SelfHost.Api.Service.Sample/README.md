# SelfHost API Service Sample (Split Architecture — API Service)

This sample demonstrates a split-architecture deployment where the viewer UI and the rendering API run as separate applications. This is the **API service** that handles document rendering requests.

Use this together with the [SelfHost.Api.App.Sample](../GroupDocs.Viewer.UI.SelfHost.Api.App.Sample/) which provides the viewer UI.

## What This Sample Shows

- Running a standalone rendering API without the viewer UI
- Configuring CORS to allow cross-origin requests from the UI app
- Setting an explicit `ApiDomain` for absolute URL generation
- Using the Options pattern to keep `PreloadPages` in sync with the UI app

## Prerequisites

- .NET 8.0 SDK or later

## Running

1. **Start this API service:**
   ```bash
   cd samples/GroupDocs.Viewer.UI.SelfHost.Api.Service.Sample
   dotnet run
   ```

2. **Then start the UI app:**
   ```bash
   cd samples/GroupDocs.Viewer.UI.SelfHost.Api.App.Sample
   dotnet run
   ```

The API service runs at `https://localhost:5001` and serves the API at `/document-viewer-api`.

## Configuration

```csharp
// CORS is required for cross-origin UI-to-API communication
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Keep PreloadPages in sync with the UI app
builder.Services
    .AddOptions<Config>()
    .Configure<IConfiguration>((config, configuration) =>
    {
        config.PreloadPages = 10;
    });

endpoints.MapGroupDocsViewerApi(options =>
{
    options.ApiPath = "/document-viewer-api";
    options.ApiDomain = "https://localhost:5001";  // Explicit domain for absolute URLs
});
```

Note that this app does **not** call `MapGroupDocsViewerUI()` — the UI is hosted separately.

## Adding Documents

Place document files in the `./Files` directory. The UI app will list and render them via this API service.
