using GroupDocs.Viewer.UI.NetFramework.Core;
using GroupDocs.Viewer.UI.NetFramework.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Web;

namespace GroupDocs.Viewer.UI.NetFramework
{
    public class ViewerUI
    {
        private readonly AppResourcesReader _resourcesReader = new AppResourcesReader();
        private readonly ViewerUIConfig _viewerUIConfig;

        public ViewerUI(ViewerUIConfig viewerUIConfig)
        {
            _viewerUIConfig = viewerUIConfig;
        }

        public static ViewerUI Configure(ViewerUIConfig viewerConfig)
        {
            return new ViewerUI(viewerConfig);
        }

        public void HandleRequest(HttpContext context)
        {
            if (context.Request.RequestType != "GET")
                return;

            string requestPath = PathExtensions.TrimTrailingSlash(context.Request.Path);
            if (!requestPath.StartsWith(_viewerUIConfig.UIPath, StringComparison.OrdinalIgnoreCase) || requestPath.StartsWith(_viewerUIConfig.ApiEndpoint, StringComparison.OrdinalIgnoreCase))
                return;

            UIResource indexPage = UIResourceExtensions.GetIndexPage(_resourcesReader.UIResources);

            if (requestPath.Equals(_viewerUIConfig.UIPath, StringComparison.OrdinalIgnoreCase))
            {
                string pathBase = context.Request.ApplicationPath == "/" ? string.Empty : context.Request.ApplicationPath;

                Dictionary<string, string> routeValues = new Dictionary<string, string>();
                string[] keys = context.Request.QueryString.AllKeys;
                for (int i = 0; i < keys.Length; i++)
                {
                    string key = keys[i];
                    routeValues.Add(key, context.Request.QueryString[key]);
                }

                string content = UIResourceExtensions.GetIndexPageHtml(indexPage, _viewerUIConfig, pathBase, routeValues);

                // Write response
                context.Response.ContentType = indexPage.ContentType;
                context.Response.Write(content);

                // Stop further processing
                context.Response.End();
            }

            foreach (var resource in _resourcesReader.UIResources)
            {
                string resourcePath = $"{_viewerUIConfig.UIPath}/{resource.FileName}";

                if (requestPath.Equals(resourcePath, StringComparison.OrdinalIgnoreCase))
                {
                    // Write response
                    context.Response.ContentType = resource.ContentType;
                    context.Response.AddHeader("ContentLength", resource.Content.Length.ToString());
                    context.Response.BinaryWrite(resource.Content);

                    // Stop further processing
                    context.Response.End();
                }
            }

            ICollection<UIStylesheet> stylesheets = UIResourceExtensions.GetCustomStylesheets(indexPage, _viewerUIConfig);
            foreach (UIStylesheet stylesheet in stylesheets)
            {
                if (!requestPath.Equals(stylesheet.ResourcePath, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                // Write response
                context.Response.ContentType = "text/css";
                context.Response.AddHeader("ContentLength", stylesheet.Content.Length.ToString());
                context.Response.BinaryWrite(stylesheet.Content);

                // Stop further processing
                context.Response.End();
            }
        }
    }
}