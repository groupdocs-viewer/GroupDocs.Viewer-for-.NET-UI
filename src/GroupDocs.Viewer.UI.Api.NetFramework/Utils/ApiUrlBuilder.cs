using GroupDocs.Viewer.UI.Api.Utils;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;

namespace GroupDocs.Viewer.UI.Api.NetFramework.Utils
{
    public class ApiUrlBuilder : IApiUrlBuilder
    {
        private readonly HttpContextBase _httpContext;
        private readonly Configuration.Options _options;

        public ApiUrlBuilder(HttpContextBase httpContext, IOptions<Configuration.Options> options)
        {
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
            _options = options.Value;
        }

        public string GetApiDomainOrDefault()
        {
            var request = _httpContext.Request;

            return string.IsNullOrEmpty(_options.ApiDomain)
                ? $"{request.Url?.Scheme}://{request.Url?.Authority}"
                : _options.ApiDomain;
        }

        public string BuildPageUrl(string file, int page, string extension) =>
            BuildUrl(
                apiDomain: GetApiDomainOrDefault(),
                apiPath: _options.ApiPath,
                apiMethodName: ApiNames.API_METHOD_GET_PAGE,
                values: new { file, page });

        public string BuildThumbUrl(string file, int page, string extension) =>
            BuildUrl(
                apiDomain: GetApiDomainOrDefault(),
                apiPath: _options.ApiPath,
                apiMethodName: ApiNames.API_METHOD_GET_THUMB,
                values: new { file, page });

        public string BuildPdfUrl(string file) =>
            BuildUrl(
                apiDomain: GetApiDomainOrDefault(),
                apiPath: _options.ApiPath,
                apiMethodName: ApiNames.API_METHOD_GET_PDF,
                values: new { file });

        public string BuildResourceUrl(string file, int page, string resource) =>
            BuildUrl(
                apiDomain: GetApiDomainOrDefault(),
                apiPath: _options.ApiPath,
                apiMethodName: ApiNames.API_METHOD_GET_RESOURCE,
                values: new { file, page, resource });

        public string BuildResourceUrl(string file, string pageTemplate, string resourceTemplate) =>
            BuildUrl(
                apiDomain: GetApiDomainOrDefault(),
                apiPath: _options.ApiPath,
                apiMethodName: ApiNames.API_METHOD_GET_RESOURCE,
                values: new { file, page = pageTemplate, resource = resourceTemplate });

        /// <summary>
        /// Builds a full URL using the API domain, path, method name, and query parameters.
        /// </summary>
        /// <param name="apiDomain">The base API domain, e.g., "https://www.example.com".</param>
        /// <param name="apiPath">The API path, e.g., "viewer-api".</param>
        /// <param name="apiMethodName">The API method name, e.g., "get-page".</param>
        /// <param name="values">An object containing query parameter key-value pairs, e.g., new { file = "my-file.docx", page = 5 }.</param>
        /// <returns>The full URL as a string, e.g., "https://www.example.com/viewer-api/get-page?file=my-file.docx&page=5".</returns>
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
            foreach (var property in values.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
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
