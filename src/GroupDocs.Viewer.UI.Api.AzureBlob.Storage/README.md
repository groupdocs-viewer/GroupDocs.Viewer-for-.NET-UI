# GroupDocs.Viewer.UI.Api.AzureBlob.Storage

`GroupDocs.Viewer.UI.Api.AzureBlob.Storage` is a Microsoft Azure Blob Storage implementation that can be used with `GroupDocs.Viewer.UI.Api`. It provides seamless integration with Azure Blob Storage for storing and retrieving documents in your `GroupDocs.Viewer.UI` application.

## Installation

To use Azure Blob Storage in your ASP.NET Core project:

1. Add the required package to your project:

```bash
dotnet add package GroupDocs.Viewer.UI.Api.AzureBlob.Storage
```

2. Configure Azure Blob Storage in your `Startup` class:

```cs
using Azure.Storage.Blobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGroupDocsViewerUI();

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi()
    .AddAzureBlobStorage(options =>
    {
        options.AccountName = "your-storage-account-name";
        options.AccountKey = "your-storage-account-key";
        options.ContainerName = "your-container-name";
        options.ClientOptions = new BlobClientOptions
        {
            // Configure additional Azure Blob client settings if needed
        };
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

The Azure Blob Storage implementation supports the following configuration options:

- `AccountName` (Required): The name of your Azure Storage account.
- `AccountKey` (Required): The access key for your Azure Storage account.
- `ContainerName` (Required): The name of the blob container in your storage account.
- `ClientOptions` (Optional): Additional configuration for the Azure Blob client.

## Azure Storage Connection String

Alternatively, you can use a connection string instead of separate account name and key:

```cs
options.ConnectionString = "DefaultEndpointsProtocol=https;AccountName=your-storage-account-name;AccountKey=your-storage-account-key;EndpointSuffix=core.windows.net";
```

## License

This project is licensed under the MIT License - see the [LICENSE.txt](../../LICENSE.txt) file for details. 