using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Responses
{
    public class ViewerConfiguration
    {
        // Singleton instance
        internal static readonly ViewerConfiguration Instance = new ViewerConfiguration();

        [JsonPropertyName("pageSelector")]
        public bool PageSelector { get; set; } = true;

        [JsonPropertyName("download")]
        public bool Download { get; set; } = false;

        [JsonPropertyName("upload")]
        public bool Upload { get; set; } = false;

        [JsonPropertyName("print")]
        public bool Print { get; set; } = true;

        [JsonPropertyName("browse")]
        public bool Browse { get; set; } = true;

        [JsonPropertyName("rewrite")]
        public bool Rewrite { get; set; } = false;

        [JsonPropertyName("enableRightClick")]
        public bool EnableRightClick { get; set; } = true;

        [JsonPropertyName("filesDirectory")]
        public string FilesDirectory { get; set; } = string.Empty;

        [JsonPropertyName("fontsDirectory")]
        public string FontsDirectory { get; set; } = string.Empty;

        [JsonPropertyName("defaultDocument")]
        public string DefaultDocument { get; set; } = string.Empty;

        [JsonPropertyName("watermarkText")]
        public string WatermarkText { get; set; } = string.Empty;

        [JsonPropertyName("preloadPageCount")]
        public int PreloadPageCount { get; set; } = 10;

        [JsonPropertyName("zoom")]
        public bool Zoom { get; set; } = true;

        [JsonPropertyName("search")]
        public bool Search { get; set; } = false;

        [JsonPropertyName("thumbnails")]
        public bool Thumbnails { get; set; } = false;

        [JsonPropertyName("rotate")]
        public bool Rotate { get; set; } = false;

        [JsonPropertyName("htmlMode")]
        public bool HtmlMode { get; set; } = false;

        [JsonPropertyName("cache")]
        public bool Cache { get; set; } = true;

        [JsonPropertyName("saveRotateState")]
        public bool SaveRotateState { get; set; } = false;

        [JsonPropertyName("printAllowed")]
        public bool PrintAllowed { get; set; } = true;

        [JsonPropertyName("showGridLines")]
        public bool ShowGridLines { get; set; } = true;

        [JsonPropertyName("cacheFolderName")]
        public string CacheFolderName { get; set; } = string.Empty;
    }

}
