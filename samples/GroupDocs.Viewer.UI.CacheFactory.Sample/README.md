# Cache Factory Sample

This is a console application that pre-populates the local file cache by rendering all documents in the `./Files` directory. Running this before starting the viewer application ensures that users see instant page loads without waiting for on-demand rendering.

## What This Sample Shows

- Using GroupDocs.Viewer API directly (without ASP.NET) to pre-render documents
- Wrapping a viewer with `CachingViewer` to write rendered output to a `LocalFileCache`
- Generating document info, pages, thumbnails, and PDF for each document

## Prerequisites

- .NET 8.0 SDK or later
- Windows (uses `System.Drawing.Common` via `SelfHost.Api`)

## Running

1. **Place documents** in the `./Files` directory.

2. **Run the cache generator:**
   ```bash
   cd samples/GroupDocs.Viewer.UI.CacheFactory.Sample
   dotnet run
   ```

3. **Copy the generated `Cache` folder** to your viewer application's directory, or configure both to use the same cache path.

## Configuration

Edit the variables at the top of `Program.cs`:

```csharp
string storagePath = "./Files";   // Input documents
string cachePath = "./Cache";     // Output cache directory
```

### Password-Protected Documents

Files whose names start with `password` are automatically opened with the password `12345`.

## How It Works

The sample creates a `CachingViewer` that wraps an `HtmlWithEmbeddedResourcesViewer` with a `LocalFileCache`. For each document it:

1. Retrieves document info (file type, page count, dimensions)
2. Renders all pages as HTML with embedded resources
3. Generates page thumbnails
4. Creates a PDF representation

The resulting cache files match the format used by the viewer's `AddLocalCache()`, so the cache can be shared directly.
