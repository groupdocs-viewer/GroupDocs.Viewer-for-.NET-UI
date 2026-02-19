using System;
using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Validation
{
    public class ServiceRegistrationValidator : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ServiceRegistrationValidator> _logger;

        public ServiceRegistrationValidator(
            IServiceProvider serviceProvider,
            ILogger<ServiceRegistrationValidator> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var fileStorage = scope.ServiceProvider.GetService<IFileStorage>();
            if (fileStorage == null)
            {
                _logger.LogWarning(
                    "No IFileStorage implementation is registered. " +
                    "Add a storage provider after AddGroupDocsViewerSelfHostApi(), e.g. .AddLocalStorage(\"./Files\").");
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
