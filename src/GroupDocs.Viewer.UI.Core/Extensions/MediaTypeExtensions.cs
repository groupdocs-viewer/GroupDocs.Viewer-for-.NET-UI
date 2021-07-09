using System.IO;

namespace GroupDocs.Viewer.UI.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ContentTypeFromFileName(this string filename)
        {
            var extension = Path.GetExtension(filename);

            switch (extension)
            {
                case ".css": return "text/css";
                case ".woff": return "font/woff";
                case ".png": return "image/png";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                case ".svg": return "image/svg+xml";
                default:
                    return "application/octet-stream";
            }
        }
    }
}