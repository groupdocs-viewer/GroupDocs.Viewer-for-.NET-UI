using System.IO;
using System.Threading.Tasks;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using GroupDocs.Viewer.UI.SelfHost.Api.InternalCaching;
using GroupDocs.Viewer.UI.SelfHost.Api.Licensing;
using GroupDocs.Viewer.UI.SelfHost.Api.Viewers.Extensions;
using Microsoft.Extensions.Options;
using Page = GroupDocs.Viewer.UI.Core.Entities.Page;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Viewers
{
    public class HtmlWithEmbeddedResourcesViewer : BaseViewer
    {
        private readonly Config _config;

        public HtmlWithEmbeddedResourcesViewer(
            IOptions<Config> config,
            IAsyncLock asyncLock,
            IViewerLicenser licenser, 
            IInternalCache viewerCache,
            IFileStorage fileStorage, 
            IFileTypeResolver fileTypeResolver, 
            IPageFormatter pageFormatter) 
            : base(config, asyncLock, licenser, viewerCache, fileStorage, fileTypeResolver, pageFormatter)
        {
            _config = config.Value;
        }

        public override string PageExtension => HtmlPage.Extension;

        public override Page CreatePage(int pageNumber, byte[] data)
            => new HtmlPage(pageNumber, data);

        public override Task<byte[]> GetPageResourceAsync(
            FileCredentials fileCredentials, int pageNumber, string resourceName) =>
            throw new System.NotImplementedException(
                $"{nameof(HtmlWithEmbeddedResourcesViewer)} does not support retrieving external HTML resources.");

        protected override ViewInfoOptions CreateViewInfoOptions() =>
            ViewInfoOptions.FromHtmlViewOptions(_config.HtmlViewOptions);

        protected override Page RenderPage(Viewer viewer, string filePath, int pageNumber)
        {
            var pageStream = new MemoryStream();
            var viewOptions = CreateViewOptions(pageStream);

            viewer.View(viewOptions, pageNumber);

            var bytes = pageStream.ToArray();
            var page = CreatePage(pageNumber, bytes);

            return page;
        }

        private HtmlViewOptions CreateViewOptions(MemoryStream pageStream)
        {
            var viewOptions = HtmlViewOptions.ForEmbeddedResources(_ => pageStream,
                (_, __) => { /*NOTE: Do nothing here*/ });

            viewOptions.CopyViewOptions(_config.HtmlViewOptions);

            return viewOptions;
        }
    }
}