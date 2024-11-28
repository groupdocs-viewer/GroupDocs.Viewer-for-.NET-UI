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
    }
}
