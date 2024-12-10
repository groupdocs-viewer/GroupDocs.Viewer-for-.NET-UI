using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Options for rendering document into image
    /// </summary>
    public class ImageOptions : RenderOptions
    {
        /// <summary>
        /// Allows to specify output image width.  Specify image width in case when you want to change output image dimensions. When Width has value and Height value is 0 then Height value will be calculated  to save image proportions.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Allows to specify output image height.  Specify image height in case when you want to change output image dimensions. When Height has value and Width value is 0 then Width value will be calculated  to save image proportions.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// When enabled Viewer will extract text when it's possible (e.g. raster formats don't have text layer) and return it in the viewing result. This option might be useful when you want to add selectable text layer over the image.
        /// </summary>
        public bool ExtractText { get; set; }

        /// <summary>
        /// Allows to specify quality when rendering as JPG. Valid values are between 1 and 100.  Default value is 90.
        /// </summary>
        public int? JpegQuality { get; set; }

        /// <summary>
        /// Max width of an output image in pixels
        /// </summary>
        public int? MaxWidth { get; set; }

        /// <summary>
        /// Max height of an output image in pixels
        /// </summary>
        public int? MaxHeight { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ImageOptions {\n");
            sb.Append("  Width: ").Append(this.Width).Append("\n");
            sb.Append("  Height: ").Append(this.Height).Append("\n");
            sb.Append("  ExtractText: ").Append(this.ExtractText).Append("\n");
            sb.Append("  JpegQuality: ").Append(this.JpegQuality).Append("\n");
            sb.Append("  MaxWidth: ").Append(this.MaxWidth).Append("\n");
            sb.Append("  MaxHeight: ").Append(this.MaxHeight).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
