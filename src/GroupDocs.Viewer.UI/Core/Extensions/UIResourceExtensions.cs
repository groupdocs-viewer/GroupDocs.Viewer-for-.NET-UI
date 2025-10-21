using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using GroupDocs.Viewer.UI.Configuration;
using GroupDocs.Viewer.UI.Core.Configuration;
using Microsoft.AspNetCore.Http;

namespace GroupDocs.Viewer.UI.Core.Extensions
{
    internal static class UIResourceExtensions
    {
        public static UIResource GetIndexPage(this IEnumerable<UIResource> resources)
        {
            var index = resources
                .FirstOrDefault(r => 
                    r.ContentType == ContentType.HTML && r.FileName == Keys.GROUPDOCSVIEWERUI_MAIN_UI_RESOURCE);

            return index;
        }

        public static string GetIndexPageHtml(this UIResource index,
            Options options, Config config, PathString pathBase, Dictionary<string, string> routeValues)
        {
            var uiPath = options.UIPath
                .AppendPathBase(pathBase)
                .ReplacePatternsWithRouteValues(routeValues)
                .WithTrailingSlash();

            var html = new StringBuilder(index.GetContentString());

            html.Replace(Keys.GROUPDOCSVIEWERUI_MAIN_UI_PATH, uiPath);
            html.Replace(Keys.GROUPDOCSVIEWERUI_MAIN_UI_TITLE, options.UITitle);

            var apiEndpoint = options.ApiEndpoint
                .AppendPathBase(pathBase)
                .ReplacePatternsWithRouteValues(routeValues)
                .TrimTrailingSlash();

            string uiConfig = SerializeWindowConfig(apiEndpoint, config);

            html.Replace(Keys.GROUPDOCSVIEWERUI_MAIN_UI_CONFIG, uiConfig);

            return html.ToString();
        }

        private static string SerializeWindowConfig(string apiEndpoint, Config config)
        {
            var apiConfig = config.StaticContentMode
                ? new
                {
                    filesTree = "/list-dir.json",
                    viewData = "/view-data.json",
                    printPdf = "/print.pdf"
                } as object
                : new
                {
                    filesTree = "/list-dir",
                    uploadFile = "/upload-file",
                    viewData = "/view-data",
                    createPages = "/create-pages",
                    createPdf = "/create-pdf",
                    printPdf = "/print-pdf"
                };

            var windowConfig = new
            {
                apiEndpoint = apiEndpoint,
                renderingMode = config.RenderingMode.Value,
                staticContentMode = config.StaticContentMode,
                initialFile = config.InitialFile,
                preloadPages = config.PreloadPages,
                initialZoom = config.InitialZoom?.Value,
                enableHeader = config.EnableHeader,
                enableToolbar = config.EnableToolbar,
                enablePageSelector = config.EnablePageSelector,
                enableDownloadPdf = config.EnableDownloadPdf,
                enableFileUpload = config.EnableFileUpload,
                enableFileBrowser = config.EnableFileBrowser,
                enableContextMenu = config.EnableContextMenu,
                enableZoom = config.EnableZoom,
                enableSearch = config.EnableSearch,
                enableFileName = config.EnableFileName,
                enableThumbnails = config.EnableThumbnails,
                enablePrint = config.EnablePrint,
                enablePresentation = config.EnablePresentation,
                enableHyperlinks = config.EnableHyperlinks,
                enableScrollAnimation = config.EnableScrollAnimation,
                enableLanguageSelector = config.EnableLanguageSelector,
                defaultLanguage = config.DefaultLanguage.Value,
                supportedLanguages = config.SupportedLanguages.Select(lang => lang.Value).ToArray(),
                showExitButton = false,
                apiEndpointConfig = apiConfig
            };

            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true, 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            };

            string json = JsonSerializer.Serialize(windowConfig, jsonOptions);
            return json;
        }

        public static ICollection<UIStylesheet> GetCustomStylesheets(this UIResource resource, Options options)
        {
            List<UIStylesheet> styleSheets = new List<UIStylesheet>();

            string content = resource.GetContentString();

            if (!options.CustomStylesheets.Any())
            {
                content = content.Replace(Keys.GROUPDOCSVIEWERUI_STYLESHEETS_TARGET, string.Empty);
                resource.SetContentString(content);
                return styleSheets;
            }

            foreach (var stylesheet in options.CustomStylesheets)
            {
                styleSheets.Add(UIStylesheet.Create(options, stylesheet));
            }

            var htmlStyles = styleSheets.Select(s =>
            {
                var linkHref = s.ResourceRelativePath.AsRelativeResource();
                return $"<link rel='stylesheet' href='{linkHref}'/>";
            });

            content = content.Replace(Keys.GROUPDOCSVIEWERUI_STYLESHEETS_TARGET,
                string.Join("\n", htmlStyles));

            resource.SetContentString(content);

            return styleSheets;
        }
    }
}
