namespace GroupDocs.Viewer.UI.Core.Configuration
{
    public class Config
    {
        /// <summary>
        /// Rendering mode for the UI.
        /// Possible values: <c>RenderingMode.Html</c>, <c>RenderingMode.Image</c>.
        /// Default value: <c>RenderingMode.Html</c>.
        /// </summary>
        public RenderingMode RenderingMode { get; set; } = RenderingMode.Html;

        /// <summary>
        /// When enabled, the app uses pre-generated static content via GET requests.
        /// Default value: <c>false</c>.
        /// </summary>
        public bool StaticContentMode { get; set; } = false;

        /// <summary>
        /// File to load by default. Set this property to a relative file path or a file ID.
        /// By default, no file is loaded unless this property is set or a file name is specified 
        /// through the <c>file</c> query string parameter, e.g., <c>file=annual-review.docx</c>.
        /// </summary>
        public string InitialFile { get; set; }

        /// <summary>
        /// Number of pages to preload. Default value: <c>3</c>. 
        /// Set to <c>0</c> to render all pages at once.
        /// This property also determines how many pages are loaded in subsequent requests.
        /// The UI respects scroll direction: when scrolling down, the next pages are loaded; 
        /// when scrolling up, the previous pages are loaded.
        /// </summary>
        /// <remarks>
        /// <para>If you're using the API without the UI, you can control this setting
        /// in the service configuration:</para>
        /// <code>
        /// using GroupDocs.Viewer.UI.Core.Configuration;
        ///
        /// var builder = WebApplication.CreateBuilder(args);
        ///
        /// builder.Services
        ///     .AddOptions&lt;Config&gt;()
        ///     .Configure&lt;IConfiguration&gt;((config, configuration) =>
        ///     {
        ///         config.PreloadPages = 0; // Preload all pages at once
        ///     });
        /// </code>
        /// </remarks>
        public int PreloadPages { get; set; } = 3;

        /// <summary>
        /// Initial zoom level. If not specified, the UI automatically sets the zoom level.
        /// </summary>
        public ZoomLevel InitialZoom { get; set; }

        /// <summary>
        /// Enable or disable the right-click context menu.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnableContextMenu { get; set; } = true;

        /// <summary>
        /// Enable or disable clickable links in documents.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnableHyperlinks { get; set; } = true;

        /// <summary>
        /// Enables or disables scroll animation when navigating to a page using page seclector control.
        /// Default value: <c>true</c> and the scroll animation is enabled.
        /// </summary>
        public bool EnableScrollAnimation { get; set; } = true;

        /* Control Visibility Settings */

        /// <summary>
        /// Show or hide the header.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnableHeader { get; set; } = true;

        /// <summary>
        /// Show or hide the toolbar.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnableToolbar { get; set; } = true;

        /// <summary>
        /// Show or hide the file name.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnableFileName { get; set; } = true;

        /// <summary>
        /// Show or hide the thumbnails pane.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnableThumbnails { get; set; } = true;

        /// <summary>
        /// Show or hide the zoom controls.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnableZoom { get; set; } = true;

        /// <summary>
        /// Show or hide the page navigation menu.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnablePageSelector { get; set; } = true;

        /// <summary>
        /// Show or hide the search control.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnableSearch { get; set; } = true;

        /// <summary>
        /// Show or hide the "Print" button.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnablePrint { get; set; } = true;

        /// <summary>
        /// Show or hide the "Download PDF" button.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnableDownloadPdf { get; set; } = true;

        /// <summary>
        /// Show or hide the "Present" button.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnablePresentation { get; set; } = true;

        /// <summary>
        /// Show or hide the "File Browser" button.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnableFileBrowser { get; set; } = true;

        /// <summary>
        /// Show or hide the "Upload File" button.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnableFileUpload { get; set; } = true;

        /* Language and Localization Settings */

        /// <summary>
        /// Show or hide the language menu.
        /// Default value: <c>true</c>.
        /// </summary>
        public bool EnableLanguageSelector { get; set; } = true;

        /// <summary>
        /// Default language code.
        /// Default value: <c>en</c>.
        /// </summary>
        public LanguageCode DefaultLanguage { get; set; } = LanguageCode.English;

        /// <summary>
        /// List of supported language codes.
        /// </summary>
        public LanguageCode[] SupportedLanguages { get; set; } = LanguageCode.All;
    }
}
