using GroupDocs.Viewer.UI.Api.Local.Storage;
using GroupDocs.Viewer.UI.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class GroupDocsViewerUIApiBuilderExtensions
    {
        public static GroupDocsViewerUIApiBuilder AddLocalStorage(
            this GroupDocsViewerUIApiBuilder builder, string storagePath)
        {
            builder.Services.AddTransient<IFileStorage>(_ =>
                new LocalFileStorage(storagePath));

            return builder;
        }
    }
}
