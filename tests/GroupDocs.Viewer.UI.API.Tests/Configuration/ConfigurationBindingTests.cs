using System.Collections.Generic;
using GroupDocs.Viewer.UI.Core.Configuration;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Configuration
{
    public class ConfigurationBindingTests
    {
        [Fact]
        public void BindUISettings_ShouldBindSimpleProperties()
        {
            var configData = new Dictionary<string, string>
            {
                ["GroupDocsViewerUI:PreloadPages"] = "5",
                ["GroupDocsViewerUI:EnableThumbnails"] = "false",
                ["GroupDocsViewerUI:ResponseCacheDurationSeconds"] = "3600",
                ["GroupDocsViewerUI:EnableHeader"] = "false",
                ["GroupDocsViewerUI:EnableFileBrowser"] = "false",
                ["GroupDocsViewerUI:EnableSearch"] = "false",
                ["GroupDocsViewerUI:EnableHyperlinks"] = "false",
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData!)
                .Build();

            var config = new Config();
            configuration.BindUISettings(config);

            Assert.Equal(5, config.PreloadPages);
            Assert.False(config.EnableThumbnails);
            Assert.Equal(3600, config.ResponseCacheDurationSeconds);
            Assert.False(config.EnableHeader);
            Assert.False(config.EnableFileBrowser);
            Assert.False(config.EnableSearch);
            Assert.False(config.EnableHyperlinks);
        }

        [Fact]
        public void BindUISettings_WithEmptySection_ShouldRetainDefaults()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>()!)
                .Build();

            var config = new Config();
            configuration.BindUISettings(config);

            Assert.Equal("html", config.RenderingMode.Value);
            Assert.Equal(3, config.PreloadPages);
            Assert.True(config.EnableThumbnails);
            Assert.Equal(0, config.ResponseCacheDurationSeconds);
        }

        [Fact]
        public void BindSelfHostApiSettings_ShouldNotThrow()
        {
            var configData = new Dictionary<string, string>
            {
                ["GroupDocsViewerUISelfHostApi:ViewerType"] = "Png",
                ["GroupDocsViewerUISelfHostApi:LicensePath"] = "/path/to/license.lic",
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData!)
                .Build();

            var apiConfig = new GroupDocs.Viewer.UI.SelfHost.Api.Configuration.Config();
            configuration.BindSelfHostApiSettings(apiConfig);
        }
    }
}
