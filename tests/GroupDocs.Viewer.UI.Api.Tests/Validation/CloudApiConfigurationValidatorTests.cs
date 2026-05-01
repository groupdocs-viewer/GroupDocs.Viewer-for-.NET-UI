using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using GroupDocs.Viewer.UI.Cloud.Api.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Validation
{
    public class CloudApiConfigurationValidatorTests
    {
        [Fact]
        public async Task StartAsync_WhenFullyConfigured_ShouldNotLogWarning()
        {
            var config = new Config();
            config.SetClientId("test-client-id");
            config.SetClientSecret("test-client-secret");

            var logger = new Mock<ILogger<ConfigurationValidator>>();

            var validator = new ConfigurationValidator(MSOptions.Create(config), logger.Object);

            await validator.StartAsync(default);

            logger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never);
        }

        [Fact]
        public async Task StartAsync_WhenCredentialsMissing_ShouldLogWarningPerMissingValue()
        {
            var config = new Config();

            var logger = new Mock<ILogger<ConfigurationValidator>>();

            var validator = new ConfigurationValidator(MSOptions.Create(config), logger.Object);

            await validator.StartAsync(default);

            logger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Exactly(2));
        }

        [Fact]
        public async Task StartAsync_WhenApiEndpointInvalid_ShouldLogWarning()
        {
            var config = new Config();
            config.SetClientId("test-client-id");
            config.SetClientSecret("test-client-secret");
            config.SetApiEndpoint("not-a-valid-uri");

            var logger = new Mock<ILogger<ConfigurationValidator>>();

            var validator = new ConfigurationValidator(MSOptions.Create(config), logger.Object);

            await validator.StartAsync(default);

            logger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}
