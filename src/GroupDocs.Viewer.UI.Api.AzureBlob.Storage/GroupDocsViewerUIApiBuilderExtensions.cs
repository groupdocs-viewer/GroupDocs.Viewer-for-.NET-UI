using System;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Api.AzureBlob.Storage;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GroupDocsViewerUIApiBuilderExtensions
    {
        public static GroupDocsViewerUIApiBuilder AddAzureStorage(this GroupDocsViewerUIApiBuilder builder, Action<AzureBlobOptions> setupOptions)
        {
            var options = new AzureBlobOptions();

            setupOptions?.Invoke(options);

            builder.Services.AddTransient<IFileStorage>(_ => new AzureBlobStorage(options));

            return builder;
        }
    }
}
