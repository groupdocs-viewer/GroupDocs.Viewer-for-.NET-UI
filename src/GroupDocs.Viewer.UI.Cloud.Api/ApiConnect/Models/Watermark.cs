using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Text watermark
    /// </summary>  
    public class Watermark
    {
        /// <summary>
        /// Watermark position. Default value is \&quot;Diagonal\&quot;.
        /// </summary>
        public Position? Position { get; set; }

        /// <summary>
        /// Watermark text.
        /// </summary>  
        public string Text { get; set; }

        /// <summary>
        /// Watermark color. Supported formats {Magenta|(112,222,11)|(50,112,222,11)}. Default value is \"Red\".
        /// </summary>  
        public string Color { get; set; }

        /// <summary>
        /// Watermark size in percents. Default value is 100.
        /// </summary>  
        public int? Size { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Watermark {\n");
            sb.Append("  Text: ").Append(this.Text).Append("\n");
            sb.Append("  Color: ").Append(this.Color).Append("\n");
            sb.Append("  Position: ").Append(this.Position).Append("\n");
            sb.Append("  Size: ").Append(this.Size).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
