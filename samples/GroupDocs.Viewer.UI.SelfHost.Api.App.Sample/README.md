# SelfHost API App Sample (Split Architecture — UI App)

This sample demonstrates a split-architecture deployment where the viewer UI and the rendering API run as separate applications. This is the **UI application** that serves the Angular viewer and connects to a remote API service.

Use this together with the [SelfHost.Api.Service.Sample](../GroupDocs.Viewer.UI.SelfHost.Api.Service.Sample/) which provides the rendering API.

## What This Sample Shows

- Running the viewer UI without a local rendering API
- Pointing the UI to a remote API endpoint (`https://localhost:5001/document-viewer-api`)
- Configuring language support (English, French, Italian)
- Keeping `PreloadPages` in sync between UI and API apps

## Prerequisites

- .NET 8.0 SDK or later
- The [SelfHost.Api.Service.Sample](../GroupDocs.Viewer.UI.SelfHost.Api.Service.Sample/) running on `https://localhost:5001`

## Running

1. **Start the API service first:**
   ```bash
   cd samples/GroupDocs.Viewer.UI.SelfHost.Api.Service.Sample
   dotnet run
   ```

2. **Then start this UI app:**
   ```bash
   cd samples/GroupDocs.Viewer.UI.SelfHost.Api.App.Sample
   dotnet run
   ```

Open your browser at the URL shown in the console output.

## Configuration

```csharp
builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.PreloadPages = 10;  // Must match the API service config
        config.DefaultLanguage = LanguageCode.English;
        config.SupportedLanguages = new[] {
            LanguageCode.English, LanguageCode.French, LanguageCode.Italian
        };
    });

endpoints.MapGroupDocsViewerUI(options =>
{
    options.UIPath = "/document-viewer";
    options.ApiEndpoint = "https://localhost:5001/document-viewer-api";  // Remote API
});
```

Note that this app does **not** call `MapGroupDocsViewerApi()` — the API is hosted separately.

## When to Use Split Architecture

- Scaling UI and API independently
- Running the rendering API on a more powerful machine (rendering is CPU-intensive)
- Sharing one API service across multiple UI deployments
