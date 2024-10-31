using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Middleware
{
    public class EmbeddedResourceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Assembly _assembly;
        private readonly string _namespace = "GroupDocs.Viewer.UI.assets";
        private readonly string _urlPrefix;
        private readonly IViewer _viewer;

        public EmbeddedResourceMiddleware(RequestDelegate next, IViewer viewer, string urlPrefix = "viewer")
        {
            _next = next;
            _viewer = viewer;
            _urlPrefix = urlPrefix;
            _assembly = typeof(EmbeddedResourceMiddleware).Assembly;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path.Value?.Trim('/') ?? string.Empty;
            if (string.IsNullOrWhiteSpace(path) || !path.StartsWith(_urlPrefix) && !path.StartsWith("assets/config/config.json"))
            {
                await _next(context);
                return;
            }
            else if (path.StartsWith($"{_urlPrefix}/storage"))
            {
                var pathSegments = path.Split('/');
                var pageNumber = int.TryParse(Path.GetFileNameWithoutExtension(pathSegments.Last()), out var result);
                var data = await _viewer.GetPageAsync(new FileCredentials(Path.GetFileName(pathSegments[^2]), null), pageNumber ? result : 1);
                await context.Response.WriteAsync(data.GetContent());
                return;
            }
            else if (path.Equals("assets/config/config.json", System.StringComparison.InvariantCultureIgnoreCase))
            {
                path = $"{_namespace}.{path.Replace("/", ".")}";
            }
            else if (path.Equals("viewer", System.StringComparison.InvariantCultureIgnoreCase) || path.EndsWith("index.html", StringComparison.CurrentCultureIgnoreCase))
            {
                path = $"{_namespace}.index.html";
                await using Stream indexStream = _assembly.GetManifestResourceStream(path);
                if (indexStream != null)
                {
                    using (StreamReader reader = new StreamReader(indexStream))
                    {
                        var htmlContent = await reader.ReadToEndAsync();

                        // Modify the <title> and <base> tags
                        htmlContent = SetTitleAndBaseHref(htmlContent, "GroupDocs.Viewer UI application", _urlPrefix);

                        // Return the modified HTML content
                        context.Response.ContentType = MimeMapping.GetContentType(path);
                        await context.Response.WriteAsync(htmlContent);
                        return;
                    }
                }

                throw new InvalidDataException("missing index.html for Angular App");
            }
            else
            {
                path = path.Replace("/", ".").Replace("viewer", _namespace);
            }

            await using Stream stream = _assembly.GetManifestResourceStream(path);
            if (stream != null)
            {
                context.Response.ContentType = MimeMapping.GetContentType(path);
                await stream.CopyToAsync(context.Response.Body);
                return;
            }

            await _next(context); // If not found, proceed to next middleware
        }

        private string SetTitleAndBaseHref(string htmlContent, string title, string baseHref)
        {
            // Modify the <title> tag
            htmlContent = System.Text.RegularExpressions.Regex.Replace(htmlContent, "<title>(.*?)</title>", $"<title>{title}</title>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            // Modify the <base> tag
            htmlContent = System.Text.RegularExpressions.Regex.Replace(htmlContent, "<base href=\"(.*?)\">", $"<base href=\"/{baseHref}/\">", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return htmlContent;
        }
    }

    public class MimeMapping
    {
        private static readonly IDictionary<string, string> MimeTypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { ".txt", "text/plain" },
            { ".html", "text/html" },
            { ".htm", "text/html" },
            { ".css", "text/css" },
            { ".js", "application/javascript" },
            { ".json", "application/json" },
            { ".xml", "application/xml" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".gif", "image/gif" },
            { ".svg", "image/svg+xml" },
            { ".pdf", "application/pdf" },
            { ".zip", "application/zip" },
            { ".mp4", "video/mp4" },
            { ".woff", "font/woff" },
            { ".woff2", "font/woff2" },
            { ".ttf", "font/ttf" },
            { ".otf", "font/otf" }
        };

        public static string GetContentType(string path)
        {
            // Get the file extension
            var extension = System.IO.Path.GetExtension(path);

            // Check if the extension exists in the dictionary
            if (extension != null && MimeTypes.TryGetValue(extension, out string contentType))
            {
                return contentType;
            }

            // Return a default content type if not found
            return "application/octet-stream";
        }
    }
}
