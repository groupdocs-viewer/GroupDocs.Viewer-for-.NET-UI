# GroupDocs.Viewer.UI.Api.Cloud.Storage

`GroupDocs.Viewer.UI.Api.Cloud.Storage` is a cloud storage implementation that can be used with `GroupDocs.Viewer.UI.Api`. It provides seamless integration with [GroupDocs.Cloud](https://groupdocs.cloud/) Storage for storing and retrieving documents in your `GroupDocs.Viewer.UI` application.

This storage implementation is primarily designed to work with `GroupDocs.Viewer.UI.Cloud.Api`, but it can also be used with `GroupDocs.Viewer.UI.SelfHost.Api` if you need to store files in GroupDocs.Cloud Storage.

## Installation

To use Cloud Storage in your ASP.NET Core project:

1. Add the required package to your project:

```bash
dotnet add package GroupDocs.Viewer.UI.Api.Cloud.Storage
```

2. Configure Cloud Storage in your `Startup` class:

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGroupDocsViewerUI();

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddCloudStorage(config =>
    {
        config.SetApiEndpoint("https://api.groupdocs.cloud/v2.0/");
        config.SetClientId("your-client-id");
        config.SetClientSecret("your-client-secret");
        config.SetStorageName("your-storage-name"); // Optional, uses default storage if not set
    });

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

await app.RunAsync();
```

## Configuration Options

The Cloud Storage implementation supports the following configuration options:

- `ApiEndpoint` (Required): The GroupDocs Cloud API endpoint (default: "https://api.groupdocs.cloud/v2.0/").
- `ClientId` (Required): Your GroupDocs Cloud client ID, obtained from [GroupDocs Cloud Dashboard](https://dashboard.groupdocs.cloud/applications).
- `ClientSecret` (Required): Your GroupDocs Cloud client secret, obtained from [GroupDocs Cloud Dashboard](https://dashboard.groupdocs.cloud/applications).
- `StorageName` (Optional): The name of your storage in GroupDocs Cloud. If not set, the default storage will be used.

## Getting Credentials

To obtain your client ID and client secret:

1. Go to [GroupDocs Cloud Dashboard](https://dashboard.groupdocs.cloud/applications)
2. Create a new application or select an existing one
3. Copy the client ID and client secret from the application details

## License

This project is licensed under the MIT License - see the [LICENSE.txt](../../LICENSE.txt) file for details. 