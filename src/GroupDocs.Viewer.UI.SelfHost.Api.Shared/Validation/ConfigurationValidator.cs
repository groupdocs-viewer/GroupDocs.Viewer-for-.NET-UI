using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ApiConfig = GroupDocs.Viewer.UI.SelfHost.Api.Configuration.Config;
using UIConfig = GroupDocs.Viewer.UI.Core.Configuration.Config;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Validation
{
    public class ConfigurationValidator : IHostedService
    {
        private readonly IOptions<ApiConfig> _apiConfig;
        private readonly IOptions<UIConfig> _uiConfig;
        private readonly ILogger<ConfigurationValidator> _logger;

        public ConfigurationValidator(
            IOptions<ApiConfig> apiConfig,
            IOptions<UIConfig> uiConfig,
            ILogger<ConfigurationValidator> logger)
        {
            _apiConfig = apiConfig;
            _uiConfig = uiConfig;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var expectedRenderingMode = _apiConfig.Value.ViewerType.ToRenderingMode();
            var actualRenderingMode = _uiConfig.Value.RenderingMode;

            if (expectedRenderingMode != actualRenderingMode)
            {
                _logger.LogWarning(
                    "ViewerType is set to {ViewerType} (expects RenderingMode.{ExpectedMode}) but UI RenderingMode is set to {ActualMode}. " +
                    "Set RenderingMode to {CorrectMode} in AddGroupDocsViewerUI() or use viewerType.ToRenderingMode().",
                    _apiConfig.Value.ViewerType,
                    expectedRenderingMode,
                    actualRenderingMode,
                    expectedRenderingMode);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
