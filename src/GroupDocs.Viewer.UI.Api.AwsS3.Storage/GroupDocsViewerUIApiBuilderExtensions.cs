using System;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Api.AwsS3.Storage;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    // ReSharper disable once InconsistentNaming
    public static class GroupDocsViewerUIApiBuilderExtensions
    {
        public static GroupDocsViewerUIApiBuilder AddAwsS3Storage(
            this GroupDocsViewerUIApiBuilder builder, Action<AwsS3Options> setupOptions)
        {
            var options = new AwsS3Options();
            setupOptions?.Invoke(options);

            builder.Services.AddTransient<IFileStorage>(_ => 
                new AwsS3FileStorage(options));

            return builder;
        }
    }
}
