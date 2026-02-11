using GroupDocs.Viewer.UI.Api.Configuration;
using GroupDocs.Viewer.UI.Api.Utils;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Utils
{
    /// <summary>
    /// Tests for path normalization edge cases - trailing slashes, leading slashes, etc.
    /// </summary>
    public class ApiUrlBuilderPathNormalizationTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IOptionsProvider> _optionsProviderMock;
        private readonly Mock<HttpContext> _httpContextMock;
        private readonly Mock<HttpRequest> _httpRequestMock;

        public ApiUrlBuilderPathNormalizationTests()
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

        [Theory]
        [InlineData("/viewer-api", "/get-page")]
        [InlineData("/viewer-api/", "/get-page")]
        [InlineData("viewer-api", "/get-page")]
        [InlineData("viewer-api/", "/get-page")]
        public void BuildPageUrl_WithDifferentPathFormats_ShouldIgnoreApiPathWhenDomainNotSet(string apiPath, string expectedPath)
        {
            // Arrange
            var builder = CreateBuilder(apiPath: apiPath);
            var file = "test.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // When apiDomain is not set, apiPath should be ignored
            Assert.StartsWith(expectedPath, result);
            Assert.DoesNotContain("/viewer-api/", result); // apiPath should be ignored
            Assert.DoesNotContain("//", result); // No double slashes
        }

        [Fact]
        public void BuildPageUrl_WithTrailingSlashInApiPath_ShouldIgnoreApiPathWhenDomainNotSet()
        {
            // Arrange
            var builder = CreateBuilder(apiPath: "/viewer-api/");
            var file = "test.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // When apiDomain is not set, apiPath should be ignored
            Assert.Equal("/get-page?file=test.docx&page=1", result);
            Assert.DoesNotContain("/viewer-api/", result); // apiPath should be ignored
        }

        [Fact]
        public void BuildPageUrl_WithoutLeadingSlashInApiPath_ShouldIgnoreApiPathWhenDomainNotSet()
        {
            // Arrange
            var builder = CreateBuilder(apiPath: "viewer-api");
            var file = "test.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // When apiDomain is not set, apiPath should be ignored
            Assert.StartsWith("/get-page", result);
            Assert.DoesNotContain("/viewer-api/", result); // apiPath should be ignored
        }

        [Fact]
        public void BuildPageUrl_WithNestedApiPath_ShouldIgnoreApiPathWhenDomainNotSet()
        {
            // Arrange
            var builder = CreateBuilder(apiPath: "/api/v1/viewer");
            var file = "test.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // When apiDomain is not set, apiPath should be ignored
            Assert.StartsWith("/get-page", result);
            Assert.DoesNotContain("/api/v1/viewer/", result); // apiPath should be ignored
        }

        [Fact]
        public void BuildPageUrl_WithRootApiPath_ShouldIgnoreApiPathWhenDomainNotSet()
        {
            // Arrange
            var builder = CreateBuilder(apiPath: "/");
            var file = "test.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // When apiDomain is not set, apiPath should be ignored
            Assert.Equal("/get-page?file=test.docx&page=1", result);
            Assert.DoesNotContain("//", result); // No double slash
        }

        [Fact]
        public void BuildPageUrl_WithEmptyApiPath_ShouldNotThrowWhenDomainNotSet()
        {
            // Arrange
            var options = new Options
            {
                ApiPath = "",
                ApiDomain = null
            };

            _optionsProviderMock.Setup(x => x.GetOptions()).Returns(options);
            var builder = new ApiUrlBuilder(_httpContextAccessorMock.Object, _optionsProviderMock.Object);
            var file = "test.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // When apiDomain is not set, apiPath is ignored, so empty apiPath should not throw
            Assert.StartsWith("/get-page", result);
        }

        [Fact]
        public void BuildPageUrl_WithWhitespaceApiPath_ShouldNotThrowWhenDomainNotSet()
        {
            // Arrange
            var options = new Options
            {
                ApiPath = "   ",
                ApiDomain = null
            };

            _optionsProviderMock.Setup(x => x.GetOptions()).Returns(options);
            var builder = new ApiUrlBuilder(_httpContextAccessorMock.Object, _optionsProviderMock.Object);
            var file = "test.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // When apiDomain is not set, apiPath is ignored, so whitespace apiPath should not throw
            Assert.StartsWith("/get-page", result);
        }

        [Fact]
        public void BuildPageUrl_WithMultipleSlashes_ShouldIgnoreApiPathWhenDomainNotSet()
        {
            // Arrange
            var builder = CreateBuilder(apiPath: "///viewer-api///");
            var file = "test.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // When apiDomain is not set, apiPath should be ignored
            Assert.StartsWith("/get-page", result);
            Assert.DoesNotContain("/viewer-api/", result); // apiPath should be ignored
            Assert.DoesNotContain("///", result);
        }

        [Fact]
        public void BuildPageUrl_WithDomainAndEmptyApiPath_ShouldThrowException()
        {
            // Arrange
            var options = new Options
            {
                ApiPath = "",
                ApiDomain = "https://example.com",
                UseAbsoluteUrls = true
            };

            _optionsProviderMock.Setup(x => x.GetOptions()).Returns(options);
            var builder = new ApiUrlBuilder(_httpContextAccessorMock.Object, _optionsProviderMock.Object);
            var file = "test.docx";
            var page = 1;
            var extension = "html";

            // Act & Assert
            // When UseAbsoluteUrls is true, apiPath is required
            Assert.Throws<ArgumentNullException>(() => builder.BuildPageUrl(file, page, extension));
        }

        [Fact]
        public void BuildPageUrl_WithDomainAndApiPath_ShouldIncludeApiPath()
        {
            // Arrange
            var builder = CreateBuilder(apiPath: "/viewer-api", apiDomain: "https://example.com", useAbsoluteUrls: true);
            var file = "test.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // When UseAbsoluteUrls is true, apiPath should be included
            Assert.StartsWith("https://example.com/viewer-api/get-page", result);
        }

        [Fact]
        public void BuildPageUrl_WithDomainButUseAbsoluteUrlsFalse_ShouldReturnRelativeUrl()
        {
            // Arrange
            var builder = CreateBuilder(apiPath: "/viewer-api", apiDomain: "https://example.com", useAbsoluteUrls: false);
            var file = "test.docx";
            var page = 1;
            var extension = "html";

            // Act
            var result = builder.BuildPageUrl(file, page, extension);

            // Assert
            // When UseAbsoluteUrls is false, should return relative URL even if ApiDomain is set
            Assert.StartsWith("/get-page", result);
            Assert.DoesNotContain("example.com", result);
            Assert.DoesNotContain("/viewer-api/", result);
        }
    }
}
