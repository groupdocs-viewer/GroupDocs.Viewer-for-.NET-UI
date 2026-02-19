# View As PNG Sample

This sample demonstrates rendering documents as PNG images instead of HTML. This is useful when pixel-perfect rendering is required or when HTML rendering produces layout issues for complex documents.

## What This Sample Shows

- Configuring `ViewerType.Png` for image-based rendering
- Rendering documents as PNG images instead of HTML
- Image rendering mode in the viewer UI (no text selection, zoom works on images)

## Prerequisites

- .NET 8.0 SDK or later

## Running

```bash
cd samples/GroupDocs.Viewer.UI.Sample.ViewAsPng
dotnet run
```

Open your browser at `https://localhost:5001` (or the URL shown in the console output).

## Configuration

The key difference from the basic sample is the `ViewerType`:

```csharp
ViewerType viewerType = ViewerType.Png;

builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.RenderingMode = viewerType.ToRenderingMode();  // Sets RenderingMode.Image
    });

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config =>
    {
        config.SetViewerType(viewerType);  // Renders pages as PNG
    });
```

### Available Image Types

- `ViewerType.Png` — lossless PNG images (larger files, better quality)
- `ViewerType.Jpg` — compressed JPEG images (smaller files, lossy compression)

## When to Use Image Rendering

- Documents with complex layouts that don't render well in HTML
- When text selection is not required
- When consistent cross-browser rendering is important
- CAD drawings, diagrams, or image-heavy documents
