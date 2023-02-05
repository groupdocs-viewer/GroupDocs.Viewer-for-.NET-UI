using System;
using System.Reflection;
using AsyncKeyedLock;
using GroupDocs.Viewer.UI.Api;
using GroupDocs.Viewer.UI.Api.Controllers;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Caching;
using GroupDocs.Viewer.UI.Core.FileCaching;
using GroupDocs.Viewer.UI.Core.PageFormatting;
using GroupDocs.Viewer.UI.SelfHost.Api;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using GroupDocs.Viewer.UI.SelfHost.Api.Licensing;
using GroupDocs.Viewer.UI.SelfHost.Api.Viewers;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcBuilderExtensions
    {
        public static GroupDocsViewerUIApiBuilder AddGroupDocsViewerSelfHostApi(this IMvcBuilder builder, 
            Action<Config> setupConfig = null)
        {
            var config = new Config();
            setupConfig?.Invoke(config);

            // GroupDocs.Viewer API Registration
            builder.PartManager.ApplicationParts.Add(new AssemblyPart(
                Assembly.GetAssembly(typeof(ViewerController))));

            builder.Services
                .AddOptions<Config>()
                .Configure<IConfiguration>((settings, configuration) =>
                {
                    configuration.BindSelfHostApiSettings(settings);
                    setupConfig?.Invoke(settings);
                });

            builder.Services.AddSingleton<IViewerLicenser, ViewerLicenser>();
            builder.Services.AddTransient<IFileCache, NoopFileCache>();
            builder.Services.AddSingleton(new AsyncKeyedLocker<string>(o =>
            {
                o.PoolSize = 20;
                o.PoolInitialFill = 1;
            }));
            builder.Services.TryAddSingleton<IFileNameResolver, FilePathFileNameResolver>();
            builder.Services.TryAddSingleton<IFileTypeResolver, FileExtensionFileTypeResolver>();
            builder.Services.TryAddSingleton<IPageFormatter, NoopPageFormatter>();
           
            builder.Services.AddTransient<HtmlWithEmbeddedResourcesViewer>();
            builder.Services.AddTransient<HtmlWithExternalResourcesViewer>();
            builder.Services.AddTransient<PngViewer>();
            builder.Services.AddTransient<JpgViewer>();
            builder.Services.AddTransient<IViewer>(factory =>
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
                    factory.GetRequiredService<AsyncKeyedLocker<string>>()
                );
            });

            return new GroupDocsViewerUIApiBuilder(builder.Services);
        }
    }
}
