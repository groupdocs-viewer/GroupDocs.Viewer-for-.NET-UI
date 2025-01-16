using GroupDocs.Viewer.UI.Api.Configuration;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Utils;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.Cloud.Api.Viewers
{
    internal class JpgViewer : BaseViewer
    {
        public JpgViewer(IOptions<Config> config,
            IFileStorage fileStorage,
            IViewerApiConnect viewerApiConnect,
            IPageFormatter pageFormatter)
            : base(config, fileStorage, viewerApiConnect, pageFormatter)
        {
        }

        public override string PageExtension => JpgPage.DefaultExtension;

        public override string ThumbExtension => JpgThumb.DefaultExtension;

        public override Page CreatePage(int pageNumber, byte[] data) =>
            new JpgPage(pageNumber, data);

        public override Thumb CreateThumb(int pageNumber, byte[] data) =>
            new JpgThumb(pageNumber, data);

        public override ViewOptions CreatePagesViewOptions(FileInfo fileInfo)
        {
            var jpgOptions = new ImageOptions();

            Config.JpgViewOptionsSetupAction(jpgOptions);

            var viewOptions = new ViewOptions
            {
                FileInfo = fileInfo,
                ViewFormat = ViewFormat.JPG,
                RenderOptions = jpgOptions,
                OutputPath = Config.OutputFolderPath
            };

            return viewOptions;
        }

        public override ViewOptions CreateThumbsViewOptions(FileInfo fileInfo)
        {
            var imageOptions = new ImageOptions
            {
                MaxWidth = ThumbSettings.MaxThumbWidth,
                MaxHeight = ThumbSettings.MaxThumbHeight,
                JpegQuality = ThumbSettings.ThumbQuality
            };

            var tempImageOptions = new ImageOptions();
            Config.JpgViewOptionsSetupAction(tempImageOptions);
            RenderOptionsUtils.CopyRenderOptions(imageOptions, tempImageOptions);

            var viewOptions = new ViewOptions
            {
                FileInfo = fileInfo,
                ViewFormat = ViewFormat.JPG,
                RenderOptions = imageOptions,
                OutputPath = Config.OutputFolderPath,
            };

            return viewOptions;
        }
    }
}