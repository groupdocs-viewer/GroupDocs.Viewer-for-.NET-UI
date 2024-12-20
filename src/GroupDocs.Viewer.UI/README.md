# UI for GroupDocs.Viewer for .NET

![GroupDocs.Viewer.UI](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/raw/main/doc/images/viewer-ui.png)

GroupDocs.Viewer.UI is a feature-rich UI designed to work with [GroupDocs.Viewer for .NET](https://products.groupdocs.com/viewer/net). It enables viewing of popular file and document formats in a web browser.

## Installation and integration

To integrate GroupDocs.Viewer.UI in your ASP.NET Core project:

### Add required packages

Include packages in your project:

```bash
dotnet add package GroupDocs.Viewer.UI
dotnet add package GroupDocs.Viewer.UI.SelfHost.Api
dotnet add package GroupDocs.Viewer.UI.Api.Local.Storage
dotnet add package GroupDocs.Viewer.UI.Api.Local.Cache
```

> **Note**: If you're planning to host your app on Linux, use the `GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform` package.

### Update the `Startup` class

Add the required services and middleware in your `Startup` class:

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGroupDocsViewerUI();

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddLocalStorage("./Files")
    .AddLocalCache("./Cache");

var app = builder.Build();

app
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapGroupDocsViewerUI(options =>
        {
            options.UIPath = "/viewer";
            options.ApiEndpoint = "/viewer-api";
        });
        endpoints.MapGroupDocsViewerApi(options =>
        {
            options.ApiPath = "/viewer-api";
        });
    });

app.Run();
```

This configuration registers `/viewer` as the middleware for the Single Page Application (SPA) and `/viewer-api` as the middleware for serving the API.

> **Note**: Ensure the `Files` and `Cache` folders are created manually before running the application.

### Set the license

Optionally, you can specify the license path using the following code or set the environment variable `GROUPDOCS_LIC_PATH`:

```cs
builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config =>
    {
        config.SetLicensePath("GroupDocs.Viewer.lic"); 
    })
```

For more information about trial limitations, refer to the [Licensing and Evaluation](https://docs.groupdocs.com/viewer/net/licensing-and-evaluation/) documentation.

To request a temporary license, visit [GroupDocs.Viewer for .NET](https://products.groupdocs.com/viewer/net/) and click the **Start Free Trial** button.

## UI Overview

The UI is an Angular-based Single Page Application (SPA), the same one used in the [GroupDocs.Viewer App](https://products.groupdocs.app/viewer/total). You can configure the SPA's path by updating the `UIPath` property:

```cs
endpoints.MapGroupDocsViewerUI(options =>
{
    options.UIPath = "/my-viewer-app";
});
```

There are two types of configuration options you can use to customize the UI:  
1. **Customize UI behavior**.  
2. **Show or hide UI controls**.

### Customize UI behavior

Using the following options, you can customize UI behavior:

#### Set Rendering Mode

The UI can display HTML and image files. You can set which mode should be used. By default, the UI is configured to display HTML documents.

```cs
builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.RenderingMode = RenderingMode.Image; // Set rendering mode to Image
    });
```

**Important**: When using `Image` mode ensure to set corresponding `ViewerType.Png` or `ViewerType.Jpg` for the self-hosted API:

```cs
builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config =>
    {
        config.SetViewerType(ViewerType.Png); // or ViewerType.Jpg
    })
```

#### Enable Static Content mode

By default, the viewer relies on a backend API that serves information about the file, pages, thumbnails, and the PDF file for printing.  
When `StaticContentMode` is enabled, the app will use pre-generated static content via GET requests.  

```cs
builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.StaticContentMode = true; // Enable static content mode
    });
```

See this [sample app](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/tree/main/samples/GroupDocs.Viewer.UI.Sample.StaticContentMode) for more details.  
You can also find the content generator app [here](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/tree/main/samples/GroupDocs.Viewer.UI.Sample.StaticContentMode.Generator).

#### Set the initial file

By default, no file is opened when the application starts. You can specify an initial file to open on startup:

```cs
builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.InitialFile = "annual-review.docx";
    });
```

The initial file can also be set using a query string parameter, e.g., `?file=annual-review.docx`.

#### Set number pages to preload

By default, the first three pages of a document are generated when it is opened. To render all pages at once, set `PreloadPages` to `0`:

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.PreloadPages = 0; // Render all pages on open
    });
```

### Disable context menu

To disable context menu or mouse right click, set `EnableContextMenu` to `false`. By default, feature is enabled.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnableContextMenu = false;
    });
```

### Disable huperlinks

To disable clickable links in document set `EnableHyperlinks` to `false`. By default, links are enabled.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnableHyperlinks = false;
    });
```

### Show or hide UI controls

The screenshot below highlights the main UI controls that you can show or hide. By default all the controls are visible.

![GroupDocs.Viewer.UI Controlls](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/raw/main/doc/images/viewer-ui-controls.png)

You can also completely hide header and toolbar:

#### Hide header

To hide header, set `EnableHeader` to `false`. By default, the header is visible.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnableHeader = false; // Hide header
    });
```

#### Hide toolbar

To hide toolbar, set `EnableToolbar` to `false`. By default, the toolbar is visible.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnableToolbar = false; // Hide toolbar
    });
```

#### Hide File Name

To hide the file name on the header pane, set `EnableFileName` to `false`. By default, the file name is visible.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnableFileName = false; // Hide file name
    });
```

#### Hide thumbnails

To hide thumbnails pane, set `EnableThumbnails` to `false`. By default, this pane is visible.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnableThumbnails = false; // Hide thumbnails
    });
```

#### Hide zoom selector

To hide zoom selector in the tools pane, set `EnableZoom` to `false`. By default, this selector is visible.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnableZoom = false; // Hide zoom selector
    });
```

#### Hide page selector

To hide the page selector control in the tools pane, set `EnablePageSelector` to `false`. By default, the page selector is enabled.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnablePageSelector = false; // Hide page selector
    });
```

#### Hide Search button

To hide search button in the tools pane, set `EnableSearch` to `false`. By default, this button is visible.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnableSearch = false;
    });
```

> **Note**: Search is not supported when `RenderingMode` is set to `RenderingMode.Image`.

#### Hide Print button

To hide the `Print` button in the tools pane, set `EnableDownloadPdf` to `false`. By default, this button is visible.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnablePrint = false;
    });
```

#### Hide Download PDF button

To hide the `Download PDF` button in the tools pane, set `EnableDownloadPdf` to `false`. By default, this button is visible.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnableDownloadPdf = false;
    });
```

#### Hide Present button

To hide the `Present` button in the tools pane, set `EnablePresentation` to `false`. By default, this button is visible.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnablePresentation = false;
    });
```

#### Hide Open File button

To hide `Open File` button, set `EnableFileBrowser` to `false`. By default, this button is visible.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnableFileBrowser = false;
    });
```

#### Hide File Upload button

To hide file upload in file browser popup, set `EnableFileUpload` to `false`. By default, this button is visible.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnableFileUpload = false;
    });
```

#### Hide language selector

To hide language selector, set `EnableLanguageSelector` to `false`. By default, this selector is enabled.

```cs
builder.Services
   .AddGroupDocsViewerUI(config =>
    {
        config.EnableLanguageSelector = false;
    });
```

#### Set the UI language

To set the UI language set `DefaultLanguage` property. The list of supported languages is configured via `SupportedLanguages` property.

```cs
builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.DefaultLanguage = LanguageCode.German;
        config.SupportedLanguages = new[] { 
            LanguageCode.English, 
            LanguageCode.German, 
            LanguageCode.French 
        };
    });
```

Default language can also be set via a query string parameter, e.g. `?lang=de`.

## API Overview

The API serves document data such as metadata, pages in HTML/PNG/JPG formats, and PDFs for printing. It can be hosted in the same application or separately.

### Available API Implementations

- [GroupDocs.Viewer.UI.SelfHost.Api](https://www.nuget.org/packages/GroupDocs.Viewer.UI.SelfHost.Api)
- [GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform](https://www.nuget.org/packages/GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform)
- [GroupDocs.Viewer.UI.Cloud.Api](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Cloud.Api)

### Self-Hosted

The API is used to retrieve document information and convert documents to HTML, PNG, JPG, and PDF.

There are two versions of the self-hosted API:

- **[GroupDocs.Viewer.UI.SelfHost.Api](https://www.nuget.org/packages/GroupDocs.Viewer.UI.SelfHost.Api)**:  
  This package is based on the [GroupDocs.Viewer](https://www.nuget.org/packages/groupdocs.viewer) NuGet package and can be used in .NET 6 applications on Windows and Linux.  
  It relies heavily on `System.Drawing.Common`.

- **[GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform](https://www.nuget.org/packages/GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform)**:  
  This package is based on the [GroupDocs.Viewer.CrossPlatform](https://www.nuget.org/packages/groupdocs.viewer.crossplatform) NuGet package and works with .NET 6 and higher versions on both Linux and Windows.

After installing the package that suits your requirements, you can add it in your startup code:

```cs
services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi();
```

A sample application demonstrating how to use the self-hosted API can be found [here](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/tree/main/samples/GroupDocs.Viewer.UI.Sample).

### GroupDocs Cloud

In case you want to offload rendering to [GroupDocs Cloud](https://www.groupdocs.cloud/) infrastructure you can opt to use [GroupDocs.Viewer Cloud API](https://products.groupdocs.cloud/viewer/family/). To get started create your first application at <https://dashboard.groupdocs.cloud/applications> and copy your `Client Id` and `Client Secret` keys.

```cs
services
    .AddControllers()
    .AddGroupDocsViewerCloudApi(config => 
        config
            .SetClientId("XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX")
            .SetClientSecret("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX")
    )
```

The sample application that shows how to use Cloud Api can be found [here](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/tree/main/samples/GroupDocs.Viewer.UI.Cloud.Api.Sample).

## Storage providers

Storage providers are used to read/write file from/to the storage. The storage provider is mandatory.

- [GroupDocs.Viewer.UI.Api.Local.Storage](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Api.Local.Storage)
- [GroupDocs.Viewer.UI.Api.Cloud.Storage](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Api.Cloud.Storage)
- [GroupDocs.Viewer.UI.Api.AzureBlob.Storage](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Api.AzureBlob.Storage)
- [GroupDocs.Viewer.UI.Api.AwsS3.Storage](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Api.AwsS3.Storage)

### Local storage

To render files from your local drive use local file storage.

```cs
services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddLocalStorage("./Files");
```

### Cloud storage

When rendering files using [GroupDocs Cloud](https://dashboard.groupdocs.cloud/) infrastructure you can use our cloud storage provider. GroupDocs Cloud storage supports number of 3d-party storages including Amazon S3, Google Drive and Cloud, Azure, Dropbox, Box, and FTP. To start using GroupDocs Cloud get your `Client ID` and `Client Secret` at <https://dashboard.groupdocs.cloud/applications>.

```cs
var clientId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
var clientSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
var storageName = "Storage Name"

services
    .AddControllers()
    .AddGroupDocsViewerCloudApi(config => 
        config
            .SetClientId(clientId)
            .SetClientSecret(clientSecret)
            .SetStorageName(storageName)
    )
    .AddCloudStorage(config => 
        config
            .SetClientId(clientId)
            .SetClientSecret(clientSecret)
            .SetStorageName(storageName)
    )
```

### Azure Blob Storage

You can also use [Azure Blob Storage](https://azure.microsoft.com/en-us/products/storage/blobs/) as a storage provider for Viewer.

```cs
services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddAzureBlobStorage(options =>
    {
        options.AccountName = "<account name here>";
        options.AccountKey = "<account key here>";
        options.ContainerName = "<conainer name here>";
    });
```

### Amazon S3 Storage

Viewer also supports the [Amazon S3 Storage](https://aws.amazon.com/s3/) storage provider.

```cs
services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddAwsS3Storage(options =>
    {
        options.Region = "<region>";
        options.BucketName = "<bucket name>";
        options.AccessKey = "<access key>";
        options.SecretKey = "<secret key>";
    });
```

### Custom storage provider

You can add your storage provider by implementing the [GroupDocs.Viewer.UI.Core.IFileStorage](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/blob/main/src/GroupDocs.Viewer.UI.Core/IFileStorage.css) interface.

To add you storage provider you have to register it:

```cs
//NOTE: register after AddGroupDocsViewerSelfHostApi() 
builder.Services.AddTransient<IFileStorage, MyFileStorage>();
```

## Cache providers

To cache the output files created by GroupDocs.Viewer you can use one of the cache providers:

- [GroupDocs.Viewer.UI.Api.Local.Cache](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Api.Local.Cache)
- [GroupDocs.Viewer.UI.Api.InMemory.Cache](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Api.InMemory.Cache)

### Local cache

Stores cache on the local drive. You have to specify the path to the folder where cache files will be stored. 

```cs
services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddLocalStorage("./Files")
    .AddLocalCache("./Cache");
```

### In-memory cache

Stores cache in memory using [Microsoft.Extensions.Caching.Memory](https://www.nuget.org/packages/microsoft.extensions.caching.memory/) package.

```cs
services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddLocalStorage("./Files")
    .AddInMemoryCache();
```

### Custom cache provider

You can add your cache provider by implementing the [GroupDocs.Viewer.UI.Core.IFileCache](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/blob/main/src/GroupDocs.Viewer.UI.Core/IFileCache.cs) interface.
To add you cache provider you have to register it:

```cs
//NOTE: register after AddGroupDocsViewerSelfHostApi()
builder.Services.AddSingleton<IFileCache, MyFileCache>();
```

Find example implementation of custom cache provider [here](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/tree/main/samples/GroupDocs.Viewer.UI.Sample.CustomCacheProvider).

## Linux dependencies

To run the self-hosted API on Linux, the following dependencies are required:

1. **Microsoft Fonts**: You can copy these from your Windows host.  
   If you're using Ubuntu, refer to the [How to Install Windows Fonts on Ubuntu](https://docs.groupdocs.com/viewer/java/how-to-install-windows-fonts-on-ubuntu/) documentation.

2. **`libgdiplus` Package**: This package is required for using `GroupDocs.Viewer.UI.SelfHost.Api` on Linux. Ensure you install the latest available version of `libgdiplus`.

## Running in Docker

To run the self-hosted API in Docker, you need to install Microsoft Fonts and the latest version of the `libgdiplus` package.  
Refer to these example Dockerfiles that list the required dependencies:

- For `GroupDocs.Viewer.UI.SelfHost.Api` and .NET 6 app, see this [Dockerfile](/samples/GroupDocs.Viewer.UI.Sample/Dockerfile).
- For `GroupDocs.Viewer.UI.SelfHost.Api.CrossPlatform` and .NET 8 app, see this [Dockerfile](/samples/GroupDocs.Viewer.UI.Sample.CrossPlatform/Dockerfile).

## Contributing

Your contributions are welcome when you want to make the project better by adding new feature, improvement or a bug-fix.

1. Read and follow the [Don't push your pull requests](https://www.igvita.com/2011/12/19/dont-push-your-pull-requests/)
2. Follow the code guidelines and conventions.
3. Make sure to describe your pull requests well and add documentation.

## Technical support

GroupDocs provides unlimited free technical support for all of its products. Support is available to all users, including those evaluating the product. You can access support at the [Free Support Forum](https://forum.groupdocs.com/) and the [Paid Support Helpdesk](https://helpdesk.groupdocs.com/).
