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
    internal class PngViewer : BaseViewer
    {
        public PngViewer(IOptions<Config> config, 
            IFileStorage fileStorage, 
            IViewerApiConnect viewerApiConnect, 
            IPageFormatter pageFormatter) 
            : base (config, fileStorage, viewerApiConnect, pageFormatter)
        {
        }

        public override string PageExtension => PngPage.Extension;

        public override string ThumbExtension => PngThumb.Extension;

        public override Page CreatePage(int pageNumber, byte[] data) =>
            new PngPage(pageNumber, data);

        public override Thumb CreateThumb(int pageNumber, byte[] data) =>
            new PngThumb(pageNumber, data);

        public override ViewOptions CreatePagesViewOptions(FileInfo fileInfo)
        {
            var pngOptions = new ImageOptions();
            
            Config.PngViewOptionsSetupAction(pngOptions);
            
            var viewOptions = new ViewOptions
            {
                FileInfo = fileInfo,
                ViewFormat = ViewFormat.PNG,
                RenderOptions = pngOptions,
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
            };

            var tempImageOptions = new ImageOptions();
            Config.PngViewOptionsSetupAction(tempImageOptions);
            RenderOptionsUtils.CopyRenderOptions(imageOptions, tempImageOptions);

            var viewOptions = new ViewOptions
            {
                FileInfo = fileInfo,
                ViewFormat = ViewFormat.PNG,
                RenderOptions = imageOptions,
                OutputPath = Config.OutputFolderPath,
            };

            return viewOptions;
        }
    }
}