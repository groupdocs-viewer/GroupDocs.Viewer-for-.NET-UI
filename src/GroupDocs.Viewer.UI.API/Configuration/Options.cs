namespace GroupDocs.Viewer.UI.Api.Configuration
{
    /// <summary>
    /// Configuration options for API connection settings.
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Specifies the base domain for the API, including the protocol and host.
        /// If not explicitly set, the default value will be inferred from the current HTTP context.
        /// Example: "https://localhost:5001" or "https://api.example.com".
        /// </summary>
        public string ApiDomain { get; set; }

        /// <summary>
        /// Specifies the path for the API endpoint relative to the domain.
        /// Default: "/viewer-api".
        /// Example: "/api/v1" or "/custom-path".
        /// </summary>
        public string ApiPath { get; set; } = "/viewer-api";

        /// <summary>
        /// Controls whether the ApiUrlBuilder generates absolute URLs or relative URLs for resources.
        /// 
        /// <para>
        /// When set to <c>false</c> (default):
        /// - Relative URLs are generated (e.g., "/get-page?file=document.docx&amp;page=1")
        /// - The <see cref="ApiPath"/> property is ignored
        /// - URLs respect the browser's current domain, which is useful for scenarios like Azure App Service with custom domains
        /// - No domain validation is performed
        /// </para>
        /// 
        /// <para>
        /// When set to <c>true</c>:
        /// - Absolute URLs are generated (e.g., "https://example.com/viewer-api/get-page?file=document.docx&amp;page=1")
        /// - Both <see cref="ApiDomain"/> and <see cref="ApiPath"/> are required and used
        /// - Useful for cross-domain scenarios, CDN integration, or when the API is hosted on a different domain
        /// - If <see cref="ApiDomain"/> is not explicitly set, it will be inferred from the current HTTP context
        /// </para>
        /// 
        /// <para>
        /// Examples:
        /// <list type="bullet">
        /// <item><description>Relative (default): <c>UseAbsoluteUrls = false</c> → "/get-page?file=doc.docx&amp;page=1"</description></item>
        /// <item><description>Absolute: <c>UseAbsoluteUrls = true</c>, <c>ApiDomain = "https://api.example.com"</c>, <c>ApiPath = "/viewer-api"</c> → "https://api.example.com/viewer-api/get-page?file=doc.docx&amp;page=1"</description></item>
        /// </list>
        /// </para>
        /// 
        /// <para>
        /// Default: <c>false</c> (relative URLs).
        /// </para>
        /// </summary>
        public bool UseAbsoluteUrls { get; set; } = false;
    }
}
