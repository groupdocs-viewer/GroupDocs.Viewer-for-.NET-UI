using System.Collections.Generic;
using System.IO;

namespace GroupDocs.Viewer.UI.NetFramework.Core.Extensions
{
    internal static class PathExtensions
    {
        public static bool IsPathFullyQualified(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            // Check if the path starts with a drive letter (e.g., "C:\")
            if (Path.IsPathRooted(path) && path.Length > 1 && path[1] == ':')
            {
                return true;
            }

            // Check if the path is a UNC path (e.g., "\\Server\Share")
            if (path.StartsWith(@"\\"))
            {
                return true;
            }

            return false;
        }

        public static string AppendPathBase(string resourcePath, string pathBase) =>
            $"{pathBase}{resourcePath}";

        public static string ReplacePatternsWithRouteValues(string resourcePath, Dictionary<string, string> routeValues)
        {
            foreach (KeyValuePair<string, string> keyValue in routeValues)
            {
                resourcePath = resourcePath.Replace($"{{{keyValue.Key}}}", keyValue.Value);
            }

            return resourcePath;
        }

        public static string AsRelativeResource(string resourcePath)
        {
            return resourcePath.StartsWith("/") ? resourcePath.Substring(1) : resourcePath;
        }

        public static string WithTrailingSlash(string resourcePath)
        {
            return resourcePath.EndsWith("/") ? resourcePath : $"{resourcePath}/";
        }

        public static string TrimTrailingSlash(string resourcePath)
        {
            return resourcePath.TrimEnd('/');
        }
    }

}
