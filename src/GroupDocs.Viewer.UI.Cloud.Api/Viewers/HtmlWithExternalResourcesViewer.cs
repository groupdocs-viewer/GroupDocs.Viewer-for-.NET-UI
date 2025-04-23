using GroupDocs.Viewer.UI.Api.Configuration;
using GroupDocs.Viewer.UI.Api.Utils;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Utils;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.Cloud.Api.Viewers
{
    internal class HtmlWithExternalResourcesViewer : BaseViewer
    {
        private readonly IApiUrlBuilder _apiUrlBuilder;

        public HtmlWithExternalResourcesViewer(
            IOptions<Config> config,
            IFileStorage fileStorage, 
            IViewerApiConnect viewerApiConnect,
            IPageFormatter pageFormatter,
            IApiUrlBuilder apiUrlBuilder)
            : base(config, fileStorage, viewerApiConnect, pageFormatter)
        {
            _apiUrlBuilder = apiUrlBuilder;
        }

        public override string PageExtension => HtmlPage.DefaultExtension;

        public override string ThumbExtension => JpgThumb.DefaultExtension;

        public override Page CreatePage(int pageNumber, byte[] data) =>
            new HtmlPage(pageNumber, data);

        public override Thumb CreateThumb(int pageNumber, byte[] data) =>
            new JpgThumb(pageNumber, data);

        public override ViewOptions CreatePagesViewOptions(FileInfo fileInfo)
        {
            var filePath = fileInfo.FilePath;
            var htmlOptions = new HtmlOptions
            {
                ExternalResources = true,
                ResourcePath = _apiUrlBuilder.BuildResourceUrl(
                    filePath, "{page-number}", "{resource-name}")
            };

            Config.HtmlViewOptionsSetupAction(htmlOptions);

            var viewOptions = new ViewOptions
            {
                FileInfo = fileInfo,
                ViewFormat = ViewFormat.HTML,
                RenderOptions = htmlOptions,
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

            var tempHtmlOptions = new HtmlOptions();
            Config.HtmlViewOptionsSetupAction(tempHtmlOptions);
            RenderOptionsUtils.CopyRenderOptions(imageOptions, tempHtmlOptions);

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