# UI for GroupDocs.Viewer for .NET (Samples)

This directory contains various sample projects demonstrating different ways to implement and use GroupDocs.Viewer.UI. Below is a description of each sample project.

## Quick reference

| Sample | Hosting | Storage | Cache | Rendering | Demonstrates |
|---|---|---|---|---|---|
| [`GroupDocs.Viewer.UI.Sample`](GroupDocs.Viewer.UI.Sample/) | SelfHost.Api | Local | Local | HTML | Default integration; ships a Dockerfile |
| [`GroupDocs.Viewer.UI.Sample.ViewAsPng`](GroupDocs.Viewer.UI.Sample.ViewAsPng/) | SelfHost.Api | Local | Local | PNG | Rendering documents as PNG |
| [`GroupDocs.Viewer.UI.Sample.AzureCustomDomain`](GroupDocs.Viewer.UI.Sample.AzureCustomDomain/) | SelfHost.Api | Local | Local | JPG | Azure App Service with custom domains; relative URLs; Docker Compose + Nginx |
| [`GroupDocs.Viewer.UI.Sample.StaticContentMode`](GroupDocs.Viewer.UI.Sample.StaticContentMode/) | UI only (no API) | — | — | HTML | Pre-generated static content served from `Content/` |
| [`GroupDocs.Viewer.UI.Sample.StaticContentMode.Generator`](GroupDocs.Viewer.UI.Sample.StaticContentMode.Generator/) | Console | Local | — | HTML | Generates the static content used by the sample above |
| [`GroupDocs.Viewer.UI.Sample.CustomBranding`](GroupDocs.Viewer.UI.Sample.CustomBranding/) | SelfHost.Api | Local | Local | HTML | White-labeling: logo, title, and custom stylesheet |
| [`GroupDocs.Viewer.UI.SelfHost.Api.App.Sample`](GroupDocs.Viewer.UI.SelfHost.Api.App.Sample/) | UI only (talks to remote API) | — | — | HTML | UI ↔ API split — pairs with `SelfHost.Api.Service.Sample` |
| [`GroupDocs.Viewer.UI.SelfHost.Api.Service.Sample`](GroupDocs.Viewer.UI.SelfHost.Api.Service.Sample/) | SelfHost.Api (API only, CORS) | Local | Local | HTML | API split — pairs with `SelfHost.Api.App.Sample` |
| [`GroupDocs.Viewer.UI.Cloud.Api.Sample`](GroupDocs.Viewer.UI.Cloud.Api.Sample/) | Cloud.Api | Local | Local | HTML | GroupDocs Cloud rendering (needs credentials) |
| [`GroupDocs.Viewer.UI.Sample.CustomCacheProvider`](GroupDocs.Viewer.UI.Sample.CustomCacheProvider/) | SelfHost.Api | Local | **Custom `IFileCache`** | HTML | Implementing a custom cache provider |
| [`GroupDocs.Viewer.UI.Sample.CustomFileStorage`](GroupDocs.Viewer.UI.Sample.CustomFileStorage/) | SelfHost.Api | **Custom `IFileStorage`** | Local | HTML | Implementing a custom storage provider (DB/blob-style) |
| [`GroupDocs.Viewer.UI.CacheFactory.Sample`](GroupDocs.Viewer.UI.CacheFactory.Sample/) | Console | Local | Local | HTML | Pre-warming cache before UI access |

## Basic Implementation Samples

### GroupDocs.Viewer.UI.Sample

A basic implementation sample showing how to integrate GroupDocs.Viewer UI into an ASP.NET Core application with default settings.

### GroupDocs.Viewer.UI.Sample.ViewAsPng

Demonstrates how to configure the viewer to render documents as PNG images instead of HTML.

### GroupDocs.Viewer.UI.Sample.AzureCustomDomain

Demonstrates how to deploy and run GroupDocs.Viewer.UI on Azure App Service with custom domains. The sample is configured to generate relative URLs by default, ensuring that custom domains work correctly without requiring additional configuration. Includes Docker Compose setup with Nginx to simulate Azure custom domain scenarios locally, and provides step-by-step instructions for deploying to Azure App Service.

### GroupDocs.Viewer.UI.Sample.StaticContentMode

Shows how to implement the viewer in static content mode, where all content is pre-generated and served as static files. The content is pre-generated and located in `Content` folder.

### GroupDocs.Viewer.UI.Sample.StaticContentMode.Generator

A utility application that generates static content for use with the static content mode sample.

### GroupDocs.Viewer.UI.Sample.CustomBranding

Demonstrates how to white-label the viewer with custom colors, logo, and app title. Uses `SetLogoImage()`, `SetLogoText()`, `AddCustomStylesheet()`, and `UITitle` to replace default branding with a custom teal color scheme and "Acme Corp" branding.

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

### GroupDocs.Viewer.UI.Sample.CustomFileStorage

Demonstrates how to implement a custom `IFileStorage` for a database- or blob-backed store. The sample includes a `DatabaseFileStorage` class that simulates a SQL `VARBINARY(MAX)` column with an in-memory dictionary seeded from disk, and shows how to register the custom storage after `AddGroupDocsViewerSelfHostApi()`. Note that custom storages must implement `ReadFileStreamAsync` (required since 26.3.0).

### GroupDocs.Viewer.UI.CacheFactory.Sample

This example demonstrates how to pre-generate cache before the file is opened using UI. It shows how to implement background processing of documents to improve user experience.

## Getting Started

To run any of these samples:

1. Ensure you have the .NET 8.0 SDK installed
2. Navigate to the sample project(s) directory
3. Run `dotnet restore` to restore dependencies
4. Run `dotnet run` to start the application

## Prerequisites

- .NET 8.0 SDK
- License file for production use 
- Keys for Cloud samples

## Notes

- In case you would like to configure the specific sample, check `Program.cs` file
- `GroupDocs.Viewer.UI.Sample` and `GroupDocs.Viewer.UI.Sample.AzureCustomDomain` provide Dockerfile
- `GroupDocs.Viewer.UI.Sample.AzureCustomDomain` also includes Docker Compose with Nginx for testing custom domains locally
- Please let us know if you can't find the example you're looking for by posting an issue