using System.Collections.Generic;
using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Options for rendering document into HTML
    /// </summary>
    public class HtmlOptions : RenderOptions
    {
        /// <summary>
        /// Controls output HTML document resources (styles, images and fonts) linking. By default this option is disabled and all the resources are embedded into HTML document.
        /// </summary>
        internal bool ExternalResources { get; set; }

        /// <summary>
        /// Path for the HTML resources (styles, images and fonts). For example when resource path is http://example.com/api/pages/{page-number}/resources/{resource-name} the {page-number} and {resource-name} templates will be replaced with page number and resource name accordingly. This option is ignored when ExternalResources option is disabled.
        /// </summary>
        internal string ResourcePath { get; set; }

        /// <summary>
        /// Enables HTML content and HTML resources minification
        /// </summary>
        public bool Minify { get; set; }

        /// <summary>
        /// When enabled prevents adding any fonts into HTML document
        /// </summary>
        public bool ExcludeFonts { get; set; }

        /// <summary>
        /// This option is supported for presentations only. The list of font names, to exclude from HTML document
        /// </summary>
        public List<string> FontsToExclude { get; set; } = new List<string>();

        /// <summary>
        /// Indicates whether to optimize output HTML for printing.
        /// </summary>
        public bool ForPrinting { get; set; }

        /// <summary>
        /// The height of an output image in pixels. (When converting single image to HTML only)
        /// </summary>
        public int? ImageHeight { get; set; }

        /// <summary>
        /// The width of the output image in pixels. (When converting single image to HTML only)
        /// </summary>
        public int? ImageWidth { get; set; }

        /// <summary>
        /// Max height of an output image in pixels. (When converting single image to HTML only)
        /// </summary>
        public int? ImageMaxHeight { get; set; }

        /// <summary>
        /// Max width of an output image in pixels. (When converting single image to HTML only)
        /// </summary>
        public int? ImageMaxWidth { get; set; }

        /// <summary>
        /// Enables HTML content will be rendered to single page
        /// </summary>
        public bool RenderToSinglePage { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class HtmlOptions {\n");
            sb.Append("  ExternalResources: ").Append(this.ExternalResources).Append("\n");
            sb.Append("  ResourcePath: ").Append(this.ResourcePath).Append("\n");
            sb.Append("  Minify: ").Append(this.Minify).Append("\n");
            sb.Append("  ExcludeFonts: ").Append(this.ExcludeFonts).Append("\n");
            sb.Append("  FontsToExclude: ").Append(this.FontsToExclude).Append("\n");
            sb.Append("  ForPrinting: ").Append(this.ForPrinting).Append("\n");
            sb.Append("  ImageHeight: ").Append(this.ImageHeight).Append("\n");
            sb.Append("  ImageWidth: ").Append(this.ImageWidth).Append("\n");
            sb.Append("  ImageMaxHeight: ").Append(this.ImageMaxHeight).Append("\n");
            sb.Append("  ImageMaxWidth: ").Append(this.ImageMaxWidth).Append("\n");
            sb.Append("  RenderToSinglePage: ").Append(this.RenderToSinglePage).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
