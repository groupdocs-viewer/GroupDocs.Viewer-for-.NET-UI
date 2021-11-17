using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Represents layout contained by the CAD drawing
    /// </summary>  
    public class Layout
    {
        /// <summary>
        /// The name of the layout
        /// </summary>  
        public string Name { get; set; }

        /// <summary>
        /// The width of the layout
        /// </summary>  
        public double? Width { get; set; }

        /// <summary>
        /// The height of the layout
        /// </summary>  
        public double? Height { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Layout {\n");
            sb.Append("  Name: ").Append(this.Name).Append("\n");
            sb.Append("  Width: ").Append(this.Width).Append("\n");
            sb.Append("  Height: ").Append(this.Height).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
