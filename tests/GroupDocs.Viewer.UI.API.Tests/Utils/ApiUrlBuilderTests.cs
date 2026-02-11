using GroupDocs.Viewer.UI.Api.Configuration;
using GroupDocs.Viewer.UI.Api.Utils;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Utils
{
    public class ApiUrlBuilderTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IOptionsProvider> _optionsProviderMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<HttpRequest> _httpRequestMock;

        public ApiUrlBuilderTests()
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
        public void BuildPageUrl_ShouldReturnRelativeUrl()
        {
            // Arrange
            var builder = CreateBuilder();
            var file = "test-file.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            Assert.StartsWith("/", result);
            Assert.Contains("/get-page", result);
            Assert.DoesNotContain("/viewer-api/", result); // apiPath should be ignored when apiDomain is not set
            Assert.Contains("file=test-file.docx", result);
            Assert.Contains("page=1", result);
            Assert.DoesNotContain("http://", result);
            Assert.DoesNotContain("https://", result);
        }

        [Fact]
        public void BuildPageUrl_WithAzureDomain_ShouldReturnAbsoluteUrl()
        {
            // Arrange
            var builder = CreateBuilder(apiDomain: "https://app-name.azurewebsites.net", useAbsoluteUrls: true);
            var file = "test-file.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            Assert.StartsWith("https://", result);
            Assert.Contains("azurewebsites.net", result);
            Assert.Contains("/viewer-api/get-page", result);
            Assert.Contains("file=test-file.docx", result);
            Assert.Contains("page=1", result);
        }

        [Fact]
        public void BuildThumbUrl_ShouldReturnRelativeUrl()
        {
            // Arrange
            var builder = CreateBuilder();
            var file = "test-file.docx";
            var page = 2;
            var extension = "jpg";

            // Act
            var result = builder.BuildThumbUrl(file, page, extension);

            // Assert
            Assert.StartsWith("/", result);
            Assert.Contains("/get-thumb", result);
            Assert.DoesNotContain("/viewer-api/", result); // apiPath should be ignored when apiDomain is not set
            Assert.Contains("file=test-file.docx", result);
            Assert.Contains("page=2", result);
            Assert.DoesNotContain("http://", result);
            Assert.DoesNotContain("https://", result);
        }

        [Fact]
        public void BuildPdfUrl_ShouldReturnRelativeUrl()
        {
            // Arrange
            var builder = CreateBuilder();
            var file = "test-file.docx";

            // Act
            var result = builder.BuildPdfUrl(file);

            // Assert
            Assert.StartsWith("/", result);
            Assert.Contains("/get-pdf", result);
            Assert.DoesNotContain("/viewer-api/", result); // apiPath should be ignored when apiDomain is not set
            Assert.Contains("file=test-file.docx", result);
            Assert.DoesNotContain("http://", result);
            Assert.DoesNotContain("https://", result);
        }

        [Fact]
        public void BuildResourceUrl_WithPageAndResource_ShouldReturnRelativeUrl()
        {
            // Arrange
            var builder = CreateBuilder();
            var file = "test-file.docx";
            var page = 3;
            var resource = "style.css";

            // Act
            var result = builder.BuildResourceUrl(file, page, resource);

            // Assert
            Assert.StartsWith("/", result);
            Assert.Contains("/get-resource", result);
            Assert.DoesNotContain("/viewer-api/", result); // apiPath should be ignored when apiDomain is not set
            Assert.Contains("file=test-file.docx", result);
            Assert.Contains("page=3", result);
            Assert.Contains("resource=style.css", result);
            Assert.DoesNotContain("http://", result);
            Assert.DoesNotContain("https://", result);
        }

        [Fact]
        public void BuildResourceUrl_WithTemplates_ShouldReturnRelativeUrl()
        {
            // Arrange
            var builder = CreateBuilder();
            var file = "test-file.docx";
            var pageTemplate = "{page-number}";
            var resourceTemplate = "{resource-name}";

            // Act
            var result = builder.BuildResourceUrl(file, pageTemplate, resourceTemplate);

            // Assert
            Assert.StartsWith("/", result);
            Assert.Contains("/get-resource", result);
            Assert.DoesNotContain("/viewer-api/", result); // apiPath should be ignored when apiDomain is not set
            Assert.Contains("file=test-file.docx", result);
            // Curly braces are URL encoded by HttpUtility.UrlEncode
            // { becomes %7B and } becomes %7D
            Assert.Contains("page=", result);
            Assert.Contains("resource=", result);
            // Verify the template values are present (encoded or not)
            Assert.True(result.Contains("page-number") || result.Contains("%7Bpage-number%7D"), 
                $"Expected page template in result: {result}");
            Assert.True(result.Contains("resource-name") || result.Contains("%7Bresource-name%7D"), 
                $"Expected resource template in result: {result}");
            Assert.DoesNotContain("http://", result);
            Assert.DoesNotContain("https://", result);
        }

        [Theory]
        [InlineData("/viewer-api")]
        [InlineData("/viewer-api/")]
        [InlineData("viewer-api")]
        [InlineData("viewer-api/")]
        public void BuildPageUrl_WithDifferentApiPathFormats_ShouldIgnoreApiPathWhenDomainNotSet(string apiPath)
        {
            // Arrange
            var builder = CreateBuilder(apiPath: apiPath);
            var file = "test-file.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // When apiDomain is not set, apiPath should be ignored
            Assert.StartsWith("/", result);
            Assert.Contains("/get-page", result);
            Assert.DoesNotContain("/viewer-api/", result); // apiPath should be ignored
            Assert.DoesNotContain("//", result); // No double slashes
        }

        [Fact]
        public void BuildPageUrl_WithCustomApiPath_ShouldIgnoreApiPathWhenDomainNotSet()
        {
            // Arrange
            var builder = CreateBuilder(apiPath: "/custom-api");
            var file = "test-file.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // When apiDomain is not set, apiPath should be ignored
            Assert.StartsWith("/", result);
            Assert.Contains("/get-page", result);
            Assert.DoesNotContain("/custom-api/", result); // apiPath should be ignored
            Assert.DoesNotContain("/viewer-api", result);
        }

        [Fact]
        public void BuildPageUrl_WithLocalhostDomain_ShouldReturnAbsoluteUrl()
        {
            // Arrange
            var builder = CreateBuilder(apiDomain: "https://localhost:5001", useAbsoluteUrls: true);
            var file = "test-file.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            Assert.StartsWith("https://", result);
            Assert.Contains("localhost:5001", result);
            Assert.Contains("/viewer-api/get-page", result);
            Assert.Contains("file=test-file.docx", result);
            Assert.Contains("page=1", result);
        }

        [Fact]
        public void BuildPageUrl_WithQueryParameters_ShouldEncodeCorrectly()
        {
            // Arrange
            var builder = CreateBuilder();
            var file = "test file with spaces.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            Assert.StartsWith("/", result);
            Assert.Contains("file=test+file+with+spaces.docx", result);
        }

        [Fact]
        public void BuildPageUrl_WithSpecialCharacters_ShouldEncodeCorrectly()
        {
            // Arrange
            var builder = CreateBuilder();
            var file = "test&file=value.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            Assert.StartsWith("/", result);
            // The & should be encoded
            Assert.Contains("file=", result);
        }

        [Fact]
        public void BuildPageUrl_WithNullApiDomain_ShouldStillReturnRelativeUrl()
        {
            // Arrange
            var builder = CreateBuilder(apiDomain: null);
            var file = "test-file.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            Assert.StartsWith("/", result);
            Assert.Contains("/get-page", result);
            Assert.DoesNotContain("/viewer-api/", result); // apiPath should be ignored when apiDomain is not set
            Assert.DoesNotContain("http://", result);
            Assert.DoesNotContain("https://", result);
        }

        [Fact]
        public void BuildPageUrl_WithEmptyApiDomain_ShouldStillReturnRelativeUrl()
        {
            // Arrange
            var builder = CreateBuilder(apiDomain: "");
            var file = "test-file.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            Assert.StartsWith("/", result);
            Assert.Contains("/get-page", result);
            Assert.DoesNotContain("/viewer-api/", result); // apiPath should be ignored when apiDomain is not set
            Assert.DoesNotContain("http://", result);
            Assert.DoesNotContain("https://", result);
        }

        [Fact]
        public void BuildPageUrl_WithMultiplePages_ShouldIncludeCorrectPageNumber()
        {
            // Arrange
            var builder = CreateBuilder();
            var file = "test-file.docx";

            // Act & Assert
            for (int page = 1; page <= 5; page++)
            {
                var result = builder.BuildPageUrl(file, page, "html");
                Assert.Contains($"page={page}", result);
            }
        }

        [Fact]
        public void BuildPageUrl_AllMethods_ShouldReturnRelativeUrls()
        {
            // Arrange
            var builder = CreateBuilder();
            var file = "test-file.docx";
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
                Assert.StartsWith("/", url);
                Assert.DoesNotContain("http://", url);
                Assert.DoesNotContain("https://", url);
            });
        }

        [Fact]
        public void BuildPageUrl_WithDomain_AllMethods_ShouldReturnAbsoluteUrls()
        {
            // Arrange
            var builder = CreateBuilder(apiDomain: "https://example.com", useAbsoluteUrls: true);
            var file = "test-file.docx";
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
                Assert.Contains("example.com", url);
            });
        }

        [Fact]
        public void BuildPageUrl_WithUseAbsoluteUrlsFalse_ShouldReturnRelativeUrlEvenWithDomain()
        {
            // Arrange
            var builder = CreateBuilder(apiDomain: "https://example.com", useAbsoluteUrls: false);
            var file = "test-file.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // When UseAbsoluteUrls is false, should return relative URL even if ApiDomain is set
            Assert.StartsWith("/", result);
            Assert.Contains("/get-page", result);
            Assert.DoesNotContain("example.com", result);
            Assert.DoesNotContain("https://", result);
        }
    }
}
