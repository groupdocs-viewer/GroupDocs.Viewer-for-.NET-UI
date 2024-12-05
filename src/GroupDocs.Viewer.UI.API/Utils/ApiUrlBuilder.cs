using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Web;
using System;

namespace GroupDocs.Viewer.UI.Api.Utils
{
    public class ApiUrlBuilder : IApiUrlBuilder
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOptions<Configuration.Options> _options;

        public ApiUrlBuilder(
            IHttpContextAccessor httpContextAccessor, 
            IOptions<Configuration.Options> options)
        {
            _httpContextAccessor = httpContextAccessor;
            _options = options;
        }

        public string GetApiDomainOrDefault()
        {
            var request = _httpContextAccessor.HttpContext.Request;

            return string.IsNullOrEmpty(_options.Value.ApiDomain)
                ? $"{request.Scheme}://{request.Host}"
                : _options.Value.ApiDomain;
        }

        public string BuildPageUrl(string file, int page) =>
            BuildUrl(
                apiDomain: GetApiDomainOrDefault(),
                apiPath: _options.Value.ApiPath,
                apiMethodName: ApiNames.API_METHOD_GET_PAGE,
                values: new { file = file, page = page });

        public string BuildThumbUrl(string file, int page) =>
            BuildUrl(
                apiDomain: GetApiDomainOrDefault(),
                apiPath: _options.Value.ApiPath,
                apiMethodName: ApiNames.API_METHOD_GET_THUMB,
                values: new { file = file, page = page });

        public string BuildPdfUrl(string file) =>
            BuildUrl(
                apiDomain: GetApiDomainOrDefault(),
                apiPath: _options.Value.ApiPath,
                apiMethodName: ApiNames.API_METHOD_GET_PDF,
                values: new { file = file });

        public string BuildResourceUrl(string file, int page, string resource) =>
            BuildUrl(
                apiDomain: GetApiDomainOrDefault(),
                apiPath: _options.Value.ApiPath,
                apiMethodName: ApiNames.API_METHOD_GET_RESOURCE,
                values: new { file = file, page = page, resource = resource });

        public string BuildResourceUrl(string file, string pageTemplate, string resourceTemplate) =>
            BuildUrl(
                apiDomain: GetApiDomainOrDefault(),
                apiPath: _options.Value.ApiPath,
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
        /// Builds a full URL using the API domain, path, method name, and query parameters.
        /// </summary>
        /// <param name="apiDomain">The base API domain, e.g., "https://www.example.com".</param>
        /// <param name="apiPath">The API path, e.g., "viewer-api".</param>
        /// <param name="apiMethodName">The API method name, e.g., "get-page".</param>
        /// <param name="values">An object containing query parameter key-value pairs, e.g., new { file = "my-file.docx", page = 5 }.</param>
        /// <returns>The full URL as a string, e.g., "https://www.example.com/viewer-api/get-page?file=my-file.docx&page=5".</returns>
        /// <example>
        /// string url = UrlHelper.BuildUrl("https://www.example.com", "viewer-api", "get-page", new { file = "my-file.docx", page = 5 });
        /// Console.WriteLine(url); // Output: https://www.example.com/viewer-api/get-page?file=my-file.docx&page=5
        /// </example>
        private static string BuildUrl(string apiDomain, string apiPath, string apiMethodName, object values)
        {
            if (string.IsNullOrWhiteSpace(apiDomain))
                throw new ArgumentNullException(nameof(apiDomain), "API domain cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(apiPath))
                throw new ArgumentNullException(nameof(apiPath), "API path cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(apiMethodName))
                throw new ArgumentNullException(nameof(apiMethodName), "API method name cannot be null or empty.");

            // Ensure proper URL formatting
            string basePath = $"{apiDomain.TrimEnd('/')}/{apiPath.TrimStart('/').TrimEnd('/')}/{apiMethodName.TrimStart('/')}";
            var queryString = BuildQueryString(values);

            return string.IsNullOrWhiteSpace(queryString) ? basePath : $"{basePath}?{queryString}";
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