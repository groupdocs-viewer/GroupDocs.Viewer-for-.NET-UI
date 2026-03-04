# Desktop Applications Specification

This document specifies three desktop application variants that reuse the existing GroupDocs.Viewer.UI rendering, storage, and caching infrastructure.

## Shared Architecture

All three variants reuse the following framework-agnostic components without modification:

| Component | Package | Purpose |
|---|---|---|
| `GroupDocs.Viewer.UI.Core` | Interfaces, entities, config | `IViewer`, `IFileStorage`, `IFileCache`, `Page`, `DocumentInfo`, etc. |
| `GroupDocs.Viewer.UI.SelfHost.Api.Shared` | Rendering logic | `BaseViewer`, `HtmlViewer`, `PngViewer`, `JpgViewer` |
| `GroupDocs.Viewer.UI.Api.Local.Storage` | File I/O | `LocalFileStorage` |
| `GroupDocs.Viewer.UI.Api.Local.Cache` | Disk cache | `LocalFileCache` |
| `GroupDocs.Viewer.UI.Api.InMemory.Cache` | Memory cache | `InMemoryFileCache` |

The Angular SPA (embedded in `GroupDocs.Viewer.UI`) is also reused — it communicates with the backend via a JSON API contract (`window.groupdocs.viewer` config + REST-like endpoints). Each variant hosts the SPA differently but serves the same API.

### API Contract (shared by all variants)

The Angular app expects these endpoints relative to `apiEndpoint`:

| Endpoint | Method | Request | Response |
|---|---|---|---|
| `/list-dir` | POST | `{ path: "/" }` | `[{ path, name, isDir, size }]` |
| `/upload-file` | POST | Multipart form | `{ fileName }` |
| `/view-data` | POST | `{ file, fileType, password }` | `{ file, fileType, fileName, pages: [{ number, width, height, pageUrl, thumbUrl }] }` |
| `/create-pages` | POST | `{ file, fileType, password, pages: [1,2,3] }` | `[{ number, width, height, pageUrl, thumbUrl }]` |
| `/create-pdf` | POST | `{ file, fileType, password }` | `{ pdfUrl }` |
| `/get-page` | GET | `?file=X&page=N` | Binary (HTML/PNG/JPG) |
| `/get-thumb` | GET | `?file=X&page=N` | Binary (PNG/JPG) |
| `/get-pdf` | GET | `?file=X` | Binary (PDF) |
| `/get-resource` | GET | `?file=X&page=N&resource=Y` | Binary (CSS/JS/font) |

---

## 1. WPF Application

### Overview

A Windows desktop application using WPF with WebView2 to host the Angular SPA. An in-process Kestrel server provides the API backend. The user sees a native Windows window with the document viewer filling the client area.

### Target Framework

- .NET 8.0, Windows only
- `<TargetFramework>net8.0-windows</TargetFramework>`

### Dependencies

| Package | Purpose |
|---|---|
| `Microsoft.Web.WebView2` | Chromium-based browser control |
| `Microsoft.AspNetCore.App` (FrameworkReference) | In-process Kestrel for API |
| `GroupDocs.Viewer.UI` | Angular SPA (embedded resources) |
| `GroupDocs.Viewer.UI.Api` | API controller + models |
| `GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform` | Rendering engine |
| `GroupDocs.Viewer.UI.Api.Local.Storage` | File storage |
| `GroupDocs.Viewer.UI.Api.Local.Cache` | File cache |

### Architecture

```
┌─────────────────────────────────────┐
│           WPF MainWindow            │
│  ┌───────────────────────────────┐  │
│  │         WebView2              │  │
│  │   (Angular SPA from          │  │
│  │    embedded resources)       │  │
│  │                              │  │
│  │   HTTP calls to localhost    │  │
│  └──────────┬───────────────────┘  │
│             │                       │
│  ┌──────────▼───────────────────┐  │
│  │   In-process Kestrel         │  │
│  │   localhost:{random-port}    │  │
│  │                              │  │
│  │   ┌─ ViewerController ────┐  │  │
│  │   │  IViewer              │  │  │
│  │   │  IFileStorage         │  │  │
│  │   │  IFileCache           │  │  │
│  │   └───────────────────────┘  │  │
│  └──────────────────────────────┘  │
└─────────────────────────────────────┘
```

### Startup Flow

1. `App.OnStartup`:
   - Build and start a `WebApplication` on `http://localhost:0` (random available port)
   - Register all viewer services (same DI setup as ASP.NET samples)
   - Map viewer UI and API endpoints
   - Capture the assigned port from `app.Urls`
2. `MainWindow.Loaded`:
   - Initialize WebView2
   - Navigate to `http://localhost:{port}/`
3. `MainWindow.Closing`:
   - Stop the Kestrel server
   - Dispose WebView2

### Key Implementation Details

**MainWindow.xaml:**
```xml
<Window Title="GroupDocs Document Viewer" Width="1280" Height="800">
    <wv2:WebView2 x:Name="webView" />
</Window>
```

**App.xaml.cs (startup):**
```csharp
var builder = WebApplication.CreateBuilder();
builder.WebHost.UseUrls("http://127.0.0.1:0");

// Reuse existing DI registration
builder.Services.AddGroupDocsViewerUI(config => { ... });
builder.Services.AddControllers()
    .AddGroupDocsViewerSelfHostApi(config => { ... })
    .AddLocalStorage(documentsPath)
    .AddLocalCache(cachePath);

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(endpoints => {
    endpoints.MapGroupDocsViewerUI(options => { options.UIPath = "/"; ... });
    endpoints.MapGroupDocsViewerApi(options => { options.ApiPath = "/viewer-api"; });
});

await app.StartAsync();
var port = new Uri(app.Urls.First()).Port;
```

**Window features:**
- Native file open dialog (Open File button in toolbar or menu)
- Drag-and-drop documents onto the window
- Window title shows current document name
- Remember window size/position in user settings

### Project Structure

```
apps/GroupDocs.Viewer.UI.WPF/
├── App.xaml
├── App.xaml.cs              # Kestrel startup, DI registration
├── MainWindow.xaml          # WebView2 host
├── MainWindow.xaml.cs       # WebView2 init, navigation
├── GroupDocs.Viewer.UI.WPF.csproj
└── Properties/
    └── launchSettings.json
```

### Packaging

- Publish as single-file executable: `dotnet publish -r win-x64 --self-contained -p:PublishSingleFile=true`
- Optionally package as MSIX for Windows Store distribution
- WebView2 runtime: require Evergreen runtime (pre-installed on Windows 10/11)

---

## 2. MAUI Application

### Overview

A cross-platform desktop/mobile application using .NET MAUI. Uses `BlazorWebView` to host the Angular SPA (same embedded resources). Supports Windows, macOS, and optionally Android/iOS.

### Target Framework

- .NET 8.0 multi-platform
- `<TargetFrameworks>net8.0-windows10.0.19041.0;net8.0-maccatalyst</TargetFrameworks>`

### Dependencies

| Package | Purpose |
|---|---|
| `Microsoft.Maui.Controls` | MAUI framework |
| `Microsoft.AspNetCore.Components.WebView.Maui` | BlazorWebView for hosting web content |
| `GroupDocs.Viewer.UI` | Angular SPA (embedded resources) |
| `GroupDocs.Viewer.UI.Api` | API controller + models |
| `GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform` | Rendering engine |
| `GroupDocs.Viewer.UI.Api.Local.Storage` | File storage |
| `GroupDocs.Viewer.UI.Api.Local.Cache` | File cache |

### Architecture

```
┌─────────────────────────────────────┐
│           MAUI Application          │
│  ┌───────────────────────────────┐  │
│  │       BlazorWebView           │  │
│  │   (Angular SPA loaded via     │  │
│  │    custom scheme handler)     │  │
│  │                              │  │
│  │   Intercepts HTTP requests   │  │
│  └──────────┬───────────────────┘  │
│             │                       │
│  ┌──────────▼───────────────────┐  │
│  │   In-process Kestrel         │  │
│  │   (same as WPF approach)     │  │
│  │                              │  │
│  │   ViewerController           │  │
│  │   IViewer + IFileStorage     │  │
│  └──────────────────────────────┘  │
└─────────────────────────────────────┘
```

### Two Hosting Options

**Option A: In-process Kestrel (recommended)**

Same approach as WPF — start Kestrel on a random localhost port and point the web view to it. Works identically on all platforms.

**Option B: Custom scheme handler (advanced)**

Register a custom URI scheme (e.g., `app://viewer/`) that intercepts requests and routes them to in-process services without an HTTP server. More complex but avoids opening a network port.

Recommendation: Start with Option A for simplicity. Option B can be added later for mobile platforms where opening a port may be restricted.

### Startup Flow

1. `MauiProgram.CreateMauiApp()`:
   - Register MAUI services
   - Build and start Kestrel server (same as WPF)
   - Register server URL as a singleton for the main page
2. `MainPage` loads:
   - WebView navigates to `http://localhost:{port}/`
3. App lifecycle:
   - `OnSleep`: optionally pause server
   - `OnResume`: ensure server is running
   - `OnDestroying`: stop server

### Key Implementation Details

**MainPage.xaml:**
```xml
<ContentPage>
    <WebView x:Name="webView" />
</ContentPage>
```

**Platform considerations:**

| Platform | Web Engine | Notes |
|---|---|---|
| Windows | WebView2 (Chromium) | Same as WPF |
| macOS | WKWebView (Safari) | Test CSS/JS compatibility |
| Android | Android WebView (Chromium) | Localhost access works |
| iOS | WKWebView (Safari) | Localhost access works |

### Project Structure

```
apps/GroupDocs.Viewer.UI.MAUI/
├── App.xaml
├── App.xaml.cs
├── MauiProgram.cs           # DI, Kestrel startup
├── MainPage.xaml             # WebView host
├── MainPage.xaml.cs
├── GroupDocs.Viewer.UI.MAUI.csproj
├── Platforms/
│   ├── Windows/
│   ├── MacCatalyst/
│   ├── Android/
│   └── iOS/
└── Resources/
```

### Packaging

- Windows: MSIX package
- macOS: .app bundle via `dotnet publish -f net8.0-maccatalyst`
- Android: APK/AAB
- iOS: IPA (requires Mac build host)

---

## 3. Blazor Application

### Overview

A Blazor Server or Blazor WebAssembly application that hosts the document viewer. Unlike WPF/MAUI, this runs in a browser but offers tighter .NET integration than the pure ASP.NET approach — the viewer can be embedded as a Blazor component alongside other Blazor pages.

### Variants

#### 3a. Blazor Server

The viewer runs as part of a Blazor Server app. The Angular SPA is served as a sub-path, and API calls go to the same server. SignalR handles Blazor interactions for other pages; the viewer page loads the Angular SPA in an iframe or embedded div.

**Target Framework:** `net8.0`

**Architecture:**
```
┌─────────────────────────────────────┐
│         ASP.NET + Blazor Server     │
│                                      │
│  /              → Blazor pages       │
│  /documents     → Blazor file list   │
│  /viewer        → Angular SPA        │
│  /viewer-api/*  → ViewerController   │
│                                      │
│  Blazor pages can link to /viewer    │
│  with query params for initial file  │
└─────────────────────────────────────┘
```

**Key benefit:** The viewer integrates into a larger Blazor app. Blazor pages handle navigation, authentication, and business logic. The viewer page is an Angular SPA loaded as a component.

#### 3b. Blazor WebAssembly (Standalone)

The entire app runs client-side. The Angular SPA is replaced with a Blazor-rendered viewer UI that calls the API backend directly.

**Note:** This variant requires building a new Blazor UI component that replaces the Angular SPA. This is significantly more work and is listed here for completeness. The recommended approach is 3a (Blazor Server with embedded Angular SPA).

### Recommended: Blazor Server with Angular SPA (3a)

### Dependencies

| Package | Purpose |
|---|---|
| `Microsoft.AspNetCore.Components` | Blazor framework |
| `GroupDocs.Viewer.UI` | Angular SPA |
| `GroupDocs.Viewer.UI.Api` | API controller |
| `GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform` | Rendering |
| `GroupDocs.Viewer.UI.Api.Local.Storage` | Storage |
| `GroupDocs.Viewer.UI.Api.Local.Cache` | Cache |

### Architecture

```
┌──────────────────────────────────────────┐
│            Blazor Server App             │
│                                          │
│  ┌────────────────────────────────────┐  │
│  │  Blazor Pages (Razor Components)  │  │
│  │  - Home page                       │  │
│  │  - Document list (file browser)   │  │
│  │  - Settings page                   │  │
│  └──────────┬─────────────────────────┘  │
│             │ Navigation                  │
│  ┌──────────▼─────────────────────────┐  │
│  │  Viewer Page                       │  │
│  │  <iframe src="/viewer?file=X" />   │  │
│  │  or                                │  │
│  │  <ViewerComponent File="X" />      │  │
│  └────────────────────────────────────┘  │
│                                          │
│  ┌────────────────────────────────────┐  │
│  │  ASP.NET Pipeline                  │  │
│  │  /viewer     → Angular SPA        │  │
│  │  /viewer-api → ViewerController   │  │
│  │  /_blazor    → SignalR             │  │
│  └────────────────────────────────────┘  │
└──────────────────────────────────────────┘
```

### Startup Flow

1. `Program.cs`:
   - Standard Blazor Server setup
   - Register viewer services (same DI as ASP.NET)
   - Map Blazor hub, viewer UI, and viewer API endpoints
2. Blazor `ViewerPage.razor`:
   - Renders an iframe pointing to `/viewer?initialFile={filename}`
   - Or uses JS interop to load the Angular SPA into a div

### Key Implementation Details

**Program.cs:**
```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddGroupDocsViewerUI(config => { ... });
builder.Services.AddControllers()
    .AddGroupDocsViewerSelfHostApi(config => { ... })
    .AddLocalStorage("./Files")
    .AddLocalCache("./Cache");

var app = builder.Build();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapGroupDocsViewerUI(options => { options.UIPath = "/viewer"; ... });
app.MapGroupDocsViewerApi(options => { options.ApiPath = "/viewer-api"; });

app.Run();
```

**ViewerPage.razor:**
```razor
@page "/view/{FileName}"

<PageTitle>@FileName - Document Viewer</PageTitle>

<div class="viewer-container">
    <iframe src="/viewer?initialFile=@FileName"
            style="width:100%; height:100%; border:none;" />
</div>

@code {
    [Parameter] public string FileName { get; set; }
}
```

**DocumentList.razor (Blazor-native file browser):**
```razor
@page "/documents"
@inject IFileStorage FileStorage

<h3>Documents</h3>
<ul>
    @foreach (var file in files)
    {
        <li>
            <a href="/view/@file.FileName">@file.FileName</a>
        </li>
    }
</ul>

@code {
    private IEnumerable<FileSystemEntry> files;

    protected override async Task OnInitializedAsync()
    {
        files = await FileStorage.ListDirsAndFilesAsync("/");
    }
}
```

### Project Structure

```
apps/GroupDocs.Viewer.UI.Blazor/
├── Program.cs
├── App.razor
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   └── Pages/
│       ├── Home.razor
│       ├── Documents.razor       # Blazor file browser
│       └── ViewerPage.razor      # Embeds Angular SPA
├── GroupDocs.Viewer.UI.Blazor.csproj
└── wwwroot/
    └── css/
```

### Packaging

- Deploy as standard ASP.NET app (same as current web deployment)
- Docker image (reuse existing Dockerfile pattern)
- Azure App Service, AWS, etc.

---

## Comparison

| Feature | WPF | MAUI | Blazor Server |
|---|---|---|---|
| **Platforms** | Windows | Windows, macOS, Android, iOS | Browser (any OS) |
| **Web engine** | WebView2 | Platform WebView | Browser native |
| **Offline** | Yes | Yes | No (needs server) |
| **Distribution** | EXE / MSIX | MSIX / .app / APK | Web deploy / Docker |
| **Network port** | localhost (hidden) | localhost (hidden) | Standard web server |
| **Implementation effort** | Medium | Medium-High | Low |
| **Reuse of existing code** | ~90% | ~85% | ~95% |
| **Best for** | Windows power users | Cross-platform desktop/mobile | Teams already using Blazor |

## Implementation Priority

1. **Blazor Server** — lowest effort, highest code reuse, broadest reach. Essentially the existing ASP.NET app with Blazor pages added around the viewer.
2. **WPF** — straightforward WebView2 + Kestrel pattern, Windows-only but polished desktop experience.
3. **MAUI** — most effort due to multi-platform testing, but covers mobile and macOS.

## Shared Code Extraction (prerequisite)

Before implementing any variant, extract a shared services library to avoid duplicating DI registration logic:

```
src/GroupDocs.Viewer.UI.Services/
├── ViewerServiceCollectionExtensions.cs   # Common DI setup
├── ViewerConfiguration.cs                  # Unified config POCO
└── GroupDocs.Viewer.UI.Services.csproj     # References Core + Shared + Storage + Cache
```

This library would provide:
```csharp
builder.Services.AddGroupDocsViewer(config => {
    config.StoragePath = "./Files";
    config.CachePath = "./Cache";
    config.ViewerType = ViewerType.HtmlWithEmbeddedResources;
});
```

All three apps (WPF, MAUI, Blazor) call this single method instead of repeating the same DI setup.
