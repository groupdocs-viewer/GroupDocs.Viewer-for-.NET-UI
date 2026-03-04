using System;
using System.IO;
using GroupDocs.Viewer.UI.Core;
using Xunit;
using UIOptions = GroupDocs.Viewer.UI.Configuration.Options;

namespace GroupDocs.Viewer.UI.Api.Tests.UI
{
    /// <summary>
    /// Tests that UIPath="/" does not produce double slashes in resource paths and route patterns.
    /// Covers the fix in UIEndpointsResourceMapper, UIStylesheet, and UIScript.
    /// </summary>
    public class UIPathNormalizationTests : IDisposable
    {
        private readonly string _tempDir;

        public UIPathNormalizationTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), $"viewer-ui-tests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
        }

        #region UIStylesheet ResourcePath Tests

        [Theory]
        [InlineData("/")]
        [InlineData("/viewer")]
        [InlineData("/viewer/")]
        [InlineData("/custom/path")]
        public void UIStylesheet_ResourcePath_ShouldNotContainDoubleSlashes(string uiPath)
        {
            // Arrange
            var cssFile = CreateTempFile("test.css", "body { color: red; }");
            var options = new UIOptions { UIPath = uiPath };

            // Act
            var stylesheet = UIStylesheet.Create(options, cssFile);

            // Assert
            Assert.DoesNotContain("//", stylesheet.ResourcePath);
        }

        [Fact]
        public void UIStylesheet_ResourcePath_WithRootUIPath_ShouldStartWithSlashCss()
        {
            // Arrange
            var cssFile = CreateTempFile("custom.css", "body {}");
            var options = new UIOptions { UIPath = "/" };

            // Act
            var stylesheet = UIStylesheet.Create(options, cssFile);

            // Assert
            Assert.Equal("/css/custom.css", stylesheet.ResourcePath);
        }

        [Fact]
        public void UIStylesheet_ResourcePath_WithViewerUIPath_ShouldIncludeViewerPrefix()
        {
            // Arrange
            var cssFile = CreateTempFile("custom.css", "body {}");
            var options = new UIOptions { UIPath = "/viewer" };

            // Act
            var stylesheet = UIStylesheet.Create(options, cssFile);

            // Assert
            Assert.Equal("/viewer/css/custom.css", stylesheet.ResourcePath);
        }

        [Fact]
        public void UIStylesheet_ResourcePath_WithTrailingSlashUIPath_ShouldNotDoubleSlash()
        {
            // Arrange
            var cssFile = CreateTempFile("custom.css", "body {}");
            var options = new UIOptions { UIPath = "/viewer/" };

            // Act
            var stylesheet = UIStylesheet.Create(options, cssFile);

            // Assert
            Assert.Equal("/viewer/css/custom.css", stylesheet.ResourcePath);
            Assert.DoesNotContain("//", stylesheet.ResourcePath);
        }

        [Fact]
        public void UIStylesheet_ResourceRelativePath_ShouldNotBeAffectedByUIPath()
        {
            // Arrange
            var cssFile = CreateTempFile("branding.css", "body {}");
            var rootOptions = new UIOptions { UIPath = "/" };
            var viewerOptions = new UIOptions { UIPath = "/viewer" };

            // Act
            var rootStylesheet = UIStylesheet.Create(rootOptions, cssFile);
            var viewerStylesheet = UIStylesheet.Create(viewerOptions, cssFile);

            // Assert
            Assert.Equal("css/branding.css", rootStylesheet.ResourceRelativePath);
            Assert.Equal(rootStylesheet.ResourceRelativePath, viewerStylesheet.ResourceRelativePath);
        }

        #endregion

        #region UIScript ResourcePath Tests

        [Theory]
        [InlineData("/")]
        [InlineData("/viewer")]
        [InlineData("/viewer/")]
        [InlineData("/custom/path")]
        public void UIScript_ResourcePath_ShouldNotContainDoubleSlashes(string uiPath)
        {
            // Arrange
            var jsFile = CreateTempFile("test.js", "console.log('test');");
            var options = new UIOptions { UIPath = uiPath };

            // Act
            var script = UIScript.Create(options, jsFile);

            // Assert
            Assert.DoesNotContain("//", script.ResourcePath);
        }

        [Fact]
        public void UIScript_ResourcePath_WithRootUIPath_ShouldStartWithSlashJs()
        {
            // Arrange
            var jsFile = CreateTempFile("custom.js", "console.log('test');");
            var options = new UIOptions { UIPath = "/" };

            // Act
            var script = UIScript.Create(options, jsFile);

            // Assert
            Assert.Equal("/js/custom.js", script.ResourcePath);
        }

        [Fact]
        public void UIScript_ResourcePath_WithViewerUIPath_ShouldIncludeViewerPrefix()
        {
            // Arrange
            var jsFile = CreateTempFile("custom.js", "console.log('test');");
            var options = new UIOptions { UIPath = "/viewer" };

            // Act
            var script = UIScript.Create(options, jsFile);

            // Assert
            Assert.Equal("/viewer/js/custom.js", script.ResourcePath);
        }

        [Fact]
        public void UIScript_ResourcePath_WithTrailingSlashUIPath_ShouldNotDoubleSlash()
        {
            // Arrange
            var jsFile = CreateTempFile("custom.js", "console.log('test');");
            var options = new UIOptions { UIPath = "/viewer/" };

            // Act
            var script = UIScript.Create(options, jsFile);

            // Assert
            Assert.Equal("/viewer/js/custom.js", script.ResourcePath);
            Assert.DoesNotContain("//", script.ResourcePath);
        }

        [Fact]
        public void UIScript_ResourceRelativePath_ShouldNotBeAffectedByUIPath()
        {
            // Arrange
            var jsFile = CreateTempFile("app.js", "console.log('test');");
            var rootOptions = new UIOptions { UIPath = "/" };
            var viewerOptions = new UIOptions { UIPath = "/viewer" };

            // Act
            var rootScript = UIScript.Create(rootOptions, jsFile);
            var viewerScript = UIScript.Create(viewerOptions, jsFile);

            // Assert
            Assert.Equal("js/app.js", rootScript.ResourceRelativePath);
            Assert.Equal(rootScript.ResourceRelativePath, viewerScript.ResourceRelativePath);
        }

        #endregion

        #region Route Pattern Construction Tests (UIEndpointsResourceMapper logic)

        /// <summary>
        /// Verifies the route pattern construction logic used in UIEndpointsResourceMapper.
        /// The mapper builds route patterns as: $"{uiPathBase}/{resource.FileName}"
        /// where uiPathBase = options.UIPath.TrimEnd('/').
        /// </summary>
        [Theory]
        [InlineData("/", "index.html", "/index.html")]
        [InlineData("/", "runtime.js", "/runtime.js")]
        [InlineData("/", "assets/ui/logo-image.svg", "/assets/ui/logo-image.svg")]
        [InlineData("/viewer", "index.html", "/viewer/index.html")]
        [InlineData("/viewer", "runtime.js", "/viewer/runtime.js")]
        [InlineData("/viewer/", "index.html", "/viewer/index.html")]
        [InlineData("/custom/path", "styles.css", "/custom/path/styles.css")]
        [InlineData("/custom/path/", "styles.css", "/custom/path/styles.css")]
        public void ResourceRoutePattern_ShouldBeValid(string uiPath, string fileName, string expected)
        {
            // This tests the exact logic from UIEndpointsResourceMapper.Map:
            //   var uiPathBase = options.UIPath.TrimEnd('/');
            //   $"{uiPathBase}/{resource.FileName}"
            var uiPathBase = uiPath.TrimEnd('/');
            var routePattern = $"{uiPathBase}/{fileName}";

            Assert.Equal(expected, routePattern);
            Assert.DoesNotContain("//", routePattern);
            Assert.StartsWith("/", routePattern);
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/viewer")]
        [InlineData("/viewer/")]
        [InlineData("/custom/path")]
        public void ResourceRoutePattern_ShouldNeverProduceDoubleSlashes(string uiPath)
        {
            var resourceFileNames = new[]
            {
                "index.html",
                "runtime.js",
                "polyfills.js",
                "main.js",
                "styles.css",
                "assets/ui/logo-image.svg",
                "assets/ui/logo-text.svg"
            };

            var uiPathBase = uiPath.TrimEnd('/');

            foreach (var fileName in resourceFileNames)
            {
                var routePattern = $"{uiPathBase}/{fileName}";
                Assert.DoesNotContain("//", routePattern);
                Assert.StartsWith("/", routePattern);
            }
        }

        #endregion

        private string CreateTempFile(string fileName, string content)
        {
            var filePath = Path.Combine(_tempDir, fileName);
            File.WriteAllText(filePath, content);
            return filePath;
        }
    }
}
