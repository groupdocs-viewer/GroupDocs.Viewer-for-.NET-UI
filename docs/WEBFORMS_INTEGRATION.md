# Integrating GroupDocs.Viewer.UI with ASP.NET Web Forms

GroupDocs.Viewer.UI is built on ASP.NET Core and cannot be directly referenced from ASP.NET Web Forms projects. However, you can deploy the viewer as a standalone service and integrate it into your Web Forms application using one of the approaches below.

## Option 1: Docker Sidecar (Recommended)

Run the pre-built Docker image alongside your Web Forms application. No code changes required to the viewer.

### Setup

```bash
docker run -d --name viewer -p 8080:8080 -v C:\YourDocuments:/app/Files groupdocs/viewer-net-ui
```

### Embed in Web Forms Page

**ViewDocument.aspx:**
```aspx
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewDocument.aspx.cs" Inherits="MyApp.ViewDocument" %>
<!DOCTYPE html>
<html>
<head>
    <title>Document Viewer</title>
    <style>
        .viewer-frame {
            width: 100%;
            height: 100vh;
            border: none;
        }
    </style>
</head>
<body>
    <iframe id="viewerFrame" runat="server" class="viewer-frame"></iframe>
</body>
</html>
```

**ViewDocument.aspx.cs:**
```csharp
public partial class ViewDocument : System.Web.UI.Page
{
    // URL of the viewer service
    private const string ViewerBaseUrl = "http://localhost:8080";

    protected void Page_Load(object sender, EventArgs e)
    {
        var fileName = Request.QueryString["file"];
        if (!string.IsNullOrEmpty(fileName))
        {
            viewerFrame.Attributes["src"] = $"{ViewerBaseUrl}/?initialFile={Uri.EscapeDataString(fileName)}";
        }
        else
        {
            viewerFrame.Attributes["src"] = ViewerBaseUrl;
        }
    }
}
```

### Usage

Navigate to `ViewDocument.aspx?file=report.docx` in your Web Forms app. The viewer loads in the iframe with the specified document.

### Docker Compose with IIS

If your Web Forms app runs in IIS alongside Docker Desktop:

```yaml
services:
  viewer:
    image: groupdocs/viewer-net-ui:latest
    ports:
      - "8080:8080"
    volumes:
      - C:\inetpub\wwwroot\MyApp\Documents:/app/Files
    restart: unless-stopped
```

Both apps share the same document folder. When users upload files through your Web Forms app, they're immediately available in the viewer.

---

## Option 2: Standalone ASP.NET Core Service

Deploy the viewer as a separate ASP.NET Core application on a different port or subdomain. This approach works without Docker.

### Deploy the Viewer Service

Create a minimal ASP.NET Core app (see [samples/GroupDocs.Viewer.UI.Sample](../samples/GroupDocs.Viewer.UI.Sample/)):

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGroupDocsViewerUI();
builder.Services.AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddLocalStorage(@"C:\SharedDocuments")
    .AddLocalCache(@"C:\ViewerCache");

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGroupDocsViewerUI(options =>
    {
        options.UIPath = "/";
        options.ApiEndpoint = "/viewer-api";
    });
    endpoints.MapGroupDocsViewerApi(options =>
    {
        options.ApiPath = "/viewer-api";
    });
});

app.Run("http://localhost:8080");
```

Publish and run as a Windows Service or in IIS (separate application pool):

```bash
dotnet publish -c Release -o C:\Services\ViewerUI
```

### IIS Configuration

Host both apps in IIS on the same domain using different paths:

```
Default Web Site
├── /                → Web Forms app (MyApp)
└── /viewer          → ASP.NET Core viewer (reverse proxy to localhost:8080)
```

**IIS URL Rewrite rule** (web.config of the Web Forms app):
```xml
<system.webServer>
  <rewrite>
    <rules>
      <rule name="Viewer Proxy" stopProcessing="true">
        <match url="^viewer/(.*)" />
        <action type="Rewrite" url="http://localhost:8080/{R:1}" />
      </rule>
    </rules>
  </rewrite>
</system.webServer>
```

With this setup, your Web Forms pages can link directly to `/viewer/?initialFile=report.docx` without cross-origin issues.

### Embed in Web Forms

Same iframe approach as Option 1, but use relative URLs:

```csharp
viewerFrame.Attributes["src"] = $"/viewer/?initialFile={Uri.EscapeDataString(fileName)}";
```

---

## Option 3: Direct Link (Simplest)

If you don't need the viewer embedded in your Web Forms layout, simply link to it:

```aspx
<asp:HyperLink runat="server" ID="viewLink"
    NavigateUrl='<%# "http://localhost:8080/?initialFile=" + Eval("FileName") %>'
    Target="_blank"
    Text="View Document" />
```

Or in a GridView:

```aspx
<asp:GridView ID="documentsGrid" runat="server" AutoGenerateColumns="false">
    <Columns>
        <asp:BoundField DataField="FileName" HeaderText="Document" />
        <asp:BoundField DataField="Size" HeaderText="Size" />
        <asp:HyperLinkField
            DataNavigateUrlFields="FileName"
            DataNavigateUrlFormatString="http://localhost:8080/?initialFile={0}"
            Text="View"
            Target="_blank"
            HeaderText="" />
    </Columns>
</asp:GridView>
```

---

## Shared File Storage

All three options require the viewer service to access the same documents as your Web Forms app. Common approaches:

| Approach | Setup | Best For |
|---|---|---|
| Shared folder | Both apps point to the same directory | Single server |
| Network share | UNC path (`\\server\documents`) | Multiple servers |
| Azure Blob Storage | Use `AddAzureBlobStorage()` in the viewer | Cloud deployments |
| AWS S3 | Use `AddAwsS3Storage()` in the viewer | AWS deployments |

### Shared Folder Example

Web Forms app saves uploads to `C:\SharedDocuments`:
```csharp
// Web Forms upload handler
fileUpload.SaveAs(Path.Combine(@"C:\SharedDocuments", fileUpload.FileName));
```

Viewer service reads from the same path:
```csharp
.AddLocalStorage(@"C:\SharedDocuments")
```

---

## Security Considerations

When exposing the viewer service:

1. **Bind to localhost only** if the viewer and Web Forms app run on the same server. Use IIS reverse proxy for external access.
2. **Network isolation** — in Docker, use a private network between containers instead of publishing ports.
3. **Authentication** — the viewer does not include authentication. Place it behind your existing auth layer (IIS Windows Auth, reverse proxy with auth, etc.).
4. **CORS** — if the viewer runs on a different origin, configure CORS in the ASP.NET Core app:
   ```csharp
   builder.Services.AddCors(options =>
   {
       options.AddDefaultPolicy(policy =>
       {
           policy.WithOrigins("http://your-webforms-app")
                 .AllowAnyHeader()
                 .AllowAnyMethod();
       });
   });
   app.UseCors();
   ```

---

## Summary

| Approach | Effort | Docker Required | Same Domain | Best For |
|---|---|---|---|---|
| Docker sidecar | Minimal | Yes | No (iframe) | Quick setup, isolated deployment |
| Standalone service | Low | No | Yes (via IIS proxy) | Production, same-server hosting |
| Direct link | None | Either | N/A | Simplest, opens in new tab |

All approaches reuse the existing GroupDocs.Viewer.UI without modification. The Web Forms app only needs an iframe or hyperlink to integrate the viewer.
