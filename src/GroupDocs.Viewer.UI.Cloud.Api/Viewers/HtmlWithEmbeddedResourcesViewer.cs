using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.Cloud.Api.Viewers
{
    internal class HtmlWithEmbeddedResourcesViewer : BaseViewer
    {
        public HtmlWithEmbeddedResourcesViewer(
            IOptions<Config> config, 
            IFileStorage fileStorage, 
            IViewerApiConnect viewerApiConnect)
            : base(config, fileStorage, viewerApiConnect)
        {
        }

        public override string PageExtension => HtmlPage.Extension;

        public override Page CreatePage(int pageNumber, byte[] data) => 
            new HtmlPage(pageNumber, data);

        public override ViewOptions CreateViewOptions(FileInfo fileInfo)
        {
            var htmlOptions = new HtmlOptions
            {
                ExternalResources = false
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
    }
}