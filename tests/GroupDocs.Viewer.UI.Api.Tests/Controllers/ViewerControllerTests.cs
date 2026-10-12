using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Extensions.Primitives;
using Moq;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Controllers
{
    public class ViewerControllerTests
    {
        private readonly Mock<IViewer> _viewer = new Mock<IViewer>();
        private readonly Mock<IFileStorage> _storage = new Mock<IFileStorage>();
        private readonly ViewerController _controller;

        public ViewerControllerTests()
        {
            _controller = new ViewerController(
                _viewer.Object,
                _storage.Object,
                Mock.Of<IFileNameResolver>(),
                Mock.Of<ISearchTermResolver>(),
                new ExceptionMessageProvider(),
                MSOptions.Create(new Config()),
                Mock.Of<IApiUrlBuilder>(),
                NullLogger<ViewerController>.Instance)
            {
                ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
            };
        }

        // [FromBody] arguments are null when the request body is missing or is not valid JSON
        [Fact]
        public async Task ListDir_WithoutRequestBody_ShouldReturnBadRequest()
        {
            var result = await _controller.ListDir(null!);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ViewData_WithoutRequestBody_ShouldReturnBadRequest()
        {
            var result = await _controller.ViewData(null!);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreatePages_WithoutRequestBody_ShouldReturnBadRequest()
        {
            var result = await _controller.CreatePages(null!);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreatePdf_WithoutRequestBody_ShouldReturnBadRequest()
        {
            var result = await _controller.CreatePdf(null!);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task ViewData_WithoutFile_ShouldReturnBadRequest()
        {
            var result = await _controller.ViewData(new ViewDataRequest());

            Assert.IsType<BadRequestObjectResult>(result);
            _viewer.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetPage_WithoutFile_ShouldReturnBadRequest()
        {
            var result = await _controller.GetPage(new GetPageRequest { Page = 1 });

            Assert.IsType<BadRequestObjectResult>(result);
            _viewer.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ViewData_WhenFileIsNotFound_ShouldReturnFileNameOnly()
        {
            var serverFilePath = Path.GetFullPath(Path.Combine("srv", "Files", "password-report.docx"));
            _viewer
                .Setup(viewer => viewer.GetDocumentInfoAsync(It.IsAny<FileCredentials>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new FileNotFoundException($"Could not find file '{serverFilePath}'.", serverFilePath));

            var result = await _controller.ViewData(new ViewDataRequest { File = "password-report.docx" });

            // A missing file whose name mentions a password is not a password-protected file
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("File 'password-report.docx' was not found.", objectResult.Value);
        }

        [Fact]
        public async Task ViewData_WhenPasswordIsRequired_ShouldReturnForbidden()
        {
            _viewer
                .Setup(viewer => viewer.GetDocumentInfoAsync(It.IsAny<FileCredentials>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Password is required to open the document."));

            var result = await _controller.ViewData(new ViewDataRequest { File = "protected.docx" });

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status403Forbidden, objectResult.StatusCode);
        }

        [Fact]
        public async Task GetPage_WhenAccessIsDenied_ShouldNotExposeServerPath()
        {
            var serverPath = Path.GetFullPath(Path.Combine("srv", "Files"));
            _viewer
                .Setup(viewer => viewer.GetPageAsync(It.IsAny<FileCredentials>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new UnauthorizedAccessException($"Access to the path '{serverPath}' is denied."));

            var result = await _controller.GetPage(new GetPageRequest { File = "sub", Page = 1 });

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Equal("Access to the file or folder is denied.", objectResult.Value);
        }

        [Fact]
        public async Task UploadFile_WithInternalUrl_ShouldNotSaveFile()
        {
            _controller.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                ["url"] = "http://169.254.169.254/latest/meta-data/"
            });

            var result = await _controller.UploadFile();

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
            Assert.Contains("internal network addresses is not allowed", objectResult.Value as string);
            _storage.VerifyNoOtherCalls();
        }
    }
}
