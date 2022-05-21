using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class LoadConfigResponse
    {
        [JsonPropertyName("pageSelector")]
        public bool PageSelector { get; set; }

        [JsonPropertyName("download")]
        public bool Download { get; set; }

        [JsonPropertyName("upload")]
        public bool Upload { get; set; }

        [JsonPropertyName("print")]
        public bool Print { get; set; }

        [JsonPropertyName("browse")]
        public bool Browse { get; set; }

        [JsonPropertyName("rewrite")]
        public bool Rewrite { get; set; }

        [JsonPropertyName("enableRightClick")]
        public bool EnableRightClick { get; set; }

        [JsonPropertyName("DefaultDocument")]
        public string DefaultDocument { get; set; }

        [JsonPropertyName("preloadPageCount")]
        public int PreloadPageCount { get; set; }

        [JsonPropertyName("zoom")]
        public bool Zoom { get; set; }

        [JsonPropertyName("search")]
        public bool Search { get; set; }

        [JsonPropertyName("thumbnails")]
        public bool Thumbnails { get; set; }

        [JsonPropertyName("htmlMode")]
        public bool HtmlMode { get; set; }

        [JsonPropertyName("printAllowed")]
        public bool PrintAllowed { get; set; }

        [JsonPropertyName("rotate")]
        public bool Rotate { get; set; }

        [JsonPropertyName("saveRotateState")]
        public bool SaveRotateState { get; set; }

        [JsonPropertyName("defaultLanguage")]
        public string DefaultLanguage { get; set; }

        [JsonPropertyName("supportedLanguages")]
        public string[] SupportedLanguages { get; set; }

        [JsonPropertyName("showLanguageMenu")]
        public bool ShowLanguageMenu { get; set; }
    }
}