using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Web;
using System;

namespace GroupDocs.Viewer.UI.Api.Utils
{
    public class ApiUrlBuilder : IApiUrlBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Configuration.Options _options;

        public ApiUrlBuilder(
            IHttpContextAccessor httpContextAccessor, 
            Configuration.IOptionsProvider optionsProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _options = optionsProvider.GetOptions();
        }

        public string GetApiDomainOrDefault()
        {
            var request = _httpContextAccessor.HttpContext.Request;

            if (string.IsNullOrEmpty(_options.ApiDomain))
            {
                var baseUrl = $"{request.Scheme}://{request.Host}";

                if (!string.IsNullOrEmpty(request.PathBase))
                {
                    baseUrl += request.PathBase.Value;
                }

                return baseUrl;
            }

            return _options.ApiDomain;
        }

        public string BuildPageUrl(string file, int page, string extension) =>
            BuildUrl(
                useAbsoluteUrls: _options.UseAbsoluteUrls,
                apiDomain: _options.UseAbsoluteUrls ? GetApiDomainOrDefault() : null,
                apiPath: _options.ApiPath,
                apiMethodName: ApiNames.API_METHOD_GET_PAGE,
                values: new { file = file, page = page });

        public string BuildThumbUrl(string file, int page, string extension) =>
            BuildUrl(
                useAbsoluteUrls: _options.UseAbsoluteUrls,
                apiDomain: _options.UseAbsoluteUrls ? GetApiDomainOrDefault() : null,
                apiPath: _options.ApiPath,
                apiMethodName: ApiNames.API_METHOD_GET_THUMB,
                values: new { file = file, page = page });

        public string BuildPdfUrl(string file) =>
            BuildUrl(
                useAbsoluteUrls: _options.UseAbsoluteUrls,
                apiDomain: _options.UseAbsoluteUrls ? GetApiDomainOrDefault() : null,
                apiPath: _options.ApiPath,
                apiMethodName: ApiNames.API_METHOD_GET_PDF,
                values: new { file = file });

        public string BuildResourceUrl(string file, int page, string resource) =>
            BuildUrl(
                useAbsoluteUrls: _options.UseAbsoluteUrls,
                apiDomain: _options.UseAbsoluteUrls ? GetApiDomainOrDefault() : null,
                apiPath: _options.ApiPath,
                apiMethodName: ApiNames.API_METHOD_GET_RESOURCE,
                values: new { file = file, page = page, resource = resource });

        public string BuildResourceUrl(string file, string pageTemplate, string resourceTemplate) =>
            BuildUrl(
                useAbsoluteUrls: _options.UseAbsoluteUrls,
                apiDomain: _options.UseAbsoluteUrls ? GetApiDomainOrDefault() : null,
                apiPath: _options.ApiPath,
                apiMethodName: ApiNames.API_METHOD_GET_RESOURCE,
                values: new { file = file, page = pageTemplate, resource = resourceTemplate });

        /// <summary>
        /// Builds a relative URL using the API method name and query parameters.
        /// </summary>
        /// <param name="apiMethodName">The API method name, e.g., "viewer-api/get-page".</param>
        /// <param name="values">An object containing query parameter key-value pairs, e.g., new { file = "my-file.docx", page = 5 }.</param>
        /// <returns>The relative URL as a string, e.g., "/viewer-api/get-page?file=my-file.docx&page=5".</returns>
        /// <example>
        /// string url = UrlHelper.BuildUrl("viewer-api/get-page", new { file = "my-file.docx", page = 5 });
        /// Console.WriteLine(url); // Output: /viewer-api/get-page?file=my-file.docx&page=5
        /// </example>
        private static string BuildUrl(string apiMethodName, object values)
        {
            if (string.IsNullOrWhiteSpace(apiMethodName))
                throw new ArgumentNullException(nameof(apiMethodName), "API method name cannot be null or empty.");

            var baseUrl = $"/{apiMethodName.TrimStart('/')}";
            var queryString = BuildQueryString(values);

            return string.IsNullOrWhiteSpace(queryString) ? baseUrl : $"{baseUrl}?{queryString}";
        }

        /// <summary>
        /// Builds a URL using the API path and method name. Returns a relative URL when useAbsoluteUrls is false,
        /// or an absolute URL when useAbsoluteUrls is true. When useAbsoluteUrls is false, apiPath is ignored.
        /// </summary>
        /// <param name="useAbsoluteUrls">If true, generates an absolute URL using apiDomain and apiPath. If false, generates a relative URL and ignores apiPath.</param>
        /// <param name="apiDomain">The base API domain. Required when useAbsoluteUrls is true. Ignored when useAbsoluteUrls is false.</param>
        /// <param name="apiPath">The API path, e.g., "viewer-api". Required only when useAbsoluteUrls is true. Ignored when useAbsoluteUrls is false.</param>
        /// <param name="apiMethodName">The API method name, e.g., "get-page".</param>
        /// <param name="values">An object containing query parameter key-value pairs, e.g., new { file = "my-file.docx", page = 5 }.</param>
        /// <returns>
        /// A relative URL when useAbsoluteUrls is false (apiPath is ignored), e.g., "/get-page?file=my-file.docx&page=5".
        /// An absolute URL when useAbsoluteUrls is true, e.g., "https://www.example.com/viewer-api/get-page?file=my-file.docx&page=5".
        /// </returns>
        /// <example>
        /// // Relative URL (useAbsoluteUrls is false, apiPath is ignored)
        /// string url1 = UrlHelper.BuildUrl(false, null, "viewer-api", "get-page", new { file = "my-file.docx", page = 5 });
        /// Console.WriteLine(url1); // Output: /get-page?file=my-file.docx&page=5
        /// 
        /// // Absolute URL (useAbsoluteUrls is true, apiDomain and apiPath are used)
        /// string url2 = UrlHelper.BuildUrl(true, "https://www.example.com", "viewer-api", "get-page", new { file = "my-file.docx", page = 5 });
        /// Console.WriteLine(url2); // Output: https://www.example.com/viewer-api/get-page?file=my-file.docx&page=5
        /// </example>
        private static string BuildUrl(bool useAbsoluteUrls, string apiDomain, string apiPath, string apiMethodName, object values)
        {
            if (string.IsNullOrWhiteSpace(apiMethodName))
                throw new ArgumentNullException(nameof(apiMethodName), "API method name cannot be null or empty.");

            var queryString = BuildQueryString(values);

            // Build relative URL if useAbsoluteUrls is false (ignore apiPath)
            if (!useAbsoluteUrls)
            {
                string basePath = $"/{apiMethodName.TrimStart('/')}";
                return string.IsNullOrWhiteSpace(queryString) ? basePath : $"{basePath}?{queryString}";
            }

            // Build absolute URL (apiDomain and apiPath are required when useAbsoluteUrls is true)
            if (string.IsNullOrWhiteSpace(apiDomain))
                throw new ArgumentNullException(nameof(apiDomain), "API domain cannot be null or empty when UseAbsoluteUrls is true.");

            if (string.IsNullOrWhiteSpace(apiPath))
                throw new ArgumentNullException(nameof(apiPath), "API path cannot be null or empty when UseAbsoluteUrls is true.");

            string basePathWithApiPath = $"/{apiPath.TrimStart('/').TrimEnd('/')}/{apiMethodName.TrimStart('/')}";
            var pathWithQuery = string.IsNullOrWhiteSpace(queryString) ? basePathWithApiPath : $"{basePathWithApiPath}?{queryString}";
            var domain = apiDomain.TrimEnd('/');
            return $"{domain}{pathWithQuery}";
        }

        private static string BuildQueryString(object values)
        {
            if (values == null)
                return string.Empty;

            var queryParameters = new List<string>();
            foreach (var property in values.GetType().GetProperties())
            {
                var key = property.Name;
                var value = property.GetValue(values, null);

                if (value != null)
                {
                    queryParameters.Add($"{HttpUtility.UrlEncode(key)}={HttpUtility.UrlEncode(value.ToString())}");
                }
            }

            return string.Join("&", queryParameters);
        }
    }
}