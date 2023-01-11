using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace GroupDocs.Viewer.UI.Core.Extensions
{
    public static class PathExtensionsExtensions
    {
        public static string AppendPathBase(this string resourcePath, PathString pathBase) => 
            $"{pathBase}{resourcePath}";

        public static string ReplacePatternsWithRouteValues(this string resourcePath, Dictionary<string, string> routeValues)
        {
            foreach (KeyValuePair<string, string> keyValue in routeValues)
            {
                resourcePath = resourcePath.Replace($"{{{keyValue.Key}}}", keyValue.Value);
            }

            return resourcePath;
        }
    }
}
