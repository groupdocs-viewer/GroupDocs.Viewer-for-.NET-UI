# Static Content Mode Generator

This is a console application that pre-renders documents into static files (HTML pages, thumbnails, PDF) for use with the [StaticContentMode](../GroupDocs.Viewer.UI.Sample.StaticContentMode/) sample. It processes all documents in the `./Files` directory and outputs the rendered content to `./Content`.

## What This Sample Shows

- Using GroupDocs.Viewer API directly (without ASP.NET) to render documents
- Generating static content: directory listing, view data JSON, HTML pages, thumbnails, and PDF files
- Supporting all viewer types: HTML with embedded/external resources, JPG, and PNG

## Prerequisites

- .NET 8.0 SDK or later
- Windows (uses `System.Drawing.Common` via `SelfHost.Api`)

## Running

1. **Place documents** in the `./Files` directory.

2. **Run the generator:**
   ```bash
   cd samples/GroupDocs.Viewer.UI.Sample.StaticContentMode.Generator
   dotnet run
   ```

3. **Copy the generated `Content` folder** to the `StaticContentMode` sample directory.

## Configuration

Edit the constants at the top of `Program.cs` to change behavior:

```csharp
private const ViewerType VIEWER_TYPE = ViewerType.HtmlWithEmbeddedResources;
private const string STORAGE_PATH = "./Files";
private const string CONTENT_FOLDER = "Content";
```

### Password-Protected Documents

Files whose names start with `password` are automatically opened with the password `12345`.

## Output Structure

The generator creates the following structure in `./Content`:

```
Content/
├── list-dir.json                  # File browser listing
└── <filename>/
    ├── view-data.json             # Page metadata (dimensions, URLs)
    ├── page-{n}.html              # Rendered page content
    ├── thumb-{n}.png              # Page thumbnails (HTML mode only)
    └── file.pdf                   # PDF representation
```
