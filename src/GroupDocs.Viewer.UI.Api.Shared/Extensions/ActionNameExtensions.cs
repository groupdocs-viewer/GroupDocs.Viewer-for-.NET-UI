using System;

namespace GroupDocs.Viewer.UI.Api.Extensions
{
    public static class ActionNameExtensions
    {
        public static string ToActionName(this string apiMethodPath) =>
            apiMethodPath.Replace("-", string.Empty);

        public static string RemoveApiPath(this string url, string apiPath)
        {
            return url.StartsWith(apiPath)
                ? url.Substring(apiPath.Length)
                : url;
        }
    }
}