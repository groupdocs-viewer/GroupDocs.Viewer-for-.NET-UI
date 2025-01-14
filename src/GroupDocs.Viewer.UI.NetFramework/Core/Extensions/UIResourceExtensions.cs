using System.Collections.Generic;
using System.Text;

namespace GroupDocs.Viewer.UI.NetFramework.Core.Extensions
{
    internal static class UIResourceExtensions
    {
        public static UIResource GetIndexPage(IEnumerable<UIResource> resources)
        {
            UIResource index = null;
            foreach (UIResource resource in resources)
            {
                if (resource.FileName == IndexPageTemplates.GROUPDOCSVIEWERUI_MAIN_UI_RESOURCE)
                {
                    index = resource;
                    break;
                }
            }

            return index;
        }

        public static string GetIndexPageHtml(UIResource index,
            ViewerUIConfig viewerUIConfig, string pathBase, Dictionary<string, string> routeValues)
        {
            string uiPath = PathExtensions.WithTrailingSlash(
                            PathExtensions.ReplacePatternsWithRouteValues(
                                PathExtensions.AppendPathBase(viewerUIConfig.UIPath, pathBase), routeValues));

            StringBuilder html = new StringBuilder(index.GetContentString());

            html.Replace(IndexPageTemplates.GROUPDOCSVIEWERUI_MAIN_UI_PATH, uiPath);
            html.Replace(IndexPageTemplates.GROUPDOCSVIEWERUI_MAIN_UI_TITLE, viewerUIConfig.UITitle);

            string apiEndpoint = PathExtensions.TrimTrailingSlash(
                                PathExtensions.ReplacePatternsWithRouteValues(
                                    PathExtensions.AppendPathBase(viewerUIConfig.ApiEndpoint, pathBase), routeValues));

            string uiConfig = SerializeWindowConfig(apiEndpoint, viewerUIConfig);

            html.Replace(IndexPageTemplates.GROUPDOCSVIEWERUI_MAIN_UI_CONFIG, uiConfig);

            return html.ToString();
        }

        private static string SerializeWindowConfig(string apiEndpoint, ViewerUIConfig config)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            sb.AppendFormat("\"apiEndpoint\":\"{0}\",", apiEndpoint);
            sb.AppendFormat("\"renderingMode\":\"{0}\",", config.ClientAppConfig.RenderingMode.Value);
            sb.AppendFormat("\"staticContentMode\":{0},", config.ClientAppConfig.StaticContentMode.ToString().ToLower());
            sb.AppendFormat("\"initialFile\":\"{0}\",", EscapeString(config.ClientAppConfig.InitialFile));
            sb.AppendFormat("\"preloadPages\":{0},", config.ClientAppConfig.PreloadPages);
            sb.AppendFormat("\"enableContextMenu\":{0},", config.ClientAppConfig.EnableContextMenu.ToString().ToLower());
            sb.AppendFormat("\"enableHyperlinks\":{0},", config.ClientAppConfig.EnableHyperlinks.ToString().ToLower());
            sb.AppendFormat("\"enableHeader\":{0},", config.ClientAppConfig.EnableHeader.ToString().ToLower());
            sb.AppendFormat("\"enableToolbar\":{0},", config.ClientAppConfig.EnableToolbar.ToString().ToLower());
            sb.AppendFormat("\"enableFileName\":{0},", config.ClientAppConfig.EnableFileName.ToString().ToLower());
            sb.AppendFormat("\"enableThumbnails\":{0},", config .ClientAppConfig.EnableThumbnails.ToString().ToLower());
            sb.AppendFormat("\"enableZoom\":{0},", config.ClientAppConfig.EnableZoom.ToString().ToLower());
            sb.AppendFormat("\"enablePageSelector\":{0},", config.ClientAppConfig.EnablePageSelector.ToString().ToLower());
            sb.AppendFormat("\"enableSearch\":{0},", config.ClientAppConfig.EnableSearch.ToString().ToLower());
            sb.AppendFormat("\"enablePrint\":{0},", config.ClientAppConfig.EnablePrint.ToString().ToLower());
            sb.AppendFormat("\"enableDownloadPdf\":{0},", config.ClientAppConfig.EnableDownloadPdf.ToString().ToLower());
            sb.AppendFormat("\"enablePresentation\":{0},", config.ClientAppConfig.EnablePresentation.ToString().ToLower());
            sb.AppendFormat("\"enableFileBrowser\":{0},", config.ClientAppConfig.EnableFileBrowser.ToString().ToLower());
            sb.AppendFormat("\"enableFileUpload\":{0},", config.ClientAppConfig.EnableFileUpload.ToString().ToLower());
            sb.AppendFormat("\"enableLanguageSelector\":{0},", config.ClientAppConfig.EnableLanguageSelector.ToString().ToLower());
            sb.AppendFormat("\"defaultLanguage\":\"{0}\",", config.ClientAppConfig.DefaultLanguage.Value);

            sb.Append("\"supportedLanguages\":[");
            for (int i = 0; i < config.ClientAppConfig.SupportedLanguages.Length; i++)
            {
                sb.AppendFormat("\"{0}\"", config.ClientAppConfig.SupportedLanguages[i].Value);
                if (i < config.ClientAppConfig.SupportedLanguages.Length - 1)
                    sb.Append(",");
            }
            sb.Append("]");

            sb.Append("}");
            return sb.ToString();
        }

        private static string EscapeString(string input)
        {
            if (input == null) return string.Empty;
            return input.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        public static ICollection<UIStylesheet> GetCustomStylesheets(UIResource resource, ViewerUIConfig config)
        {
            List<UIStylesheet> stylesheets = new List<UIStylesheet>();

            string content = resource.GetContentString();

            if (!(config.CustomStylesheets.Count > 0))
            {
                content = content.Replace(IndexPageTemplates.GROUPDOCSVIEWERUI_STYLESHEETS_TARGET, string.Empty);
                resource.SetContentString(content);
                return stylesheets;
            }

            foreach (string stylesheet in config.CustomStylesheets)
            {
                stylesheets.Add(UIStylesheet.Create(config, stylesheet));
            }

            string[] htmlStyles = new string[stylesheets.Count];
            for (int i = 0; i < stylesheets.Count; i++)
            {
                string linkHref = PathExtensions.AsRelativeResource(stylesheets[i].ResourceRelativePath);
                htmlStyles[i]= $"<link rel='stylesheet' href='{linkHref}'/>";
            }

            content = content.Replace(IndexPageTemplates.GROUPDOCSVIEWERUI_STYLESHEETS_TARGET,
                string.Join("\n", htmlStyles));

            resource.SetContentString(content);

            return stylesheets;
        }
    }
}
