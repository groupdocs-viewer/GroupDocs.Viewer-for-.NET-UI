# GroupDocs.Viewer.UI.Cloud.Api

`GroupDocs.Viewer.UI.Cloud.Api` is a cloud-based API implementation for `GroupDocs.Viewer.UI` that enables document viewing using GroupDocs.Viewer Cloud API. It provides seamless integration with GroupDocs.Viewer Cloud services, allowing you to view documents in various formats.

## Installation

To use GroupDocs.Viewer.UI.Cloud.Api in your ASP.NET Core project:

1. Add the required package to your project:

```bash
dotnet add package GroupDocs.Viewer.UI.Cloud.Api
```

2. Configure the Cloud API in your `Startup` class:

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddGroupDocsViewerUI();

builder.Services
    .AddControllers()
    .AddGroupDocsViewerCloudApi(config =>
    {
        config.SetApiEndpoint("https://api.groupdocs.cloud/v2.0/");
        config.SetClientId("your-client-id");
        config.SetClientSecret("your-client-secret");
        config.SetStorageName("your-storage-name");

        config.SetSaveOutput(true); // Set this to `true` to save output in the cloud storage.
        config.SetOutputFolderPath("viewer"); // Set the folder path where to store the output in the cloud storage.
    })
    .AddLocalStorage("./Files");

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

The Cloud API implementation provides the following configuration options:

### Required Settings
- `ClientId` (Required): Your GroupDocs.Viewer Cloud API client ID. Get it from [GroupDocs Cloud Dashboard](https://dashboard.groupdocs.cloud/applications).
- `ClientSecret` (Required): Your GroupDocs.Viewer Cloud API client secret. Get it from [GroupDocs Cloud Dashboard](https://dashboard.groupdocs.cloud/applications).

### Optional Settings
- `ApiEndpoint` (Optional): The GroupDocs.Viewer Cloud API endpoint. Default is "https://api.groupdocs.cloud/v2.0/".
- `StorageName` (Optional): The name of your cloud storage. Find available storages at [GroupDocs Cloud Dashboard](https://dashboard.groupdocs.cloud/storages).
- `SaveOutput` (Optional): Whether to save rendered output in cloud storage. Default is `false`.
- `OutputFolderPath` (Optional): The folder path where rendered output will be stored. Default is "viewer".
- `ViewerType` (Optional): The type of viewer to use. Options include:
  - `HtmlWithEmbeddedResources` (default)
  - `HtmlWithExternalResources`
  - `Png`
  - `Jpg`

## Performance Considerations

When using GroupDocs.Viewer.UI.Cloud.Api:

1. Network latency affects rendering performance
2. Cloud storage operations impact response times
3. Best suited for:
   - Applications requiring cloud-based document viewing
   - Distributed environments
   - Scenarios where local processing is not preferred
4. Performance depends on:
   - Network connection quality
   - Cloud API response times
   - Document size and complexity
   - Chosen viewer type

## Security Considerations

When using GroupDocs.Viewer.UI.Cloud.Api:

1. Keep your client credentials secure
2. Use HTTPS for all API communications
3. Implement proper access control
4. Monitor API usage and quotas
5. Consider implementing rate limiting
6. Regularly rotate client credentials

## License

This project is licensed under the MIT License - see the [LICENSE.txt](../../LICENSE.txt) file for details. 