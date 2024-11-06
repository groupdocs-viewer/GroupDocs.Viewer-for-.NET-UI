using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Options = GroupDocs.Viewer.UI.Api.Configuration.Options;

namespace GroupDocs.Viewer.UI.Middleware
{
    public class EmbeddedResourceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Assembly _assembly;
        private readonly string _namespace = "GroupDocs.Viewer.UI.assets";
        private readonly string _urlPrefix;
        private readonly IViewer _viewer;
        private readonly Options _options;

        public EmbeddedResourceMiddleware(
            RequestDelegate next,
            IViewer viewer,
            IOptions<Options> options,
            string urlPrefix = "viewer")
        {
            _next = next;
            _viewer = viewer;
            _urlPrefix = urlPrefix;
            _assembly = typeof(EmbeddedResourceMiddleware).Assembly;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string lang = context.Request.Query["lang"].Count > 0 ? $"/{context.Request.Query["lang"]}" : string.Empty;
            string file = context.Request.Query["file"].Count > 0 ? $"/{context.Request.Query["file"]}" : string.Empty;
            string path = $"{context.Request.Path.Value?.Trim('/')}{lang}{file}";

            if (string.IsNullOrWhiteSpace(path) || !path.Equals(_urlPrefix) && !path.StartsWith($"{_urlPrefix}/") && !path.StartsWith("assets/config/config.json"))
            {
                await _next(context);
                return;
            }

            if (path.Equals("assets/config/config.json", StringComparison.InvariantCultureIgnoreCase))
            {
                await ServeConfigJson(context);
                return;
            }

            if (path.Equals(_urlPrefix, StringComparison.InvariantCultureIgnoreCase) || path.EndsWith("index.html", StringComparison.CurrentCultureIgnoreCase))
            {
                await ServeIndexHtml(context);
                return;
            }
            if (path.StartsWith($"{_urlPrefix}/storage/pdf/"))
            {
                await ServePdf(context, path);
                return;
            }
            if (path.StartsWith($"{_urlPrefix}/storage/page/"))
            {
                await ServePageContent(context, path);
                return;
            }
            if (path.StartsWith($"{_urlPrefix}/storage/thumbnail/"))
            {
                // Placeholder for thumbnail handling
                // await ServeThumbnail(context, path);
                return;
            }

            await ServeEmbeddedFile(context, path);
        }

        private async Task ServeConfigJson(HttpContext context)
        {
            string configJson = ConfigHelper.GenerateConfigJson(context, _options);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(configJson);
        }

        private async Task ServeIndexHtml(HttpContext context)
        {
            string path = $"{_namespace}.index.html";
            await using Stream indexStream = _assembly.GetManifestResourceStream(path);

            if (indexStream == null)
            {
                throw new InvalidDataException("Missing index.html for Angular App");
            }

            using StreamReader reader = new(indexStream);
            string htmlContent = await reader.ReadToEndAsync();

            // Modify the <title> and <base> tags
            htmlContent = SetTitleAndBaseHref(htmlContent, "GroupDocs.Viewer UI Application", _urlPrefix);

            context.Response.ContentType = MimeMapping.GetContentType(path);
            await context.Response.WriteAsync(htmlContent);
        }

        private async Task ServePdf(HttpContext context, string path)
        {
            string[] pathSegments = path.Split('/');
            byte[] data = await _viewer.GetPdfAsync(new FileCredentials(Path.GetFileName(pathSegments[^2]), null));
            context.Response.ContentType = "application/pdf";
            context.Response.ContentLength = data.Length;
            await context.Response.Body.WriteAsync(data, 0, data.Length);
        }

        private async Task ServePageContent(HttpContext context, string path)
        {
            string[] pathSegments = path.Split('/');
            bool pageNumber = int.TryParse(pathSegments[^1], out int result);
            Page data = await _viewer.GetPageAsync(new FileCredentials(Path.GetFileName(pathSegments[^2]), null), pageNumber ? result : 1);
            await context.Response.WriteAsync(data.GetContent());
        }

        private async Task ServeEmbeddedFile(HttpContext context, string path)
        {
            string resourcePath = path.Replace("/", ".").Replace("viewer", _namespace);

            // Try to get the requested resource
            await using Stream stream = _assembly.GetManifestResourceStream(resourcePath);
            if (stream != null)
            {
                context.Response.ContentType = MimeMapping.GetContentType(resourcePath);
                await stream.CopyToAsync(context.Response.Body);
                return;
            }

            // Handle requests to index.html for Angular App
            if (path.StartsWith($"{_urlPrefix}/", StringComparison.InvariantCultureIgnoreCase))
            {
                await ServeIndexHtml(context);
                return;
            }

            // If no file was found, return 404
            context.Response.StatusCode = StatusCodes.Status404NotFound;
        }


        private static string SetTitleAndBaseHref(string htmlContent, string title, string baseHref)
        {
            // Modify the <title> tag
            htmlContent = Regex.Replace(htmlContent, "<title>(.*?)</title>", $"<title>{title}</title>", RegexOptions.IgnoreCase);

            // Modify the <base> tag
            htmlContent = Regex.Replace(htmlContent, "<base href=\"(.*?)\">", $"<base href=\"/{baseHref}/\">", RegexOptions.IgnoreCase);

            return htmlContent;
        }
    }

    public static class ConfigHelper
    {
        public static string GenerateConfigJson(HttpContext context, Options options)
        {
            HttpRequest request = context.Request;
            string currentUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

            var config = new
            {
                apiEndpoint = $"{currentUrl}{options.ApiPath}",
                configEndpoint = string.Empty,
                closeViewerUrl = "/"
            };

            return JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
        }
    }

    public class MimeMapping
    {
        private static readonly IDictionary<string, string> MimeTypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { ".txt", "text/plain" },
            { ".html", "text/html" },
            { ".css", "text/css" },
            { ".js", "application/javascript" },
            { ".json", "application/json" },
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".svg", "image/svg+xml" },
            { ".pdf", "application/pdf" }
        };

        public static string GetContentType(string path)
        {
            string extension = Path.GetExtension(path);
            return MimeTypes.TryGetValue(extension, out string contentType) ? contentType : "application/octet-stream";
        }
    }
}
