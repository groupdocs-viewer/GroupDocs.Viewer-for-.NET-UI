using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GroupDocs.Viewer.Interfaces;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.Results;
using GroupDocs.Viewer.UI.Api.Configuration;
using GroupDocs.Viewer.UI.Api.Utils;
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
        private readonly IApiUrlBuilder _apiUrlBuilder;

        public HtmlWithExternalResourcesViewer(
            IOptions<Config> config,
            IAsyncLock asyncLock,
            IViewerLicenseManager licenseManager,
            IInternalCache internalCache,
            IFileStorage fileStorage,
            IFileTypeResolver fileTypeResolver,
            IPageFormatter pageFormatter,
            IApiUrlBuilder apiUrlBuilder) 
            : base(config, asyncLock, licenseManager, internalCache, fileStorage, fileTypeResolver, pageFormatter)
        {
            _apiUrlBuilder = apiUrlBuilder;
            _config = config.Value;
        }
        
        public override string PageExtension => HtmlPage.DefaultExtension;

        public override string ThumbExtension => JpgThumb.DefaultExtension;

        public override Page CreatePage(int pageNumber, byte[] data) 
            => new HtmlPage(pageNumber, data);

        public override Thumb CreateThumb(int pageNumber, byte[] data)
            => new JpgThumb(pageNumber, data);

        protected override Page RenderPage(Viewer viewer, string filePath, int pageNumber)
        {
            // Create page
            var streamFactory = new MemoryPageStreamFactory(filePath, _apiUrlBuilder);
            var pageViewOptions = HtmlViewOptions.ForExternalResources(streamFactory, streamFactory);
            pageViewOptions.CopyViewOptions(_config.HtmlViewOptions);
            viewer.View(pageViewOptions, pageNumber);

            var pageContents = streamFactory.GetPageContents();
            var pageBytes = pageContents.GetPageData();

            var page = CreatePage(pageNumber, pageBytes);
            foreach (var resource in pageContents.Resources)
            {
                var pageResource = new PageResource(resource.Key, resource.Value.ToArray());
                page.AddResource(pageResource);
            }

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
            ViewInfoOptions.FromHtmlViewOptions(_config.HtmlViewOptions);

        public override async Task<byte[]> GetPageResourceAsync(
            FileCredentials fileCredentials, int pageNumber, string resourceName)
        {
            var page = await GetPageAsync(fileCredentials, pageNumber);
            var resource = page.GetResource(resourceName);

            return resource.Data;
        }

        private JpgViewOptions CreateThumbViewOptions(MemoryStream pageStream)
        {
            var viewOptions = new JpgViewOptions(_ => pageStream,
                (_, __) => { /*NOTE: Do nothing here*/ });

            viewOptions.CopyBaseViewOptions(_config.HtmlViewOptions);
            viewOptions.ExtractText = false;
            viewOptions.Quality = ThumbSettings.ThumbQuality;
            viewOptions.MaxWidth = ThumbSettings.MaxThumbWidth;
            viewOptions.MaxHeight = ThumbSettings.MaxThumbHeight;

            return viewOptions;
        }

        private class MemoryPageStreamFactory : IPageStreamFactory, IResourceStreamFactory
        {
            private readonly string _file;
            private readonly PageContents _pageContents;
            private readonly IApiUrlBuilder _apiUrlBuilder;

            public MemoryPageStreamFactory(string file, IApiUrlBuilder apiUrlBuilder)
            {
                _file = file;
                _apiUrlBuilder = apiUrlBuilder;
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
                _apiUrlBuilder.BuildResourceUrl(_file, pageNumber, resource.FileName);

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