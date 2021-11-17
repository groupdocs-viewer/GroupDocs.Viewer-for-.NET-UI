using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Represents drawing region
    /// </summary>  
    public class Tile
    {
        /// <summary>
        /// The X coordinate of the lowest left point on the drawing where the tile begins
        /// </summary>  
        public int? StartPointX { get; set; }

        /// <summary>
        /// The Y coordinate of the lowest left point on the drawing where the tile begins
        /// </summary>  
        public int? StartPointY { get; set; }

        /// <summary>
        /// The width of the tile in pixels
        /// </summary>  
        public int? Width { get; set; }

        /// <summary>
        /// The height of the tile in pixels
        /// </summary>  
        public int? Height { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Tile {\n");
            sb.Append("  StartPointX: ").Append(this.StartPointX).Append("\n");
            sb.Append("  StartPointY: ").Append(this.StartPointY).Append("\n");
            sb.Append("  Width: ").Append(this.Width).Append("\n");
            sb.Append("  Height: ").Append(this.Height).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
