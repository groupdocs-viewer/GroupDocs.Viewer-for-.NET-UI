namespace GroupDocs.Viewer.UI.Api.Caching
{
    public static class CacheKeys
    {
        public const string FILE_INFO_CACHE_KEY = "info.json";
        public const string PDF_FILE_CACHE_KEY = "file.pdf";

        public static string GetHtmlPageCacheKey(int pageNumber) =>
            $"p{pageNumber}.html";

        public static string GetPngPageCacheKey(int pageNumber) => 
            $"p{pageNumber}.png";

        public static string GetJpgPageCacheKey(int pageNumber) => 
            $"p{pageNumber}.jpg";

        public static string GetHtmlPageResourceCacheKey(int pageNumber, string resourceName) 
            => $"p{pageNumber}_{resourceName}";
    }
}