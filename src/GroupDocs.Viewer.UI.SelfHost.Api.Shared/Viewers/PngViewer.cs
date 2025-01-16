using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.UI.Api.Configuration;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.SelfHost.Api.Configuration;
using GroupDocs.Viewer.UI.SelfHost.Api.InternalCaching;
using GroupDocs.Viewer.UI.SelfHost.Api.Licensing;
using GroupDocs.Viewer.UI.SelfHost.Api.Viewers.Extensions;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;
using Page = GroupDocs.Viewer.UI.Core.Entities.Page;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Viewers
{
    public class PngViewer : BaseViewer
    {
        private readonly Config _config;

        public PngViewer(
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

        public override string PageExtension => PngPage.DefaultExtension;

        public override string ThumbExtension => PngThumb.DefaultExtension;

        public override Page CreatePage(int pageNumber, byte[] data) =>
            new PngPage(pageNumber, data);

        public override Thumb CreateThumb(int pageNumber, byte[] data)
            => new PngThumb(pageNumber, data);

        public override Task<byte[]> GetPageResourceAsync(
            FileCredentials fileCredentials, int pageNumber, string resourceName) =>
            throw new System.NotImplementedException(
                $"{nameof(PngViewer)} does not support retrieving external HTML resources.");

        protected override Page RenderPage(Viewer viewer, string filePath, int pageNumber)
        {
            var pageStream = new MemoryStream();
            var pageViewOptions = CreatePageViewOptions(pageStream);
            viewer.View(pageViewOptions, pageNumber);

            var bytes = pageStream.ToArray();

            var page = CreatePage(pageNumber, bytes);
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

        private PngViewOptions CreatePageViewOptions(MemoryStream pageStream)
        {
            var viewOptions = new PngViewOptions(_ => pageStream,
                (_, __) => { /*NOTE: Do nothing here*/ });

            viewOptions.CopyViewOptions(_config.PngViewOptions);

            return viewOptions;
        }

        private PngViewOptions CreateThumbViewOptions(MemoryStream thumbStream)
        {
            var viewOptions = new PngViewOptions(_ => thumbStream,
                (_, __) => { /*NOTE: Do nothing here*/ });

            viewOptions.CopyViewOptions(_config.PngViewOptions);
            viewOptions.MaxWidth = ThumbSettings.MaxThumbWidth;
            viewOptions.MaxHeight = ThumbSettings.MaxThumbHeight;

            return viewOptions;
        }
    }
}