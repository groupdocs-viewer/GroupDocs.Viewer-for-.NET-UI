# GroupDocs Viewer UI

A ready-to-run document viewer for 170+ file formats including PDF, DOCX, XLSX, PPTX, HTML, images, and more. No coding required — just run the container and open your browser.

Powered by [GroupDocs.Viewer for .NET](https://products.groupdocs.com/viewer/net) and the [GroupDocs.Viewer.UI](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI) library.

## Quick Start

```bash
docker run -p 8080:8080 -v ./documents:/app/Files groupdocs/viewer-ui
```

Open http://localhost:8080 in your browser. Place documents into the `./documents` folder.

## Docker Compose

```yaml
services:
  viewer:
    image: groupdocs/viewer-ui:latest
    ports:
      - "8080:8080"
    volumes:
      - ./documents:/app/Files
      - ./cache:/app/Cache
    environment:
      - VIEWER_TYPE=HtmlWithEmbeddedResources
```

## Environment Variables

| Variable | Default | Description |
|---|---|---|
| `VIEWER_TYPE` | `HtmlWithEmbeddedResources` | Rendering type: `HtmlWithEmbeddedResources`, `HtmlWithExternalResources`, `Png`, `Jpg` |
| `GROUPDOCS_LIC_PATH` | | Path to license file inside the container |
| `VIEWER_STORAGE_PATH` | `/app/Files` | Document storage directory |
| `VIEWER_CACHE_PATH` | `/app/Cache` | Rendering cache directory |
| `VIEWER_UI_PATH` | `/` | UI endpoint path |
| `VIEWER_API_PATH` | `/viewer-api` | API endpoint path |
| `VIEWER_PRELOAD_PAGES` | `3` | Number of pages to render on first request |

## Volumes

| Path | Purpose |
|---|---|
| `/app/Files` | Mount your documents here |
| `/app/Cache` | Persistent rendering cache (optional, improves performance) |
| `/app/License` | Mount your license file here |

## Licensing

The image runs in evaluation mode by default. To use a commercial license, mount it as a volume:

```bash
docker run -p 8080:8080 \
  -v ./documents:/app/Files \
  -v ./license:/app/License \
  -e GROUPDOCS_LIC_PATH=/app/License/GroupDocs.Viewer.lic \
  groupdocs/viewer-ui
```

Request a [temporary license](https://purchase.groupdocs.com/temporary-license) for evaluation without limitations.

## Supported Formats

PDF, DOC, DOCX, XLS, XLSX, PPT, PPTX, HTML, MSG, EML, ODT, ODS, ODP, RTF, TXT, CSV, TSV, XML, JSON, DWG, DXF, STL, IFC, PNG, JPG, TIFF, BMP, GIF, SVG, WEBP, ICO, PSD, AI, EPUB, MOBI, and [many more](https://docs.groupdocs.com/viewer/net/supported-document-formats/).

## Health Check

The container includes a health check endpoint at `/health`.

## Source Code

[GitHub Repository](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI)

## Support

- [Free Support Forum](https://forum.groupdocs.com/)
- [Paid Support Helpdesk](https://helpdesk.groupdocs.com/)
