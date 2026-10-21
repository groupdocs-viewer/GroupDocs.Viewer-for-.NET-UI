# UI for GroupDocs.Viewer for .NET (API)

`GroupDocs.Viewer.UI.Api` contains base types for implementing an API for `GroupDocs.Viewer.UI`.

There are two API implementations provided within this package:

* `GroupDocs.Viewer.UI.SelfHost.Api` - A self-hosted service that uses the 
  [GroupDocs.Viewer](https://www.nuget.org/packages/groupdocs.viewer) package.
* `GroupDocs.Viewer.UI.Cloud.Api` - Uses the cloud API provided by GroupDocs.

This readme includes information and usage examples for the features provided by this module.

## Features

This section lists the features provided by the module.

### Restricting Access

`MapGroupDocsViewerApi` returns an endpoint convention builder that applies conventions to all API endpoints. To require signed-in users, call `RequireAuthorization()` on it as well as on `MapGroupDocsViewerUI`:

```cs
app.MapGroupDocsViewerApi(options =>
{
    options.ApiPath = "/viewer-api";
}).RequireAuthorization();
```

Only the routes that `MapGroupDocsViewerApi` maps reach the viewer controller. Other conventional routes of your app, for example `MapDefaultControllerRoute()`, can't bypass these conventions. If you upgrade from 26.9.0 or earlier, see [Upgrade notes](#upgrade-notes).

See the [authorization sample app](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/tree/main/samples/GroupDocs.Viewer.UI.Sample.Authorization) for a complete example with sign-in and a role-based policy.

### Error Handling

Types that enable you to display custom error messages can be found in the [ErrorHandling](./ErrorHandling) folder.
The key type here is the [IErrorMessageProvider.cs](./ErrorHandling/IErrorMessageProvider.cs) interface, which defines a contract for providing user-friendly error messages based on exceptions.

The following code demonstrates how to implement a custom error message provider:

```cs
using GroupDocs.Viewer.UI.Api;

//...

public class MyErrorMessageProvider : IErrorMessageProvider
{
    public string GetErrorMessage(Exception exception, ErrorContext context)
    {
        return "This is my custom error message...";
    }
}
```

Once the custom error message provider is implemented, it can be registered during the application composition stage.

The following code snippet shows how to register a custom service as a singleton:

```cs
using GroupDocs.Viewer.UI.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IErrorMessageProvider, MyErrorMessageProvider>();
```

**NOTE:** The service should be registered before you register the self-hosted or cloud API for it to take effect. By default, [ExceptionMessageProvider.cs](./ErrorHandling/Implementation/ExceptionMessageProvider.cs) is registered.
This class provides a default implementation that returns the exception error message. File system errors (file not found, access denied, other I/O errors) get generic messages instead, because their messages contain absolute paths on the server.

Once a custom error message provider is registered, a popup with the custom error message will be displayed in case of an error.

![GroupDocs.Viewer.UI - Custom error message](https://raw.githubusercontent.com/groupdocs-viewer/groupdocs-viewer.github.io/master/resources/image/ui/custom-error-message.png)

### File Name Resolution

In case you're using identifiers instead of file names but you also want to show actual file name in UI you have to implement `IFileNameResolver` interface to map a file identifier to the file name.

The following code demonstrates how to implement a custom file name resolver:

```cs
using GroupDocs.Viewer.UI.Api;

//...

public class MyFileNameResolver : IFileNameResolver
{
    public Task<string> ResolveFileNameAsync(string file)
    {
        return Task.FromResult("quaterly-report.docx");
    }
}
```

Once the custom file name resolver is implemented, it can be registered during the application composition stage.

The following code snippet shows how to register a custom service as a singleton:

```cs
using GroupDocs.Viewer.UI.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IFileNameResolver, MyFileNameResolver>();
```

**NOTE:** The service should be registered before you register the self-hosted or cloud API for it to take effect. By default, [FilePathFileNameResolver.cs](./FileNameResolution/Implementation/FilePathFileNameResolver.cs) is registered.
This class provides a default implementation that returns the file name from the file path.

Once a custom file name resolver is registered, the resolved file name will be used in the UI and when downloading files.

### Search Term Resolution

To pass a search term that you would like to highlight automatically you can implement `ISearchTermResolver` interface. 

The following code demonstrates how to implement a custom search term resolver:

```cs
using GroupDocs.Viewer.UI.Api;

//...

public class MySearchTermResolver : ISearchTermResolver
{
    public Task<string> ResolveSearchTermAsync(string filepath)
    {
        return Task.FromResult("review");
    }
}
```

Once the custom search term resolver is implemented, it can be registered during the application composition stage.

The following code snippet shows how to register a custom service as a singleton:

```cs
using GroupDocs.Viewer.UI.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISearchTermResolver, MySearchTermResolver>();
```

![GroupDocs.Viewer.UI - Custom search term](https://raw.githubusercontent.com/groupdocs-viewer/groupdocs-viewer.github.io/master/resources/image/ui/custom-search-term.png)

**NOTE:** This feature works only when rendering to HTML and when `PreloadPages` is set to `0` which means all the pages are preloaded. The service should be registered before you register the self-hosted or cloud API for it to take effect. By default, [SearchTermResolver.cs](./SearchTermResolution/Implementation/SearchTermResolver.cs) is registered. This class provides a default implementation that returns an empty string.

### Controll Thumbnails Creation

Thumbnails are only required when rendering to HTML with embedded or external resources.
When rendering to JPG or PNG, the actual page image is used as a thumbnail for better performance.

To control thumbnail creation when rendering to HTML, set the `EnableThumbnails` property to `false`.
This hides the thumbnail panel and disables thumbnail generation by the API.

The following code demonstrates how to use this option without a UI:

```cs
builder.Services
    .AddOptions<Config>()
    .Configure<IConfiguration>((config, configuration) =>
    {
        config.EnableThumbnails = false;
    });

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config =>
//...
```

## Upgrade notes

### 26.9.1: only `MapGroupDocsViewerApi` routes reach the viewer API

To fix [#117](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/issues/117), the viewer controller now requires the route value `groupdocs-viewer-api=true` (`ApiNames.ROUTE_VALUE_KEY` and `ApiNames.ROUTE_VALUE`). Only the routes that `MapGroupDocsViewerApi` maps provide this value. In 26.9.0 and earlier, other conventional routes of the app also reached the controller. For example, the default route `{controller=Home}/{action=Index}/{id?}` from `MapDefaultControllerRoute()` exposed `/Viewer/GetPdf` and `/Viewer/ViewData`, and `RequireAuthorization()` on `MapGroupDocsViewerApi` did not apply to these requests.

Your app needs no changes if it calls the API only through the URLs under `ApiPath` that `MapGroupDocsViewerApi` maps, as all examples in this repository do. Otherwise, check this table:

| Your app | After the upgrade | What to do |
|---|---|---|
| Calls the API through a conventional route of the app, for example `/Viewer/GetPdf` or `/Viewer/ViewData` | 404 Not Found | Call the URLs under `ApiPath` instead, for example `/viewer-api/get-pdf` or `/viewer-api/view-data` |
| Maps its own conventional route to the viewer controller | 404 Not Found | Use `MapGroupDocsViewerApi` with `ApiPath`, or add the route value to the route defaults, see below |
| Reaches a subclass of `ViewerController` through a conventional route | 404 Not Found | Give the subclass an attribute route, `[Route("...")]`, or add the route value to the route defaults. Attribute routes are not affected |
| Generates API links with `Url.Action(...)` or `LinkGenerator.GetPathByAction(...)` | `null`, or a link such as `/Viewer/GetPdf` that returns 404 | Generate the links by route name, see below |

To keep your own conventional route to the viewer controller, add the route value to its defaults. Conventions on `MapGroupDocsViewerApi`, such as `RequireAuthorization()`, don't apply to your route, so add them to it as well:

```cs
using GroupDocs.Viewer.UI.Api;
using Microsoft.AspNetCore.Routing;

app.MapControllerRoute("my-viewer-api", "my-viewer-api/{action}",
    new RouteValueDictionary
    {
        ["controller"] = ApiNames.CONTROLLER_NAME,
        [ApiNames.ROUTE_VALUE_KEY] = ApiNames.ROUTE_VALUE,
    }).RequireAuthorization();
```

To generate a link to an API method, use its route name. The route names are the API method names in `ApiNames`. This works both before and after the upgrade:

```cs
var pdfUrl = Url.RouteUrl(ApiNames.API_METHOD_GET_PDF, new { file = "sample.docx" });
// /viewer-api/get-pdf?file=sample.docx
```
