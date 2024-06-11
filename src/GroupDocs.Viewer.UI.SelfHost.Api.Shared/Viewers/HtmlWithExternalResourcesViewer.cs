using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using GroupDocs.Viewer.Interfaces;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.Results;
using GroupDocs.Viewer.UI.Api;
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
    public class HtmlWithExternalResourcesViewer : BaseViewer
    {
        private readonly Config _config;
        private readonly UI.Api.Configuration.Options _options;

        public HtmlWithExternalResourcesViewer(
            IOptions<Config> config,
            IAsyncLock asyncLock,
            IOptions<UI.Api.Configuration.Options> options,
            IViewerLicenser licenser,
            IInternalCache internalCache,
            IFileStorage fileStorage,
            IFileTypeResolver fileTypeResolver,
            IPageFormatter pageFormatter) 
            : base(config, asyncLock, licenser, internalCache, fileStorage, fileTypeResolver, pageFormatter)
        {
            _config = config.Value;
            _options = options.Value;
        }
        
        public override string PageExtension => HtmlPage.Extension;

        public override Page CreatePage(int pageNumber, byte[] data) 
            => new HtmlPage(pageNumber, data);

        protected override Page RenderPage(Viewer viewer, string filePath, int pageNumber)
        {
            var basePath = _options.ApiPath;
            var actionName = Constants.LOAD_DOCUMENT_PAGE_RESOURCE_ACTION_NAME;

            var streamFactory = new MemoryPageStreamFactory(basePath, actionName, filePath);
            var viewOptions = HtmlViewOptions.ForExternalResources(streamFactory, streamFactory);
            viewOptions.CopyViewOptions(_config.HtmlViewOptions);
            viewer.View(viewOptions, pageNumber);

            var pageContents = streamFactory.GetPageContents();
            var page = CreatePage(pageNumber, pageContents.GetPageData());
            foreach (var resource in pageContents.Resources)
            {
                var pageResource = new PageResource(resource.Key, resource.Value.ToArray());
                page.AddResource(pageResource);
            }

            return page;
        }

        protected override ViewInfoOptions CreateViewInfoOptions() =>
            ViewInfoOptions.FromHtmlViewOptions(_config.HtmlViewOptions);

        public override async Task<byte[]> GetPageResourceAsync(
            FileCredentials fileCredentials, int pageNumber, string resourceName)
        {
            var page = await GetPageAsync(fileCredentials, pageNumber);
            var resource = page.GetResource(resourceName);

            return resource.Data;
        }

        private class MemoryPageStreamFactory : IPageStreamFactory, IResourceStreamFactory
        {
            private readonly string _basePath;
            private readonly string _actionName;
            private readonly string _filePath;
            private readonly PageContents _pageContents;

            public MemoryPageStreamFactory(string basePath, string actionName, string filePath)
            {
                _basePath = basePath;
                _actionName = actionName;
                _filePath = WebUtility.UrlEncode(filePath);
                _pageContents = new PageContents();
            }

            public PageContents GetPageContents() =>
                _pageContents;

            public Stream CreatePageStream(int pageNumber) =>
                _pageContents.GetPageStream();

            public void ReleasePageStream(int pageNumber, Stream pageStream) { }

            public Stream CreateResourceStream(int pageNumber, Resource resource) =>
                _pageContents.GetResourceStream(resource.FileName);

            public string CreateResourceUrl(int pageNumber, Resource resource) =>
                $"{_basePath}/{_actionName}?guid={_filePath}&pageNumber={pageNumber}&resourceName={resource.FileName}";

            public void ReleaseResourceStream(int pageNumber, Resource resource, Stream resourceStream) { }
        }

        private class PageContents
        {
            private MemoryStream PageStream { get; } = new MemoryStream();

            public Dictionary<string, MemoryStream> Resources { get; } = new Dictionary<string, MemoryStream>();

            public byte[] GetPageData() => PageStream.ToArray();

            public Stream GetPageStream() => PageStream;

            public Stream GetResourceStream(string fileName)
            {
                var resourceStream = new MemoryStream();
                Resources.Add(fileName, resourceStream);
                return resourceStream;
            }
        }
    }
}