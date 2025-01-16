using GroupDocs.Viewer.UI.Api.Controllers;
using GroupDocs.Viewer.UI.Api.Utils;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MvcBuilderExtensions
    {
        public static GroupDocsViewerUIApiBuilder AddGroupDocsViewerSelfHostApi(this IMvcBuilder builder,
            Action<Config> setupConfig = null)
        {
            // GroupDocs.Viewer API Registration
            builder.PartManager.ApplicationParts.Add(new AssemblyPart(
                Assembly.GetAssembly(typeof(ViewerController)) ?? throw new DllNotFoundException(nameof(ViewerController))));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddTransient<IApiUrlBuilder, ApiUrlBuilder>();
            return ServiceCollectionBuilderExtensions.AddGroupDocsViewerServices(builder.Services, setupConfig);
        }
    }
}
