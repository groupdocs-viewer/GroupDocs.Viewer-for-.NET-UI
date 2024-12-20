namespace GroupDocs.Viewer.UI.Core.Configuration
{
    public class Config
    {
        /// <summary>
        /// Rendering mode for the UI.
        /// Possible values: "RenderingMode.Html", "RenderingMode.Image".
        /// Default value is "RenderingMode.Html".
        /// </summary>
        public RenderingMode RenderingMode { get; set; } = RenderingMode.Html;

        /// <summary>
        /// When enabled app will use pre-generated static content via GET requests.
        /// Default value is false.
        /// </summary>
        public bool StaticContentMode { get; set; } = false;

        /// <summary>
        /// File to load by default
        /// </summary>
        public string InitialFile { get; set; }

        /// <summary>
        ///  Number of pages to preload
        /// </summary>
        public int PreloadPages { get; set; } = 3;

        /// <summary>
        /// Enable or disable right-click context menu
        /// </summary>
        public bool EnableContextMenu { get; set; } = true;

        /// <summary>
        /// Enable or disable clickable links in documents
        /// </summary>
        public bool EnableHyperlinks { get; set; } = true;

        /* Control Visibility Settings */

        /// <summary>
        /// Show or hide header
        /// </summary>
        public bool EnableHeader { get; set; } = true;

        /// <summary>
        /// Show or hide header
        /// </summary>
        public bool EnableToolbar { get; set; } = true;

        /// <summary>
        /// Show or hide filename
        /// </summary>
        public bool EnableFileName { get; set; } = true;

        /// <summary>
        /// Show or hide thumbnails pane
        /// </summary>
        public bool EnableThumbnails { get; set; } = true;

        /// <summary>
        /// Show or hide zoom controls
        /// </summary>
        public bool EnableZoom { get; set; } = true;

        /// <summary>
        /// Show or hide page navigation menu
        /// </summary>
        public bool EnablePageSelector { get; set; } = true;

        /// <summary>
        /// Show or hide search control
        /// </summary>
        public bool EnableSearch { get; set; } = true;

        /// <summary>
        /// Show or hide "Print" button
        /// </summary>
        public bool EnablePrint { get; set; } = true;

        /// <summary>
        /// Show or hide "Download PDF" button
        /// </summary>
        public bool EnableDownloadPdf { get; set; } = true;

        /// <summary>
        /// Show or hide "Present" button
        /// </summary>
        public bool EnablePresentation { get; set; } = true;

        /// <summary>
        /// Show or hide "File Browser" button
        /// </summary>
        public bool EnableFileBrowser { get; set; } = true;

        /// <summary>
        ///  Show or hide "Upload File" button
        /// </summary>
        public bool EnableFileUpload { get; set; } = true;

     

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
