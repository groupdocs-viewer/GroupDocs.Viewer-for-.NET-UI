using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using ApiConfig = GroupDocs.Viewer.UI.SelfHost.Api.Configuration.Config;

namespace GroupDocs.Viewer.UI.Api.Tests.Validation
{
    public class ConfigurationValidatorTests
    {
        [Fact]
        public async Task StartAsync_WhenConfigsMatch_ShouldNotLogWarning()
        {
            var apiConfig = new ApiConfig();
            apiConfig.SetViewerType(ViewerType.HtmlWithEmbeddedResources);

            var uiConfig = new Config { RenderingMode = RenderingMode.Html };

            var logger = new Mock<ILogger<SelfHost.Api.Validation.ConfigurationValidator>>();

            var validator = new SelfHost.Api.Validation.ConfigurationValidator(
                MSOptions.Create(apiConfig),
                MSOptions.Create(uiConfig),
                logger.Object);

            await validator.StartAsync(default);

            logger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }

        [Fact]
        public async Task StartAsync_WhenConfigsMismatch_ShouldLogWarning()
        {
            var apiConfig = new ApiConfig();
            apiConfig.SetViewerType(ViewerType.Png);

            var uiConfig = new Config { RenderingMode = RenderingMode.Html };

            var logger = new Mock<ILogger<SelfHost.Api.Validation.ConfigurationValidator>>();

            var validator = new SelfHost.Api.Validation.ConfigurationValidator(
                MSOptions.Create(apiConfig),
                MSOptions.Create(uiConfig),
                logger.Object);

            await validator.StartAsync(default);

            logger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
