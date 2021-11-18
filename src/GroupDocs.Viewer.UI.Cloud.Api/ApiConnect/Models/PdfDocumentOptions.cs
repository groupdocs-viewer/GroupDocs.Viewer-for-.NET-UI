using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models 
{
    /// <summary>
    /// Provides options for rendering PDF documents
    /// </summary>  
    public class PdfDocumentOptions 
    {
        /// <summary>
        /// Specifies output image quality for image resources when rendering into HTML. The default value is Low
        /// </summary>
        public ImageQuality? ImageQuality { get; set; }

        /// <summary>
        /// Disables chars grouping to keep maximum precision during chars positioning when rendering the page
        /// </summary>  
        public bool DisableCharsGrouping { get; set; }

        /// <summary>
        /// Enables rendering of text and graphics according to z-order in original PDF document  when rendering into HTML
        /// </summary>  
        public bool EnableLayeredRendering { get; set; }

        /// <summary>
        /// Enables font hinting. The font hinting adjusts the display of an outline font. Supported only for TTF fonts when these fonts are used in source document.
        /// </summary>  
        public bool EnableFontHinting { get; set; }

        /// <summary>
        /// When this option enabled the output pages will have the same size in pixels as page size in a source PDF document. By default GroupDocs.Viewer calculates output image page size for better rendering quality. This option is supported when rendering into PNG or JPG formats.
        /// </summary>  
        public bool RenderOriginalPageSize { get; set; }

        /// <summary>
        /// When this option is set to true, the text is rendered as an image in the output HTML. Enable this option to make text unselectable or to fix characters rendering and make HTML look like PDF. The default value is false. This option is supported when rendering into HTML.
        /// </summary>  
        public bool RenderTextAsImage { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()  
        {
          var sb = new StringBuilder();
          sb.Append("class PdfDocumentOptions {\n");
          sb.Append("  DisableCharsGrouping: ").Append(this.DisableCharsGrouping).Append("\n");
          sb.Append("  EnableLayeredRendering: ").Append(this.EnableLayeredRendering).Append("\n");
          sb.Append("  EnableFontHinting: ").Append(this.EnableFontHinting).Append("\n");
          sb.Append("  RenderOriginalPageSize: ").Append(this.RenderOriginalPageSize).Append("\n");
          sb.Append("  ImageQuality: ").Append(this.ImageQuality).Append("\n");
          sb.Append("  RenderTextAsImage: ").Append(this.RenderTextAsImage).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
        }
    }
}
