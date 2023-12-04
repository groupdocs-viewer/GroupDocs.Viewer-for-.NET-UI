# UI for GroupDocs.Viewer for .NET

![Build Packages](https://github.com/groupdocs-viewer/GroupDocs.Viewer-for-.NET-UI/actions/workflows/build_packages.yml/badge.svg)
![Nuget](https://img.shields.io/nuget/v/groupdocs.viewer.ui?label=GroupDocs.Viewer.UI)
![Nuget](https://img.shields.io/nuget/dt/GroupDocs.Viewer.UI?label=GroupDocs.Viewer.UI)

`GroupDocs.Viewer.UI` project contains Angular applicaiton in the [client](client) folder. 

This pacakge may be used independently from the oter packages in the project and can be added as a middleware to ASP.NET Core web application using extension method `AddGroupDocsViewerUI`. To map the middleware to a specific endpoint use `MapGroupDocsViewerUI` extension method.

## Getting started

To get started, add the pacakge to your project:

```PowerShell
dotnet add package GroupDocs.Viewer.UI
```

Add configuration to your `Startup` class:

```cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddGroupDocsViewerUI();
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
            });
    }
}
```

Or, if you’re using [new program](https://docs.microsoft.com/en-us/dotnet/core/tutorials/top-level-templates) style with top-level statements, global using directives, and implicit using directives the Program.cs will be a bit shorter.

```cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGroupDocsViewerUI();

var app = builder.Build();

app
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapGroupDocsViewerUI(options =>
        {
            options.UIPath = "/viewer";
            options.APIEndpoint = "https://example.com/viewer-api";
        });
    });

app.Run();
```

`MapGroupDocsViewerUI` maps `GroupDocs.Viewer.UI` middleware to `/viewer` endpoint. 

Angular app requires API. The API is used to serve content such as information about a document, document pages in HTML/PNG/JPG format and PDF file for printing. The API can be hosted in the same or a separate application. The following API implementations available at the moment:

- [SelfHost API](../GroupDocs.Viewer.UI.SelfHost.Api/) - enables you to process documents on you environment. The API is based on [GroupDocs.Viewer for .NET](https://nuget.org/packages/groupdocs.viewer) document processing API.
- [Cloud API](../GroupDocs.Viewer.UI.Cloud.Api/) - using this API you can offload processing to our [Cloud API](https://products.groupdocs.cloud/viewer/net/).

## Configuration options

There are two sets of options that you can use to configure UI.  The second one is used to controll API behaviour.

### Options that control UI appearance

Using these options you can hide UI controls or change controls appearance.

#### Set UI language

The default UI language is English. The list of suported languages can be found in [Language.cs](src/GroupDocs.Viewer.UI.Core/Configuration/Language.cs) file. The default language, supported languages, and language menu visibility can be configured in `ConfigureServices` method:

```cs
services
    .AddGroupDocsViewerUI(config =>
    {
        config.SetDefaultLanguage(Language.French);
        config.SetSupportedLanguages(Language.English, Language.French, Language.Dutch);
        //config.HideLanguageMenu();
    });
```

The SPA can also read language code from path or query string. In case path to the app contains language code e.g. `/fr/` or `/fr-fr/` the default language will be set to French. Or you can specify language code as a `lang` query string parameter e.g. `?lang=fr`.

#### Hide navigation buttons

By default, navigation control is visible. You can hide it using `HidePageSelectorControl` method.

```cs
services
    .AddGroupDocsViewerUI(config =>
    {
        config.HidePageSelectorControl();
    });
```

#### Hide thumbnails

By default, thumbnails panel is visible. You can hide it using `HideThumbnailsControl` method.

```cs
services
    .AddGroupDocsViewerUI(config =>
    {
        config.HideThumbnailsControl();
    });
```

#### Hide Browse files button

To hide Browse files button and disable browsing through files use `DisableFileBrowsing` method. By default, Browse files button is visible. 

```cs
services
    .AddGroupDocsViewerUI(config =>
    {
        config.DisableFileBrowsing();
    });
```

#### Hide Zoom buttons

To hide Zoom buttons on the controlls panel use `HideZoomButton` method. By default, Zoom buttons are visible.

```cs
services
    .AddGroupDocsViewerUI(config =>
    {
        config.HideZoomButton();
    });
```

#### Hide Search button

To hide Search button on the controlls panel use `HideSearchControl` method. By default, Search button is visible.

```cs
services
    .AddGroupDocsViewerUI(config =>
    {
        config.HideSearchControl();
    });
```

#### Hide Tool bar

To completely hide the Tool bar use `HideToolBar` method. By default, toolbar is visible.

```cs
services
    .AddGroupDocsViewerUI(config =>
    {
        config.HideToolBar();
    });
```

#### Hide Rotate buttons

To completely hide the Tool bar use `HidePageRotationControl` method. By default, Rotate buttons are visible. 

```cs
services
    .AddGroupDocsViewerUI(config =>
    {
        config.HidePageRotationControl();
    });
```

#### Hide Print button
 
To hide Print button use `DisablePrint` method. By default, Print button is visible.

```cs
services
    .AddGroupDocsViewerUI(config =>
    {
        config.DisableRightClick();
    });
```

#### Disable context menu
 
To disable using a context menu use `DisableRightClick` method.

```cs
services
    .AddGroupDocsViewerUI(config =>
    {
        config.DisableRightClick();
    });
```

#### Default file to open

By default we do not load any document, but you can configure the UI to load 

```cs
services
    .AddGroupDocsViewerUI(config =>
    {
        config.SetDefaultLanguage(Language.French);
        config.SetSupportedLanguages(Language.English, Language.French, Language.Dutch);
        //config.HideLanguageMenu();
    });
```

The SPA can also read language code from path or query string. In case path to the app contains language code e.g. `/fr/` or `/fr-fr/` the default language will be set to French. Or you can specify language code as a `lang` query string parameter e.g. `?lang=fr`.


### Set count pages to render

The `SetPreloadPageCount` method enables you to control rendering of a document. The default value is `3`, so on the first call to the API the first `3` pages are going to be rendered. When you scroll down the next `3` pages and so on. To render all of the pages on the first call set this option to `0`.

```cs
services
    .AddGroupDocsViewerUI(config =>
    {
        config.SetPreloadPageCount(0); // render all the pages on the first call to API
    });
```

## Contributing

Your contributions are welcome when you want to make the project better by adding new feature, improvement or a bug-fix.

1. Read and follow the [Don't push your pull requests](https://www.igvita.com/2011/12/19/dont-push-your-pull-requests/)
2. Follow the code guidelines and conventions.
3. Make sure to describe your pull requests well and add documentation.
