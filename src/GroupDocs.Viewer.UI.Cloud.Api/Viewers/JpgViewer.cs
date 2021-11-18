using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.Cloud.Api.Viewers
{
    internal class JpgViewer : BaseViewer
    {
        public JpgViewer(IOptions<Config> config, IFileStorage fileStorage, IViewerApiConnect viewerApiConnect) 
            : base (config, fileStorage, viewerApiConnect)
        {
        }

        public override string PageExtension => JpgPage.Extension;

        public override Page CreatePage(int pageNumber, byte[] data) =>
            new JpgPage(pageNumber, data);

        public override ViewOptions CreateViewOptions(FileInfo fileInfo)
        {
            var jpgOptions = new ImageOptions();
            
            Config.PngViewOptionsSetupAction(jpgOptions);

            var viewOptions = new ViewOptions
            {
                FileInfo = fileInfo,
                ViewFormat = ViewFormat.JPG,
                RenderOptions = jpgOptions,
                OutputPath = Config.OutputFolderPath
            };

            return viewOptions;
        }
    }
}