# UI for GroupDocs.Viewer for .NET (Samples)

This directory contains various sample projects demonstrating different ways to implement and use GroupDocs.Viewer.UI. Below is a description of each sample project:

## Basic Implementation Samples

### GroupDocs.Viewer.UI.Sample

A basic implementation sample showing how to integrate GroupDocs.Viewer UI into an ASP.NET Core application with default settings.

### GroupDocs.Viewer.UI.Sample.ViewAsPng

Demonstrates how to configure the viewer to render documents as PNG images instead of HTML.

### GroupDocs.Viewer.UI.Sample.StaticContentMode

Shows how to implement the viewer in static content mode, where all content is pre-generated and served as static files. The content is pre-generated and located in `Content` folder.

### GroupDocs.Viewer.UI.Sample.StaticContentMode.Generator

A utility application that generates static content for use with the static content mode sample.

## Cross-Platform and Framework Specific Samples

### GroupDocs.Viewer.UI.Sample.CrossPlatform

Demonstrates how to implement the viewer in a cross-platform environment, which is recommended option when running the API on Linux. The example contains Dockerfile that lists all the dependencies required to be installed on the system.

### GroupDocs.Viewer.UI.NetFramework.Sample

Shows how to integrate the viewer into a .NET Framework application, useful for legacy systems that haven't migrated to .NET Core.

### GroupDocs.Viewer.UI.SelfHosted.App.NetFramework.Sample

Demonstrates self-hosting the viewer in a .NET Framework application, providing an example of running the viewer in a traditional Windows environment.

## API Implementation Samples

### GroupDocs.Viewer.UI.SelfHost.Api.App.Sample

Shows how to separate UI from API into distinct services. Configured to run together with `GroupDocs.Viewer.UI.SelfHost.Api.Service.Sample`.

### GroupDocs.Viewer.UI.SelfHost.Api.Service.Sample

Demonstrates implementing the viewer API as a separate service. Configured to work together with `GroupDocs.Viewer.UI.SelfHost.Api.App.Sample`. 

The main difference here is pre-configured CORS policy that enables UI to call API from web browser.

### GroupDocs.Viewer.UI.Cloud.Api.Sample

Shows how to integrate the viewer with GroupDocs Cloud API. This sample requires credentials that you can obtain for free at https://dashboard.groupdocs.cloud/ (registration required).

## Caching and Storage Samples

### GroupDocs.Viewer.UI.Sample.CustomCacheProvider

Demonstrates how to implement a custom cache provider for the viewer.

### GroupDocs.Viewer.UI.CacheFactory.Sample

This example demonstrates how to pre-generate cache before the file is opened using UI. It shows how to implement background processing of documents to improve user experience.

## Getting Started

To run any of these samples:

1. Ensure you have the required .NET SDK installed (.NET 6 or .NET 8 depending on example)
2. Navigate to the sample project(s) directory
3. Run `dotnet restore` to restore dependencies
4. Run `dotnet run` to start the application

## Prerequisites

- .NET 6.0 or .NET 8 SDK
- License file for production use 
- Keys for Cloud samples

## Notes

- In case you would like to configure the specific sample, check `Program.cs` file
- `GroupDocs.Viewer.UI.Sample` and `GroupDocs.Viewer.UI.Sample.CrossPlatform` provide Dockerfile
- Please let us know if you can't find the example you're looking for by posting an issue