using System;
using System.Collections.Generic;

namespace GroupDocs.Viewer.UI.Core
{
    internal class ContentType
    {
        public const string JAVASCRIPT = "text/javascript";
        public const string CSS = "text/css";
        public const string HTML = "text/html";
        public const string PLAIN = "text/plain";

        public static Dictionary<string, string> SupportedContent =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "js", JAVASCRIPT },
            { "html", HTML },
            { "css", CSS }
        };

        public static string FromExtension(string fileExtension)
            => SupportedContent.TryGetValue(fileExtension.ToLowerInvariant(), out var result) ? result : PLAIN;
    }
}
