using System;
using System.Collections.Generic;
using System.Linq;
using GroupDocs.Viewer.UI.Configuration;

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

        public static string SetMainUIResourcePaths(this UIResource index, 
            Options options, Dictionary<string, string> routeValues)
        {
            var uiPath = options.UIPath
                .ReplacePatternsWithRouteValues(routeValues)
                .WithTrailingSlash();

            index.Content = index.Content
                .Replace(Keys.GROUPDOCSVIEWERUI_MAIN_UI_PATH, uiPath);

            var apiPath = options.APIEndpoint
                .ReplacePatternsWithRouteValues(routeValues)
                .TrimTrailingSlash();

            index.Content = index.Content
                .Replace(Keys.GROUPDOCSVIEWERUI_MAIN_UI_API_TARGET, apiPath);

            var uiConfigPath =
                options.UIConfigEndpoint
                    .ReplacePatternsWithRouteValues(routeValues);

            index.Content = index.Content
                .Replace(Keys.GROUPDOCSVIEWERUI_MAIN_UI_SETTINGS_PATH_TARGET, uiConfigPath);

            return index.Content;
        }

        public static ICollection<UIStylesheet> GetCustomStylesheets(this UIResource resource, Options options)
        {
            List<UIStylesheet> styleSheets = new List<UIStylesheet>();

            if (!options.CustomStylesheets.Any())
            {
                resource.Content = resource.Content.Replace(Keys.GROUPDOCSVIEWERUI_STYLESHEETS_TARGET, string.Empty);
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

            resource.Content = resource.Content.Replace(Keys.GROUPDOCSVIEWERUI_STYLESHEETS_TARGET,
                string.Join("\n", htmlStyles));

            return styleSheets;
        }
    }
}
