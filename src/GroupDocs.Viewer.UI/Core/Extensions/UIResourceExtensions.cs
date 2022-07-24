using GroupDocs.Viewer.UI.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GroupDocs.Viewer.UI.Core
{
    internal static class UIResourceExtensions
    {
        public static UIResource GetMainUI(this IEnumerable<UIResource> resources, Options options)
        {
            var index = resources
                .FirstOrDefault(r => r.ContentType == ContentType.HTML && r.FileName == Keys.GROUPDOCSVIEWERUI_MAIN_UI_RESOURCE);

            var uiPath = options.UIPath.WithTrailingSlash();

            index.Content = index.Content
                .Replace(Keys.GROUPDOCSVIEWERUI_MAIN_UI_PATH, uiPath);

            var apiPath = options.APIEndpoint;

            index.Content = index.Content
                .Replace(Keys.GROUPDOCSVIEWERUI_MAIN_UI_API_TARGET, apiPath.TrimTrailingSlash());

            index.Content = index.Content
                .Replace(Keys.GROUPDOCSVIEWERUI_MAIN_UI_SETTINGS_PATH_TARGET, options.UIConfigEndpoint);

            return index;
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
