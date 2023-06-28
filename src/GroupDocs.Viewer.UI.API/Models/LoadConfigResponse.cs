using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class LoadConfigResponse
    {
        /// <summary>
        /// Enables page selector control.
        /// </summary>
        [JsonPropertyName("pageSelector")]
        public bool PageSelector { get; set; }

        /// <summary>
        /// Enables download button.
        /// </summary>
        [JsonPropertyName("download")]
        public bool Download { get; set; }

        /// <summary>
        /// Enables upload.
        /// </summary>
        [JsonPropertyName("upload")]
        public bool Upload { get; set; }
        
        /// <summary>
        /// Enables printing.
        /// </summary>
        [JsonPropertyName("print")]
        public bool Print { get; set; }

        /// <summary>
        /// Enables file browser.
        /// </summary>
        [JsonPropertyName("browse")]
        public bool Browse { get; set; }

        /// <summary>
        /// Enables file rewrite.
        /// </summary>
        [JsonPropertyName("rewrite")]
        public bool Rewrite { get; set; }

        /// <summary>
        /// Enables right click.
        /// </summary>
        [JsonPropertyName("enableRightClick")]
        public bool EnableRightClick { get; set; }

        /// <summary>
        /// The default document to view.
        /// </summary>
        [JsonPropertyName("defaultDocument")]
        public string DefaultDocument { get; set; }

        /// <summary>
        /// Count pages to preload.
        /// </summary>
        [JsonPropertyName("preloadPageCount")]
        public int PreloadPageCount { get; set; }

        /// <summary>
        /// Enables zoom.
        /// </summary>
        [JsonPropertyName("zoom")]
        public bool Zoom { get; set; }

        /// <summary>
        /// Enables searching.
        /// </summary>
        [JsonPropertyName("search")]
        public bool Search { get; set; }

        /// <summary>
        /// Enables thumbnails.
        /// </summary>
        [JsonPropertyName("thumbnails")]
        public bool Thumbnails { get; set; }

        /// <summary>
        /// Image or HTML mode. 
        /// </summary>
        [JsonPropertyName("htmlMode")]
        public bool HtmlMode { get; set; }

        /// <summary>
        /// Enables printing
        /// </summary>
        [JsonPropertyName("printAllowed")]
        public bool PrintAllowed { get; set; }

        /// <summary>
        /// Enables rotation
        /// </summary>
        [JsonPropertyName("rotate")]
        public bool Rotate { get; set; }

        /// <summary>
        /// Enables saving of rotation state
        /// </summary>
        [JsonPropertyName("saveRotateState")]
        public bool SaveRotateState { get; set; }

        /// <summary>
        /// Default language e.g. "en".
        /// </summary>
        [JsonPropertyName("defaultLanguage")]
        public string DefaultLanguage { get; set; }

        /// <summary>
        /// Supported languages e.g. [ "en", "fr", "de" ]
        /// </summary>
        [JsonPropertyName("supportedLanguages")]
        public string[] SupportedLanguages { get; set; }

        /// <summary>
        /// Enables language menu.
        /// </summary>
        [JsonPropertyName("showLanguageMenu")]
        public bool ShowLanguageMenu { get; set; }

        /// <summary>
        /// Top toolbar show flag
        /// </summary>
        [JsonPropertyName("showToolBar")]
        public bool ShowToolBar { get; set; }
    }
}