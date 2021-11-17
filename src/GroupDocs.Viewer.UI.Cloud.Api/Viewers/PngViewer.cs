using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.Cloud.Api.Viewers
{
    internal class PngViewer : BaseViewer
    {
        public PngViewer(IOptions<Config> config, IFileStorage fileStorage, IViewerApiConnect viewerApiConnect) 
            : base (config, fileStorage, viewerApiConnect)
        {
        }

        public override string PageExtension => PngPage.Extension;

        public override Page CreatePage(int pageNumber, byte[] data) =>
            new PngPage(pageNumber, data);

        public override ViewOptions CreateViewOptions(FileInfo fileInfo)
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
    }
}