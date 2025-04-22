# UI for GroupDocs.Viewer for .NET (API)

`GroupDocs.Viewer.UI.Api` contains base types for implementing an API for `GroupDocs.Viewer.UI`.

There are two API implementations provided within this package:

* `GroupDocs.Viewer.UI.SelfHost.Api` - A self-hosted service that uses the 
  [GroupDocs.Viewer](https://www.nuget.org/packages/groupdocs.viewer) package.
* `GroupDocs.Viewer.UI.Cloud.Api` - Uses the cloud API provided by GroupDocs.

This readme includes information and usage examples for the features provided by this module.

## Features

This section lists the features provided by the module.

### Error Handling

Types that enable you to display custom error messages can be found in the [ErrorHandling](./ErrorHandling) folder.
The key type here is the [ErrorMessageProvider.cs](./ErrorHandling/ErrorMessageProvider.cs) interface, which defines a contract for providing user-friendly error messages based on exceptions.

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

**NOTE:** The service should be registered before you register the self-hosted or cloud API for it to take effect. By default, [ExceptionMessageProvider.cs](./ErrorHandling/ExceptionMessageProvider.cs) is registered.
This class provides a default implementation that returns the exception error message.

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

**NOTE:** This feature works only when rendering to HTML. The service should be registered before you register the self-hosted or cloud API for it to take effect. By default, [SearchTermResolver.cs](./SearchTermResolution/Implementation/SearchTermResolver.cs) is registered. This class provides a default implementation that returns an empty string.

