using System;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.Extensions.Options;
using Options = GroupDocs.Viewer.UI.Api.Configuration.Options;

namespace GroupDocs.Viewer.UI.Cloud.Api.Viewers
{
    internal class HtmlWithExternalResourcesViewer : BaseViewer
    {
        private readonly Options _options;

        public HtmlWithExternalResourcesViewer(
            IOptions<Config> config,
            IOptions<Options> options,
            IFileStorage fileStorage, 
            IViewerApiConnect viewerApiConnect,
            IPageFormatter pageFormatter)
            : base(config, fileStorage, viewerApiConnect, pageFormatter)
        {
            _options = options.Value;
        }

        public override string PageExtension => HtmlPage.Extension;

        public override Page CreatePage(int pageNumber, byte[] data) =>
            new HtmlPage(pageNumber, data);

        public override ViewOptions CreateViewOptions(FileInfo fileInfo)
        {
            var filePath = fileInfo.FilePath;
            var basePath = _options.ApiPath;
            var actionName = UI.Api.Constants.LOAD_DOCUMENT_PAGE_RESOURCE_ACTION_NAME;
            var htmlOptions = new HtmlOptions
            {
                ExternalResources = true,
                ResourcePath = 
                    $"{basePath}/{actionName}?guid={filePath}&pageNumber={{page-number}}&resourceName={{resource-name}}"
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