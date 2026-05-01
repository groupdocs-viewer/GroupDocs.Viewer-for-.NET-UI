using System;
using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.Cloud.Api.Validation
{
    public class ConfigurationValidator : IHostedService
    {
        private readonly IOptions<Config> _config;
        private readonly ILogger<ConfigurationValidator> _logger;

        public ConfigurationValidator(
            IOptions<Config> config,
            ILogger<ConfigurationValidator> logger)
        {
            _config = config;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var config = _config.Value;

            if (string.IsNullOrWhiteSpace(config.ClientId))
            {
                _logger.LogWarning(
                    "Cloud.Api ClientId is not configured. Set it via AddGroupDocsViewerCloudApi(c => c.SetClientId(\"...\")) " +
                    "or the GroupDocsViewerUICloudApi:ClientId configuration key. " +
                    "Get credentials at https://dashboard.groupdocs.cloud/applications.");
            }

            if (string.IsNullOrWhiteSpace(config.ClientSecret))
            {
                _logger.LogWarning(
                    "Cloud.Api ClientSecret is not configured. Set it via AddGroupDocsViewerCloudApi(c => c.SetClientSecret(\"...\")) " +
                    "or the GroupDocsViewerUICloudApi:ClientSecret configuration key.");
            }

            if (string.IsNullOrWhiteSpace(config.ApiEndpoint) ||
                !Uri.TryCreate(config.ApiEndpoint, UriKind.Absolute, out _))
            {
                _logger.LogWarning(
                    "Cloud.Api ApiEndpoint is missing or not a valid absolute URI (current value: '{ApiEndpoint}').",
                    config.ApiEndpoint);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
