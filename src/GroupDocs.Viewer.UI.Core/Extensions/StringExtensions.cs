namespace System
{
    public static class StringExtensions
    {
        public static string AsRelativeResource(this string resourcePath)
        {
            return resourcePath.StartsWith("/") ? resourcePath.Substring(1) : resourcePath;
        }

        public static string WithTrailingSlash(this string resourcePath)
        {
            return resourcePath.EndsWith("/") ? resourcePath : $"{resourcePath}/";
        }

        public static string TrimTrailingSlash(this string resourcePath)
        {
            return resourcePath.TrimEnd('/');
        }
    }
}
