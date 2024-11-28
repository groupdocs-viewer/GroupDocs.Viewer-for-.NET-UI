namespace GroupDocs.Viewer.UI.Core.Configuration
{
    public class Config
    {
        /// <summary>
        /// Rendering mode for the UI.
        /// Possible values: "html", "image".
        /// Default value is "html".
        /// </summary>
        public RenderingMode RenderingMode { get; set; } = RenderingMode.Html;

        /// <summary>
        /// File to load by default
        /// </summary>
        public string InitialFile { get; set; }

        /// <summary>
        ///  Number of pages to preload
        /// </summary>
        public int PreloadPages { get; set; } = 3;

        /* Control Visibility Settings */

        /// <summary>
        /// Show or hide page navigation menu
        /// </summary>
        public bool EnablePageSelector { get; set; } = true;

        /// <summary>
        /// Show or hide "Download PDF" button
        /// </summary>
        public bool EnableDownloadPdf { get; set; } = true;

        /// <summary>
        ///  Show or hide "Upload File" button
        /// </summary>
        public bool EnableFileUpload { get; set; } = true;

        /// <summary>
        /// Show or hide "File Browser" button
        /// </summary>
        public bool EnableFileBrowser { get; set; } = true;

        /// <summary>
        /// Enable or disable right-click context menu
        /// </summary>
        public bool EnableContextMenu { get; set; } = true;

        /// <summary>
        /// Show or hide zoom controls
        /// </summary>
        public bool EnableZoom { get; set; } = true;

        /// <summary>
        /// Show or hide search control
        /// </summary>
        public bool EnableSearch { get; set; } = true;

        /// <summary>
        /// Show or hide thumbnails pane
        /// </summary>
        public bool EnableThumbnails { get; set; } = true;

        /// <summary>
        /// Show or hide "Print" button
        /// </summary>
        public bool EnablePrint { get; set; } = true;

        /// <summary>
        /// Enable or disable clickable links in documents
        /// </summary>
        public bool EnableHyperlinks { get; set; } = true;

        /* Language and Localization Settings */

        /// <summary>
        /// Show or hide language menu
        /// </summary>
        public bool EnableLanguageSelector { get; set; } = true;

        /// <summary>
        /// Default language code. Default value is "en".
        /// </summary>
        public LanguageCode DefaultLanguage { get; set; } = LanguageCode.English;

        /// <summary>
        /// List of supported language codes.
        /// </summary>
        public LanguageCode[] SupportedLanguages { get; set; } = LanguageCode.All;
    }
}
