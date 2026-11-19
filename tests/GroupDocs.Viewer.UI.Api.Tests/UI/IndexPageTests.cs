using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using UIOptions = GroupDocs.Viewer.UI.Configuration.Options;

namespace GroupDocs.Viewer.UI.Api.Tests.UI
{
    /// <summary>
    /// The index page passes the settings to the UI as <c>window.groupdocs.viewer</c> and links custom stylesheets and scripts.
    /// The UI fails to start when <c>enableRotate</c> or <c>enableThemeSwitcher</c> is not a boolean,
    /// or <c>defaultTheme</c> is not "auto", "light" or "dark".
    /// </summary>
    public class IndexPageTests : IDisposable
    {
        private readonly string _tempDir;

        public IndexPageTests()
        {
            _tempDir = Path.Combine(Path.GetTempPath(), $"viewer-ui-tests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(_tempDir);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDir))
                Directory.Delete(_tempDir, recursive: true);
        }

        [Fact]
        public async Task IndexPage_WithDefaultConfig_ShouldEnableRotateAndThemeSwitcherAndUseLightTheme()
        {
            using var uiConfig = ParseUIConfig(await GetIndexPageHtmlAsync(_ => { }));

            Assert.True(uiConfig.RootElement.GetProperty("enableRotate").GetBoolean());
            Assert.True(uiConfig.RootElement.GetProperty("enableThemeSwitcher").GetBoolean());
            Assert.Equal("light", uiConfig.RootElement.GetProperty("defaultTheme").GetString());
        }

        [Fact]
        public async Task IndexPage_WithRotateAndThemeSettings_ShouldPassSettingsToUI()
        {
            using var uiConfig = ParseUIConfig(await GetIndexPageHtmlAsync(config =>
            {
                config.EnableRotate = false;
                config.EnableThemeSwitcher = false;
                config.DefaultTheme = Theme.Dark;
            }));

            Assert.False(uiConfig.RootElement.GetProperty("enableRotate").GetBoolean());
            Assert.False(uiConfig.RootElement.GetProperty("enableThemeSwitcher").GetBoolean());
            Assert.Equal("dark", uiConfig.RootElement.GetProperty("defaultTheme").GetString());
        }

        [Fact]
        public async Task IndexPage_WithAutoTheme_ShouldPassAutoTheme()
        {
            using var uiConfig = ParseUIConfig(await GetIndexPageHtmlAsync(config => config.DefaultTheme = Theme.Auto));

            Assert.Equal("auto", uiConfig.RootElement.GetProperty("defaultTheme").GetString());
        }

        [Fact]
        public async Task IndexPage_WithNullDefaultTheme_ShouldUseLightTheme()
        {
            using var uiConfig = ParseUIConfig(await GetIndexPageHtmlAsync(config => config.DefaultTheme = null!));

            Assert.Equal("light", uiConfig.RootElement.GetProperty("defaultTheme").GetString());
        }

        [Fact]
        public async Task IndexPage_WithCustomStylesheetAndScript_ShouldLinkThemAroundAppResources()
        {
            var stylesheet = CreateTempFile("branding.css", ":root { --c-bg-brand: #0d9488; }");
            var script = CreateTempFile("behavior.js", "console.log('custom');");

            var html = await GetIndexPageHtmlAsync(_ => { }, options =>
            {
                options.AddCustomStylesheet(stylesheet);
                options.AddCustomScript(script);
            });

            var appStylesheet = html.IndexOf("href=\"styles.css\"", StringComparison.Ordinal);
            var customStylesheet = html.IndexOf("href='css/branding.css'", StringComparison.Ordinal);
            var customScript = html.IndexOf("src='js/behavior.js'", StringComparison.Ordinal);
            var appScript = html.IndexOf("src=\"main.js\"", StringComparison.Ordinal);

            Assert.True(appStylesheet >= 0 && customStylesheet > appStylesheet,
                "Custom stylesheets must follow the app stylesheet to override its styles.");
            Assert.True(customScript >= 0 && appScript > customScript,
                "Custom scripts must come before the app scripts.");
            Assert.DoesNotMatch("#(uiTitle|uiPath|uiConfig|customCSS|customJS)#", html);
        }

        private static async Task<string> GetIndexPageHtmlAsync(Action<Config> setupConfig, Action<UIOptions>? setupOptions = null)
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services.AddGroupDocsViewerUI(setupConfig);

            await using var app = builder.Build();
            app.MapGroupDocsViewerUI(setupOptions);

            var indexPage = ((IEndpointRouteBuilder)app).DataSources
                .SelectMany(dataSource => dataSource.Endpoints)
                .OfType<RouteEndpoint>()
                .Single(endpoint => endpoint.RoutePattern.RawText == "/viewer");

            var body = new MemoryStream();
            var context = new DefaultHttpContext { RequestServices = app.Services };
            context.Response.Body = body;

            await indexPage.RequestDelegate!(context);

            return Encoding.UTF8.GetString(body.ToArray());
        }

        private static JsonDocument ParseUIConfig(string html)
        {
            const string configStart = "window.groupdocs.viewer = ";
            var start = html.IndexOf(configStart, StringComparison.Ordinal);
            Assert.True(start >= 0, "The index page doesn't pass the settings to the UI.");
            start += configStart.Length;
            var end = html.IndexOf("</script>", start, StringComparison.Ordinal);

            return JsonDocument.Parse(html.Substring(start, end - start).Trim().TrimEnd(';'));
        }

        private string CreateTempFile(string fileName, string content)
        {
            var filePath = Path.Combine(_tempDir, fileName);
            File.WriteAllText(filePath, content);
            return filePath;
        }
    }
}
