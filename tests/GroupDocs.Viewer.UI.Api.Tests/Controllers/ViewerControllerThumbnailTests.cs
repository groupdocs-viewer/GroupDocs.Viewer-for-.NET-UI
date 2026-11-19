using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Controllers;
using GroupDocs.Viewer.UI.Api.Models;
using GroupDocs.Viewer.UI.Api.Utils;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Configuration;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Controllers
{
    /// <summary>
    /// view-data and create-pages return the thumbnail URLs of the pages they create, without rendering
    /// the thumbnails: the browser requests each URL, and get-thumb renders the thumbnail then.
    /// </summary>
    public class ViewerControllerThumbnailTests
    {
        private readonly Mock<IViewer> _viewer = new Mock<IViewer>();

        public ViewerControllerThumbnailTests()
        {
            _viewer
                .Setup(viewer => viewer.GetDocumentInfoAsync(It.IsAny<FileCredentials>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DocumentInfo
                {
                    FileType = "docx",
                    PrintAllowed = true,
                    Pages = new[]
                    {
                        new PageInfo { Number = 1, Width = 800, Height = 1000 },
                        new PageInfo { Number = 2, Width = 800, Height = 1000 },
                        new PageInfo { Number = 3, Width = 800, Height = 1000 },
                    }
                });
            _viewer
                .Setup(viewer => viewer.GetPagesAsync(It.IsAny<FileCredentials>(), It.IsAny<int[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Pages());
        }

        [Fact]
        public async Task ViewData_ShouldReturnThumbUrlsWithoutRenderingThumbnails()
        {
            var controller = CreateController(new Config { PreloadPages = 1 });

            var result = await controller.ViewData(new ViewDataRequest { File = "sample.docx" });

            var response = Assert.IsType<ViewDataResponse>(Assert.IsType<OkObjectResult>(result).Value);
            Assert.Equal("thumb-1", response.Pages[0].ThumbUrl);
            Assert.Null(response.Pages[1].ThumbUrl);
            _viewer.Verify(viewer => viewer.GetPagesAsync(
                It.IsAny<FileCredentials>(), It.Is<int[]>(pages => pages.SequenceEqual(new[] { 1 })), It.IsAny<CancellationToken>()), Times.Once);
            _viewer.Verify(viewer => viewer.GetThumbsAsync(
                It.IsAny<FileCredentials>(), It.IsAny<int[]>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreatePages_ShouldReturnThumbUrlsWithoutRenderingThumbnails()
        {
            var controller = CreateController(new Config());

            var result = await controller.CreatePages(new CreatePagesRequest { File = "sample.docx", Pages = new[] { 2, 3 } });

            var pages = Assert.IsType<List<PageData>>(Assert.IsType<OkObjectResult>(result).Value);
            Assert.Equal(new[] { "thumb-2", "thumb-3" }, pages.Select(page => page.ThumbUrl));
            _viewer.Verify(viewer => viewer.GetThumbsAsync(
                It.IsAny<FileCredentials>(), It.IsAny<int[]>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreatePages_WithThumbnailsHidden_ShouldReturnNoThumbUrls()
        {
            var controller = CreateController(new Config { EnableThumbnails = false });

            var result = await controller.CreatePages(new CreatePagesRequest { File = "sample.docx", Pages = new[] { 2 } });

            var pages = Assert.IsType<List<PageData>>(Assert.IsType<OkObjectResult>(result).Value);
            Assert.Equal("page-2", pages.Single().PageUrl);
            Assert.Null(pages.Single().ThumbUrl);
        }

        private ViewerController CreateController(Config config)
        {
            var urls = new Mock<IApiUrlBuilder>();
            urls.Setup(builder => builder.BuildPageUrl(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns((string _, int page, string _) => $"page-{page}");
            urls.Setup(builder => builder.BuildThumbUrl(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns((string _, int page, string _) => $"thumb-{page}");

            return new ViewerController(
                _viewer.Object,
                Mock.Of<IFileStorage>(),
                Mock.Of<IFileNameResolver>(),
                Mock.Of<ISearchTermResolver>(),
                new ExceptionMessageProvider(),
                MSOptions.Create(config),
                urls.Object,
                NullLogger<ViewerController>.Instance)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
        }
    }
}
