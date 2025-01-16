using GroupDocs.Viewer.UI.Api;
using GroupDocs.Viewer.UI.Api.Shared.Controllers;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Caching;
using GroupDocs.Viewer.UI.Core.Caching.Implementation;
using GroupDocs.Viewer.UI.Core.FileCaching;
using GroupDocs.Viewer.UI.Core.PageFormatting;
using GroupDocs.Viewer.UI.SelfHost.Api;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using GroupDocs.Viewer.UI.SelfHost.Api.InternalCaching;
using GroupDocs.Viewer.UI.SelfHost.Api.Licensing;
using GroupDocs.Viewer.UI.SelfHost.Api.Viewers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionBuilderExtensions
{
    internal static GroupDocsViewerUIApiBuilder AddGroupDocsViewerServices(IServiceCollection serviceCollection,
        Action<Config> setupConfig = null)
    {
        var config = new Config();
        setupConfig?.Invoke(config);
        serviceCollection.AddSingleton(config);

        serviceCollection.AddTransient<IViewerService, ViewerService>();

        serviceCollection.AddSingleton<IViewerLicenseManager, ViewerLicenseManager>();
        serviceCollection.AddTransient<IFileCache, NoopFileCache>();
        serviceCollection.AddTransient<IAsyncLock, AsyncDuplicateLock>();
        serviceCollection.TryAddSingleton<IFileNameResolver, FilePathFileNameResolver>();
        serviceCollection.TryAddSingleton<IFileTypeResolver, FileExtensionFileTypeResolver>();
        serviceCollection.TryAddSingleton<IPageFormatter, NoopPageFormatter>();
        serviceCollection.TryAddSingleton<ISearchTermResolver, SearchTermResolver>();
        serviceCollection.TryAddSingleton<IUIConfigProvider, UIConfigProvider>();

        if (config.InternalCacheOptions.IsCacheEnabled)
        {
            serviceCollection.TryAddSingleton<IMemoryCache, MemoryCache>();
            serviceCollection.AddOptions<InternalCacheOptions>();
            serviceCollection.TryAddSingleton<IInternalCache>(factory =>
            {
                var memoryCache = factory.GetRequiredService<IMemoryCache>();
                return new InMemoryInternalCache(memoryCache, config.InternalCacheOptions);
            });
        }
        else
        {
            serviceCollection.TryAddSingleton<IInternalCache, NoopInternalCache>();
        }

        serviceCollection.AddTransient<HtmlWithEmbeddedResourcesViewer>();
        serviceCollection.AddTransient<HtmlWithExternalResourcesViewer>();
        serviceCollection.AddTransient<PngViewer>();
        serviceCollection.AddTransient<JpgViewer>();
        serviceCollection.AddTransient<IViewer>(factory =>
        {
            IViewer viewer;
            switch (config.ViewerType)
            {
                case ViewerType.HtmlWithExternalResources:
                    viewer = factory.GetRequiredService<HtmlWithExternalResourcesViewer>();
                    break;
                case ViewerType.Png:
                    viewer = factory.GetRequiredService<PngViewer>();
                    break;
                case ViewerType.Jpg:
                    viewer = factory.GetRequiredService<JpgViewer>();
                    break;
                default:
                    viewer = factory.GetRequiredService<HtmlWithEmbeddedResourcesViewer>();
                    break;
            }

            return new CachingViewer(
                viewer,
                factory.GetRequiredService<IFileCache>(),
                factory.GetRequiredService<IAsyncLock>()
            );
        });

        return new GroupDocsViewerUIApiBuilder(serviceCollection);
    }
}