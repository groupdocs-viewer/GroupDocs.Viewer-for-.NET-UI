using GroupDocs.Viewer.UI.Api.Utils;

namespace GroupDocs.Viewer.UI.Sample.StaticContentMode.Generator
{
    internal class StaticUrlBuilder : IApiUrlBuilder
    {
        private readonly string _apiEndpoint;

        public StaticUrlBuilder(string apiEndpoint)
        {
            _apiEndpoint = apiEndpoint.EndsWith("/") 
                ? apiEndpoint.TrimEnd('/') 
                : apiEndpoint;
        }

        public string BuildPageUrl(string filePath, int pageNumber, string extension)
        {
            var pageFileName = string.Format(Constants.PAGE_FILE_NAME_TEMPLATE, pageNumber, extension);

            return $"{_apiEndpoint}/{filePath}/{pageFileName}";
        }

        public string BuildThumbUrl(string filePath, int pageNumber, string extension)
        {
            var thumbFileName = string.Format(Constants.THUMB_FILE_NAME_TEMPLATE, pageNumber, extension);

            return $"{_apiEndpoint}/{filePath}/{thumbFileName}";
        }

        public string BuildPdfUrl(string filePath)
        {
            return $"{_apiEndpoint}/{filePath}/{Constants.PDF_FILE_NAME}";
        }

        public string BuildResourceUrl(string filePath, int pageNumber, string resource)
        {
            return $"{_apiEndpoint}/{filePath}/{pageNumber}/{resource}";
        }

        public string BuildResourceUrl(string filePath, string pageTemplate, string resourceTemplate)
        {
            return $"{_apiEndpoint}/{filePath}/{pageTemplate}/{resourceTemplate}";
        }
    }
}