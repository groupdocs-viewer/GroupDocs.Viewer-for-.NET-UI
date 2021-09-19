using System.Linq;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Api.Local.Cache;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GroupDocsViewerUIApiBuilderExtensions
    {
        public static GroupDocsViewerUIApiBuilder AddLocalCache(this GroupDocsViewerUIApiBuilder builder, string cachePath)
        {
            ServiceDescriptor registeredServiceDescriptor = builder.Services.FirstOrDefault(
                s => s.ServiceType == typeof(IFileCache));

            if (registeredServiceDescriptor != null)
            {
                builder.Services.Remove(registeredServiceDescriptor);
            }

            // NOTE: Replace is used here as by default we've registered Noop cache 
            builder.Services.AddTransient<IFileCache>(_ => new LocalFileCache(cachePath));

            return builder;
        }
    }
}
