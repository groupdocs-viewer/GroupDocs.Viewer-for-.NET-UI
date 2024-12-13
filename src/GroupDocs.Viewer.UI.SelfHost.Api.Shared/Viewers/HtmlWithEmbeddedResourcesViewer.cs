using System;
using System.IO;
using System.Threading.Tasks;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.UI.Api.Configuration;
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
            IViewerLicenseManager licenseManager, 
            IInternalCache viewerCache,
            IFileStorage fileStorage, 
            IFileTypeResolver fileTypeResolver, 
            IPageFormatter pageFormatter) 
            : base(config, asyncLock, licenseManager, viewerCache, fileStorage, fileTypeResolver, pageFormatter)
        {
            _config = config.Value;
        }

        public override string PageExtension => HtmlPage.DefaultExtension;

        public override string ThumbExtension => JpgThumb.DefaultExtension;

        public override Page CreatePage(int pageNumber, byte[] data)
            => new HtmlPage(pageNumber, data);

        public override Thumb CreateThumb(int pageNumber, byte[] data)
            => new JpgThumb(pageNumber, data);

        public override Task<byte[]> GetPageResourceAsync(
            FileCredentials fileCredentials, int pageNumber, string resourceName) =>
            throw new System.NotImplementedException(
                $"{nameof(HtmlWithEmbeddedResourcesViewer)} does not support retrieving external HTML resources.");

        protected override ViewInfoOptions CreateViewInfoOptions() =>
            ViewInfoOptions.FromHtmlViewOptions(_config.HtmlViewOptions);

        protected override Page RenderPage(Viewer viewer, string filePath, int pageNumber)
        {
            var pageStream = new MemoryStream();
            var pageViewOptions = CreatePageViewOptions(pageStream);
            viewer.View(pageViewOptions, pageNumber);

            var pageBytes = pageStream.ToArray();

            var page = CreatePage(pageNumber, pageBytes);
            return page;
        }

        protected override Thumb RenderThumb(Viewer viewer, string filePath, int pageNumber)
        {
            var thumbStream = new MemoryStream();
            var thumbViewOptions = CreateThumbViewOptions(thumbStream);
            viewer.View(thumbViewOptions, pageNumber);

            var thumbBytes = thumbStream.ToArray();

            var thumb = CreateThumb(pageNumber, thumbBytes);

            return thumb;
        }

        private HtmlViewOptions CreatePageViewOptions(MemoryStream pageStream)
        {
            var viewOptions = HtmlViewOptions.ForEmbeddedResources(_ => pageStream,
                (_, __) => { /*NOTE: Do nothing here*/ });

            viewOptions.CopyViewOptions(_config.HtmlViewOptions);

            return viewOptions;
        }

        private JpgViewOptions CreateThumbViewOptions(MemoryStream pageStream)
        {
            var viewOptions = new JpgViewOptions(_ => pageStream,
                (_, __) => { /*NOTE: Do nothing here*/ });

            viewOptions.CopyBaseViewOptions(_config.HtmlViewOptions);
            viewOptions.ExtractText = false;
            viewOptions.Quality = ThumbSettings.ThumbQuality;
            viewOptions.MaxWidth = ThumbSettings.MaxThumbWidth;
            viewOptions.MaxHeight = ThumbSettings.MaxThumbHeight;

            return viewOptions;
        }
    }
}