# UI for GroupDocs.Viewer for .NET

![Build Packages](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/actions/workflows/build_packages.yml/badge.svg)
![Nuget](https://img.shields.io/nuget/v/groupdocs.viewer.ui?label=GroupDocs.Viewer.UI)
![Nuget](https://img.shields.io/nuget/dt/GroupDocs.Viewer.UI?label=GroupDocs.Viewer.UI)

![GroupDocs.Viewer.UI](./doc/images/viewer-ui.png)

GroupDocs.Viewer UI is a rich UI interface that designed to work in conjunction with [GroupDocs.Viewer for .NET](https://products.groupdocs.com/viewer/net) to display most popular file and document formats in a browser.

To integrate GroupDocs.Viewer UI in your ASP.NET Core project you just need to add services and middlewares into your `Startup` class that provided in `GroupDocs.Viewer.UI` and related packages.

Include packages in your project:

```PowerShell
dotnet add package GroupDocs.Viewer.UI
dotnet add package GroupDocs.Viewer.UI.SelfHost.Api
dotnet add package GroupDocs.Viewer.UI.Api.Local.Storage
dotnet add package GroupDocs.Viewer.UI.Api.Local.Cache
```

Add configuration to your `Startup` class:

```cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddGroupDocsViewerUI();

        services
            .AddControllers()
            .AddGroupDocsViewerSelfHostApi(config =>
            {
                //Trial limitations https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/
                //Temporary license can be requested at https://purchase.groupdocs.com/temporary-license
                //config.SetLicensePath("c:\\licenses\\GroupDocs.Viewer.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'
            })
            .AddLocalStorage("./Files")
            .AddLocalCache("./Cache");
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app
            .UseRouting()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapGroupDocsViewerUI(options =>
                {
                    options.UIPath = "/viewer";
                    options.APIEndpoint = "/viewer-api";
                });

                endpoints.MapGroupDocsViewerApi(options =>
                {
                    options.ApiPath = "/viewer-api";
                });
            });
    }
}
```

Or, if you’re using [new program](https://docs.microsoft.com/en-us/dotnet/core/tutorials/top-level-templates) style with top-level statements, global using directives, and implicit using directives the Program.cs will be a bit shorter.

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services
        .AddGroupDocsViewerUI();

builder.Services
        .AddControllers()
        .AddGroupDocsViewerSelfHostApi(config =>
        {
            //Trial limitations https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/
            //Temporary license can be requested at https://purchase.groupdocs.com/temporary-license
            //config.SetLicensePath("c:\\licenses\\GroupDocs.Viewer.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'
        })
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
            options.APIEndpoint = "/viewer-api";
        });
        endpoints.MapGroupDocsViewerApi(options =>
        {
            options.ApiPath = "/viewer-api";
        });
    });

app.Run();
```

This code registers **/viewer** middleware that will serve SPA and **/viewer-api** middleware that will serve content for the UI to display.

 **Please note that Viewer does not create `Files` and `Cache` folders, please make sure to create `Files` and `Cache` folders manually before running the application.**

## UI

The UI is Angular SPA that is build upon [@groupdocs.examples.angular/viewer](https://www.npmjs.com/package/@groupdocs.examples.angular/viewer) package. You can change the path where the SPA will be available by setting `UIPath` property e.g.

```cs
endpoints.MapGroupDocsViewerUI(options =>
{
    options.UIPath = "/my-viewer-app";
});
```

### Changing UI language

The default UI language is English. The list of suported languages can be found in [Language.cs](src/GroupDocs.Viewer.UI.Core/Configuration/Language.cs) file. The default language, supported languages, and language menu visibility can be configured in `ConfigureServices` method:

```cs
services
    .AddGroupDocsViewerUI(config =>
    {
        config.SetDefaultLanguage(Language.French);
        config.SetSupportedLanguages(Language.English, Language.French, Language.Dutch);
        config.HideLanguageMenu();
    });
```

The SPA can also read language code from path or query string. In case path to the app contains language code e.g. `/fr/` or `/fr-fr/` the default language will be set to French. Or you can specify language code as a `lang` query string parameter e.g. `?lang=fr`.

## API

The API is used to serve content such as information about a document, document pages in HTML/PNG/JPG format and PDF file for printing. The API can be hosted in the same or a separate application. The following API implementations available at the moment:

- [GroupDocs.Viewer.UI.SelfHost.Api](https://www.nuget.org/packages/GroupDocs.Viewer.UI.SelfHost.Api)
- [GroupDocs.Viewer.UI.Cloud.Api](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Cloud.Api)

All the API implementations are extensions of `IMvcBuilder`:

### Self-host

Self-Host API uses [GroupDocs.Viewer for .NET](https://www.nuget.org/packages/groupdocs.viewer) to convert documents to HTML, PNG, JPG, and PDF. All the conversions are performed on the host where the application is running.

```cs
services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi();
```

GroupDocs.Viewer for .NET requires license to skip [trial limitations](https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/). A temporary license can be requested at [Get a Temporary License](https://purchase.groupdocs.com/temporary-license).

Use the following code to set a license:

```cs
services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config =>
    {
        config.SetLicensePath(".\GroupDocs.Viewer.lic");
    })
```

The sample application that shows how to use Self-Host Api can be found in the [samples](./samples) folder.

### Cloud

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

The sample application that shows how to use Cloud Api can be found in the [samples](./samples) folder.

#### Linux dependencies

When running Self-Host API on Linux the two groups of packages packages are required.
First group is `System.Drawing` implementaiton on Linux that is based on `libgdiplus`.
The second group is Microsoft fonts and tools for creating font cache.  

- [libgdiplus](https://packages.debian.org/sid/libgdiplus) - Interface library for `System.Drawing` of Mono.
- [libc6-dev](https://packages.debian.org/sid/libc6-dev) - GNU C Library: Development Libraries and Header Files.
- [libx11-dev](https://packages.debian.org/sid/libx11-dev) - X11 client-side library (development headers)
- [fontconfig](https://packages.debian.org/sid/fontconfig) - Generic font configuration library - support binaries.
- [ttf-mscorefonts-installer] - Microsoft TrueType core fonts like Arial, Times New Roman etc.

See how to install this packages in [Dockerfile](samples/GroupDocs.Viewer.UI.Sample/Dockerfile).

### API storage providers

Storage providers are used to read/write file from/to the storage. The storage provider is mandatory.

- [GroupDocs.Viewer.UI.Api.Local.Storage](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Api.Local.Storage)
- [GroupDocs.Viewer.UI.Api.Cloud.Storage](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Api.Cloud.Storage)
- [GroupDocs.Viewer.UI.Api.AzureBlob.Storage](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Api.AzureBlob.Storage)
- [GroupDocs.Viewer.UI.Api.AwsS3.Storage](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Api.AwsS3.Storage)


All the storage providers are extensions of `GroupDocsViewerUIApiBuilder`:

#### Local storage

To render files from your local drive use local file storage.

```cs
services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddLocalStorage("./Files");
```

#### Cloud storage

When rendering files using [GroupDocs Cloud](https://dashboard.groupdocs.cloud/) infrastructure it is reasonable to opt to cloud storage provider. GroupDocs Cloud storage supports number of 3d-party storages including Amazon S3, Google Drive and Cloud, Azure, Dropbox, Box, and FTP. To start using GroupDocs Cloud get your `Client ID` and `Client Secret` at <https://dashboard.groupdocs.cloud/applications>.

```cs
var clientId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
var clientSecret = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

services
    .AddControllers()
    .AddGroupDocsViewerCloudApi(config => 
        config
            .SetClientId(clientId)
            .SetClientSecret(clientSecret)
    )
    .AddCloudStorage(config => 
        config
            .SetClientId(clientId)
            .SetClientSecret(clientSecret)
    )
```

#### Azure Blob storage

You can also use [Azure Blob Storage](https://azure.microsoft.com/en-us/products/storage/blobs/) as a storage provider for Viewer.

- [GroupDocs.Viewer.UI.Api.AzureBlob.Storage](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Api.AzureBlob.Storage)

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

### Amazon S3 storage

Viewer also supports the [Amazon S3 Storage](https://aws.amazon.com/s3/) storage provider.

- [GroupDocs.Viewer.UI.Api.AwsS3.Storage](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Api.AwsS3.Storage)

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

### API cache providers

In case you would like to cache the output files produced by GroupDocs.Viewer you can use one of the cache providers:

- [GroupDocs.Viewer.UI.Api.Local.Cache](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Api.Local.Cache)
- [GroupDocs.Viewer.UI.Api.InMemory.Cache](https://www.nuget.org/packages/GroupDocs.Viewer.UI.Api.InMemory.Cache)

All the cache providers are extensions of `GroupDocsViewerUIApiBuilder`:

#### Local cache

```cs
services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddLocalStorage("./Files")
    .AddLocalCache("./Cache");
```

#### In-memory cache

```cs
services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddLocalStorage("./Files")
    .AddInMemoryCache();
```

## Contributing

Your contributions are welcome when you want to make the project better by adding new feature, improvement or a bug-fix.

1. Read and follow the [Don't push your pull requests](https://www.igvita.com/2011/12/19/dont-push-your-pull-requests/)
2. Follow the code guidelines and conventions.
3. Make sure to describe your pull requests well and add documentation.
