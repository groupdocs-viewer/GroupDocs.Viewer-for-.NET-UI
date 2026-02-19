# Basic Viewer Sample

This sample demonstrates the standard setup of GroupDocs.Viewer.UI with local self-host rendering, local file storage, and local cache. It serves as the starting point for most viewer integrations.

## What This Sample Shows

- Self-host API rendering documents as HTML with embedded resources
- Local file storage (`./Files` directory)
- Local file cache (`./Cache` directory)
- Page preloading (first 3 pages rendered on initial request)

## Prerequisites

- .NET 6.0 SDK or later

## Running

```bash
cd samples/GroupDocs.Viewer.UI.Sample
dotnet run
```

Open your browser at `https://localhost:5001` (or the URL shown in the console output).

## Configuration

```csharp
// Rendering mode: HTML with embedded resources (default)
var viewerType = ViewerType.HtmlWithEmbeddedResources;

// UI is served at /viewer, API at /viewer-api
endpoints.MapGroupDocsViewerUI(options =>
{
    options.UIPath = "/viewer";
    options.ApiEndpoint = "/viewer-api";
});
```

### Licensing

By default the sample runs in trial mode. To use a license:

```csharp
config.SetLicensePath("GroupDocs.Viewer.lic");
// or set the GROUPDOCS_LIC_PATH environment variable
```

## Adding Documents

Place document files in the `./Files` directory. They will appear in the viewer's file browser.
