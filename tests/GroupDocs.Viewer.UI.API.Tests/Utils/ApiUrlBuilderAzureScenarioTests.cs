using GroupDocs.Viewer.UI.Api.Configuration;
using GroupDocs.Viewer.UI.Api.Utils;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Utils
{
    /// <summary>
    /// Tests specifically for Azure App Service scenarios.
    /// These tests verify that absolute URLs are returned when apiDomain is configured,
    /// and relative URLs are returned when apiDomain is not specified.
    /// </summary>
    public class ApiUrlBuilderAzureScenarioTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IOptionsProvider> _optionsProviderMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<HttpRequest> _httpRequestMock;

        public ApiUrlBuilderAzureScenarioTests()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _optionsProviderMock = new Mock<IOptionsProvider>();
            _httpContextMock = new Mock<HttpContext>();
            _httpRequestMock = new Mock<HttpRequest>();

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(_httpContextMock.Object);
            _httpContextMock.Setup(x => x.Request).Returns(_httpRequestMock.Object);
        }

        private ApiUrlBuilder CreateBuilder(string apiPath = "/viewer-api", string? apiDomain = null, bool useAbsoluteUrls = false)
        {
            var options = new Options
            {
                ApiPath = apiPath,
                ApiDomain = apiDomain,
                UseAbsoluteUrls = useAbsoluteUrls
            };

            _optionsProviderMock.Setup(x => x.GetOptions()).Returns(options);
            return new ApiUrlBuilder(_httpContextAccessorMock.Object, _optionsProviderMock.Object);
        }

        [Fact]
        public void BuildPageUrl_WithAzureDomain_ShouldReturnAbsoluteUrl()
        {
            // Arrange - Simulating Azure App Service scenario
            // User configures ApiDomain with Azure hostname and UseAbsoluteUrls
            var builder = CreateBuilder(
                apiPath: "/viewer-api",
                apiDomain: "https://app-name.azurewebsites.net",
                useAbsoluteUrls: true
            );
            var file = "document.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // Should be absolute URL when UseAbsoluteUrls is true
            Assert.StartsWith("https://", result);
            Assert.Contains("azurewebsites.net", result);
            Assert.Contains("app-name", result);
            Assert.Contains("/viewer-api/get-page", result);
            Assert.Contains("file=document.docx", result);
            Assert.Contains("page=1", result);
        }

        [Fact]
        public void BuildPageUrl_WithAzureDomainAndPath_ShouldReturnAbsoluteUrl()
        {
            // Arrange - Azure domain with path in it
            var builder = CreateBuilder(
                apiPath: "/viewer-api",
                apiDomain: "https://app-name.azurewebsites.net/viewer-api",
                useAbsoluteUrls: true
            );
            var file = "document.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // Should use apiPath and build absolute URL when UseAbsoluteUrls is true
            Assert.StartsWith("https://", result);
            Assert.Contains("azurewebsites.net", result);
            Assert.Contains("/viewer-api/get-page", result);
            Assert.Contains("file=document.docx", result);
            Assert.Contains("page=1", result);
        }

        [Fact]
        public void BuildPageUrl_WithCustomDomainConfigured_ShouldReturnAbsoluteUrl()
        {
            // Arrange - Custom domain is configured
            var builder = CreateBuilder(
                apiPath: "/viewer-api",
                apiDomain: "https://custom-domain.com",
                useAbsoluteUrls: true
            );
            var file = "document.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // Should be absolute URL when UseAbsoluteUrls is true
            Assert.StartsWith("https://", result);
            Assert.Contains("custom-domain.com", result);
            Assert.Contains("/viewer-api/get-page", result);
            Assert.Contains("file=document.docx", result);
            Assert.Contains("page=1", result);
        }

        [Fact]
        public void BuildAllUrls_WithAzureDomain_ShouldAllBeAbsolute()
        {
            // Arrange
            var builder = CreateBuilder(
                apiPath: "/viewer-api",
                apiDomain: "https://app-name.azurewebsites.net",
                useAbsoluteUrls: true
            );
            var file = "document.docx";
            var page = 1;
            var extension = "html";

            // Act
            var pageUrl = builder.BuildPageUrl(file, page, extension);
            var thumbUrl = builder.BuildThumbUrl(file, page, extension);
            var pdfUrl = builder.BuildPdfUrl(file);
            var resourceUrl = builder.BuildResourceUrl(file, page, "style.css");

            // Assert
            Assert.All(new[] { pageUrl, thumbUrl, pdfUrl, resourceUrl }, url =>
            {
                Assert.StartsWith("https://", url);
                Assert.Contains("azurewebsites.net", url);
            });
        }

        [Fact]
        public void BuildPageUrl_WithHttpAzureDomain_ShouldReturnAbsoluteUrl()
        {
            // Arrange - HTTP instead of HTTPS
            var builder = CreateBuilder(
                apiPath: "/viewer-api",
                apiDomain: "http://app-name.azurewebsites.net",
                useAbsoluteUrls: true
            );
            var file = "document.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            Assert.StartsWith("http://", result);
            Assert.Contains("azurewebsites.net", result);
            Assert.Contains("/viewer-api/get-page", result);
            Assert.Contains("file=document.docx", result);
            Assert.Contains("page=1", result);
        }

        [Fact]
        public void BuildPageUrl_WithPortInDomain_ShouldReturnAbsoluteUrl()
        {
            // Arrange - Domain with port
            var builder = CreateBuilder(
                apiPath: "/viewer-api",
                apiDomain: "https://app-name.azurewebsites.net:443",
                useAbsoluteUrls: true
            );
            var file = "document.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            Assert.StartsWith("https://", result);
            Assert.Contains("azurewebsites.net", result);
            Assert.Contains(":443", result);
            Assert.Contains("/viewer-api/get-page", result);
            Assert.Contains("file=document.docx", result);
            Assert.Contains("page=1", result);
        }

        [Fact]
        public void BuildPageUrl_WithoutApiDomain_ShouldReturnRelativeUrl()
        {
            // Arrange - No apiDomain configured
            var builder = CreateBuilder(
                apiPath: "/viewer-api",
                apiDomain: null
            );
            var file = "document.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // Should be relative URL when apiDomain is not specified, and apiPath should be ignored
            Assert.StartsWith("/", result);
            Assert.Equal("/get-page?file=document.docx&page=1", result);
            Assert.DoesNotContain("/viewer-api/", result); // apiPath should be ignored
            Assert.DoesNotContain("http://", result);
            Assert.DoesNotContain("https://", result);
        }
    }
}
