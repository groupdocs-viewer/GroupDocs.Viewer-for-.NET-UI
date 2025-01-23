# GroupDocs.Viewer.UI Self-Hosted App (.NET Framework)

This document provides an overview and setup guide for the GroupDocs.Viewer.UI Self-Hosted App built with .NET Framework. The sample project integrates the GroupDocs.Viewer UI with a backend API and showcases how to view various document types.

## Features
- **Document Viewing**: Render documents as HTML with embedded resources.
- **Local File and Cache Management**: Store documents and cache locally in the application.
- **Customizable API Path**: Configure API endpoints for the Viewer.
- **Embedded UI Integration**: Provides a user interface for document viewing.

## Prerequisites
- .NET Framework 4.6.2 or higher.
- Visual Studio 2019 or later.
- GroupDocs.Viewer library and Viewer.UI components.
- A valid GroupDocs.Viewer license (optional for non-trial limitations).

## Project Structure
The sample project includes the following:

1. **Global.asax**: Handles application lifecycle events and sets up the Viewer API and UI.
2. **App_Data**:
   - `Files`: Directory to store uploaded or local files.
   - `Cache`: Directory for caching document data.
3. **Routes**: Configures API and UI routes.

## Setting Up the Application

### 1. Register Areas and Routes
In `Application_Start`, the `AreaRegistration` and `RouteConfig` are registered to set up the required routing for MVC and API endpoints.

```csharp
AreaRegistration.RegisterAllAreas();
RouteConfig.RegisterRoutes(RouteTable.Routes);
```

### 2. Configure GroupDocs.Viewer Services
Use `ServiceCollection` to register GroupDocs.Viewer services, including file storage, caching, and Viewer type.

```csharp
var serviceCollection = new ServiceCollection();
serviceCollection.AddGroupDocsViewerSelfHostApi(viewConfig =>
{
    viewConfig.SetViewerType(ViewerType.HtmlWithEmbeddedResources);
})
.AddLocalStorage(HttpContext.Current.Server.MapPath("~/App_Data/Files"))
.AddLocalCache(HttpContext.Current.Server.MapPath("~/App_Data/Cache"));

var serviceProvider = serviceCollection.BuildServiceProvider();
```

### 3. Set Up Viewer API Endpoints
Configure Viewer API endpoints using `ViewerApiEndpointRouteBuilder`:

```csharp
GlobalConfiguration.Configure(config =>
{
    config.DependencyResolver = new ViewerDependencyResolver(serviceProvider);
    ViewerApiEndpointRouteBuilder.RegisterViewerApi(config, options =>
    {
        options.ApiPath = "/viewer-api";
    });
});
```

### 4. Configure the Viewer UI
Define the UI path and API endpoint for the Viewer UI:

```csharp
ViewerUIConfig viewerConfig = new ViewerUIConfig
{
    UIPath = "/viewer",
    ApiEndpoint = "/viewer-api"
};
_viewer = ViewerUI.Configure(viewerConfig);
```

### 5. Handle UI Requests
Intercept GET requests for the Viewer UI:

```csharp
protected void Application_BeginRequest(object sender, EventArgs e)
{
    if (HttpContext.Current.Request.RequestType == "GET")
    {
        _viewer.HandleRequest(HttpContext.Current);
    }
}
```

## Licensing
To remove trial limitations, set up a valid license by specifying the path to the license file or an environment variable.

```csharp
viewConfig.SetLicensePath("c:\\licenses\\GroupDocs.Viewer.lic");
```

Alternatively, request a temporary license from [GroupDocs Temporary License](https://purchase.groupdocs.com/temporary-license).

## Running the Application
1. Open the project in Visual Studio.
2. Build the solution to restore dependencies and compile the project.
3. Run the application. The default Viewer API endpoint is `/viewer-api`, and the UI is accessible at `/viewer`.

## Example API Requests
- **Render Document**: `GET /viewer-api/view?file={filePath}`
- **Load Thumbnails**: `GET /viewer-api/thumbnail/{filePath}`

## Troubleshooting
- Ensure the `App_Data/Files` and `App_Data/Cache` directories have proper read/write permissions.
- Check the API endpoint configuration in `ViewerApiEndpointRouteBuilder` if the Viewer UI does not load.
- Verify that the required NuGet packages are restored and referenced in the project.

## Resources
- [GroupDocs.Viewer Documentation](https://docs.groupdocs.com/viewer/net/)
- [GroupDocs.Viewer Examples](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET)
- [GroupDocs Support](https://forum.groupdocs.com/)

