using System;
using System.Collections.Generic;

namespace GroupDocs.Viewer.UI.NetFramework.Core
{
    internal class ContentType
    {
        public const string JAVASCRIPT = "text/javascript";
        public const string CSS = "text/css";
        public const string HTML = "text/html";
        public const string PLAIN = "text/plain";
        public const string SVG = "image/svg+xml";
        public const string WOFF = "font/woff";
        public const string WOFF2 = "font/woff2";
        public const string ICON = "image/x-icon";
        public const string PNG = "image/png";
        public const string JSON = "application/json";
        
        public static Dictionary<string, string> SupportedContent =
            new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { "js", JAVASCRIPT },
            { "html", HTML },
            { "css", CSS },
            { "svg", SVG },
            { "woff", WOFF },
            { "woff2", WOFF2 },
            { "ico", ICON },
            { "png", PNG },
            { "json", JSON },
        };

        public static string FromExtension(string fileExtension)
            => SupportedContent.TryGetValue(fileExtension.ToLowerInvariant(), out string result) ? result : PLAIN;
    }
}
