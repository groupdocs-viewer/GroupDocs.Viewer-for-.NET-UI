using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;

namespace GroupDocs.Viewer.UI.Core.Extensions
{
    public static class RoteValuesDictionaryExtensions
    {
        public static Dictionary<string, string> ToDictionary(this RouteValueDictionary routeValueDictionary)
        {
            Dictionary<string, string> routeValues = new Dictionary<string, string>();
            foreach (var keyValue in routeValueDictionary)
                routeValues.Add(keyValue.Key, keyValue.Value?.ToString());

            return routeValues;
        }
    }
}