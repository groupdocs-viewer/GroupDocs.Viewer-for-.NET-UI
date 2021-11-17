using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// The Visio files processing documents view options.
    /// </summary>  
    public class VisioRenderingOptions
    {
        /// <summary>
        /// Render only Visio figures, not a diagram.
        /// </summary>  
        public bool RenderFiguresOnly { get; set; }

        /// <summary>
        /// Figure width, height will be calculated automatically. Default value is 100.
        /// </summary>  
        public int? FigureWidth { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class VisioRenderingOptions {\n");
            sb.Append("  RenderFiguresOnly: ").Append(this.RenderFiguresOnly).Append("\n");
            sb.Append("  FigureWidth: ").Append(this.FigureWidth).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
