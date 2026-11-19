using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Xunit;
using UIOptions = GroupDocs.Viewer.UI.Configuration.Options;

namespace GroupDocs.Viewer.UI.Api.Tests.UI
{
    /// <summary>
    /// The Angular app in src/GroupDocs.Viewer.UI/App is built in a separate repository and copied in by hand.
    /// These tests catch the mistakes that copying is prone to.
    /// </summary>
    public class EmbeddedAppTests
    {
        // NOTE: security scanners such as OWASP ZAP detect Angular by the "ng-version" string in the app.
        // Angular 19.1.7 and 22.0.x are flagged, e.g. by GHSA-hh8m-fm6v-7cvg, and 22.2.6 had no known advisories
        // when the app was built with it. An app built before `npm ci` brought node_modules up to date fails here.
        private static readonly Version MinAngularVersion = new Version(22, 2, 6);

        private const string AppResourcePrefix = "GroupDocs.Viewer.UI.App.";

        private static readonly Assembly UIAssembly = typeof(UIOptions).Assembly;

        [Fact]
        public void IndexPage_ShouldContainPlaceholdersReplacedByUI()
        {
            var html = ReadResource(AppResourcePrefix + "index.html");

            Assert.Contains("#uiTitle#", html);
            Assert.Contains("#uiPath#", html);
            Assert.Contains("#uiConfig#", html);
            Assert.Contains("#customCSS#", html);
            Assert.Contains("#customJS#", html);
            Assert.True(html.IndexOf("#customCSS#", StringComparison.Ordinal) > html.IndexOf("href=\"styles.css\"", StringComparison.Ordinal),
                "Custom stylesheets must follow the app stylesheet to override its styles.");
            Assert.True(html.IndexOf("#customJS#", StringComparison.Ordinal) < html.IndexOf("src=\"main.js\"", StringComparison.Ordinal),
                "Custom scripts must come before the app scripts.");
        }

        [Fact]
        public void EmbeddedAngular_ShouldNotBeOlderThanMinVersion()
        {
            var versions = ReadAppScripts()
                .SelectMany(script => Regex.Matches(script, "\"ng-version\",\"(\\d+\\.\\d+\\.\\d+)\"")
                    .Select(match => match.Groups[1].Value))
                .Distinct()
                .ToList();

            var version = Assert.Single(versions);
            Assert.True(new Version(version) >= MinAngularVersion,
                $"The app is built with Angular {version}, expected {MinAngularVersion} or later.");
        }

        // NOTE: SetLogoImage, SetLogoText, HideLogoImage and HideLogoText replace these files,
        // so they have no effect when the app doesn't load them
        [Theory]
        [InlineData("assets/ui/logo-image.svg")]
        [InlineData("assets/ui/logo-text.svg")]
        public void EmbeddedApp_ShouldLoadLogoFileThatUICanReplace(string logoFile)
        {
            Assert.Contains(ReadAppScripts(), script => script.Contains(logoFile, StringComparison.Ordinal));
        }

        private static string[] ReadAppScripts() =>
            UIAssembly.GetManifestResourceNames()
                .Where(name => name.StartsWith(AppResourcePrefix, StringComparison.Ordinal)
                    && name.EndsWith(".js", StringComparison.Ordinal))
                .Select(ReadResource)
                .ToArray();

        private static string ReadResource(string name)
        {
            using var stream = UIAssembly.GetManifestResourceStream(name);
            Assert.NotNull(stream);

            using var reader = new StreamReader(stream!);
            return reader.ReadToEnd();
        }
    }
}
