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
    public class JpgViewer : BaseViewer
    {
        private readonly Config _config;

        public JpgViewer(
            IOptions<Config> config,
            IAsyncLock asyncLock,
            IViewerLicenseManager licenseManager,
            IInternalCache internalCache,
            IFileStorage fileStorage, 
            IFileTypeResolver fileTypeResolver,
            IPageFormatter pageFormatter)
            : base(config, asyncLock, licenseManager, internalCache, fileStorage, fileTypeResolver, pageFormatter)
        {
            _config = config.Value;
        }

        public override string PageExtension => JpgPage.Extension;

        public override string ThumbExtension => JpgThumb.Extension;

        public override Page CreatePage(int pageNumber, byte[] data) =>
            new JpgPage(pageNumber, data);

        public override Thumb CreateThumb(int pageNumber, byte[] data)
            => new JpgThumb(pageNumber, data);

        public override Task<byte[]> GetPageResourceAsync(
            FileCredentials fileCredentials, int pageNumber, string resourceName) => 
            throw new System.NotImplementedException(
                $"{nameof(JpgViewer)} does not support retrieving external HTML resources.");

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

        protected override ViewInfoOptions CreateViewInfoOptions() =>
            ViewInfoOptions.FromJpgViewOptions(_config.JpgViewOptions);

        private JpgViewOptions CreatePageViewOptions(MemoryStream pageStream)
        {
            var viewOptions = new JpgViewOptions(_ => pageStream,
                (_, __) => { /*NOTE: Do nothing here*/ });

            viewOptions.CopyViewOptions(_config.JpgViewOptions);

            return viewOptions;
        }

        private JpgViewOptions CreateThumbViewOptions(MemoryStream thumbStream)
        {
            var viewOptions = new JpgViewOptions(_ => thumbStream,
                (_, __) => { /*NOTE: Do nothing here*/ });

            viewOptions.CopyViewOptions(_config.JpgViewOptions);
            viewOptions.Quality = ThumbSettings.ThumbQuality;
            viewOptions.MaxWidth = ThumbSettings.MaxThumbWidth;
            viewOptions.MaxHeight = ThumbSettings.MaxThumbHeight;

            return viewOptions;
        }
    }
}