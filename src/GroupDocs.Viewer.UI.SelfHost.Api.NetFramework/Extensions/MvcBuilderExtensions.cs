using GroupDocs.Viewer.UI.Api.NetFramework.Controllers;
using GroupDocs.Viewer.UI.Api.NetFramework.Utils;
using GroupDocs.Viewer.UI.Api.Utils;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using System;
using System.Web;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcBuilderExtensions
    {
        public static GroupDocsViewerUIApiBuilder AddGroupDocsViewerSelfHostApi(this IServiceCollection serviceCollection,
            Action<Config> setupConfig = null)
        {
            serviceCollection ??= new ServiceCollection();
            serviceCollection.AddTransient<ViewerController>();
            serviceCollection.AddTransient<HttpContextBase>(_ => new HttpContextWrapper(HttpContext.Current));
            serviceCollection.AddTransient<IApiUrlBuilder, ApiUrlBuilder>();
            serviceCollection.AddSingleton(new GroupDocs.Viewer.UI.Core.Configuration.Config());
            return ServiceCollectionBuilderExtensions.AddGroupDocsViewerServices(serviceCollection, setupConfig);
        }
    }
}
