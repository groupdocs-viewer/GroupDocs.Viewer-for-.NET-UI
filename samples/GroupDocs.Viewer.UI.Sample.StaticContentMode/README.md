# Static Content Mode Sample

This sample demonstrates how to serve pre-rendered document content as static files, eliminating the need for a server-side rendering API at runtime. This is ideal for read-only document hosting scenarios.

## What This Sample Shows

- Configuring the viewer UI in static content mode (`StaticContentMode = true`)
- Serving pre-generated document pages, thumbnails, and PDF files via ASP.NET static file middleware
- Running without `AddGroupDocsViewerSelfHostApi()` — no server-side rendering at runtime

## Prerequisites

- .NET 8.0 SDK or later
- Pre-generated content in the `./Content` directory (use the `StaticContentMode.Generator` sample to generate it)

## Running

1. **Generate static content first** using the [StaticContentMode.Generator](../GroupDocs.Viewer.UI.Sample.StaticContentMode.Generator/) sample.

2. **Copy the generated `Content` folder** to this sample's directory.

3. **Run the sample:**
   ```bash
   cd samples/GroupDocs.Viewer.UI.Sample.StaticContentMode
   dotnet run
   ```

Open your browser at `https://localhost:5001` (or the URL shown in the console output).

## Configuration

```csharp
builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.RenderingMode = RenderingMode.Html;
        config.StaticContentMode = true;  // Enables static content mode
    });

// Static files served from ./Content directory
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Content"))
});
```

The API endpoint is set to `/` because static files are served from the root path.

## How It Works

In static content mode, the viewer UI fetches document data from static JSON and HTML files instead of making API calls to a rendering backend. The content structure follows the naming conventions expected by the viewer:

```
Content/
├── list-dir.json              # File listing
└── document.docx/
    ├── view-data.json         # Page metadata
    ├── page-1.html            # Rendered pages
    ├── page-2.html
    ├── thumb-1.png            # Thumbnails
    ├── thumb-2.png
    └── file.pdf               # PDF download
```
