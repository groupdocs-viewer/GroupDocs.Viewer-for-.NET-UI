using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Text element
    /// </summary>  
    public class TextElement
    {
        /// <summary>
        /// The X coordinate of the highest left point on the page layout where the rectangle that contains element begins.             
        /// </summary>  
        public double? X { get; set; }

        /// <summary>
        /// The Y coordinate of the highest left point on the page layout where the rectangle that contains element begins.             
        /// </summary>  
        public double? Y { get; set; }

        /// <summary>
        /// The width of the rectangle which contains the element (in pixels).              
        /// </summary>  
        public double? Width { get; set; }

        /// <summary>
        /// The height of the rectangle which contains the element (in pixels).              
        /// </summary>  
        public double? Height { get; set; }

        /// <summary>
        /// The element value
        /// </summary>  
        public string Value { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class TextElement {\n");
            sb.Append("  X: ").Append(this.X).Append("\n");
            sb.Append("  Y: ").Append(this.Y).Append("\n");
            sb.Append("  Width: ").Append(this.Width).Append("\n");
            sb.Append("  Height: ").Append(this.Height).Append("\n");
            sb.Append("  Value: ").Append(this.Value).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
