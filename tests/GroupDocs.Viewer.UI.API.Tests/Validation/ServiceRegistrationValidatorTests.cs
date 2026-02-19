using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.SelfHost.Api.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Validation
{
    public class ServiceRegistrationValidatorTests
    {
        [Fact]
        public async Task StartAsync_WhenFileStorageMissing_ShouldLogWarning()
        {
            var services = new ServiceCollection();
            var provider = services.BuildServiceProvider();
            var logger = new Mock<ILogger<ServiceRegistrationValidator>>();

            var validator = new ServiceRegistrationValidator(provider, logger.Object);

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

        [Fact]
        public async Task StartAsync_WhenFileStorageRegistered_ShouldNotLogWarning()
        {
            var services = new ServiceCollection();
            var mockStorage = new Mock<IFileStorage>();
            services.AddSingleton(mockStorage.Object);
            var provider = services.BuildServiceProvider();
            var logger = new Mock<ILogger<ServiceRegistrationValidator>>();

            var validator = new ServiceRegistrationValidator(provider, logger.Object);

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
    }
}
