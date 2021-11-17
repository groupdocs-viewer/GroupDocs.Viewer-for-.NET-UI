using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Clockwise page rotation 
    /// </summary>  
    public class PageRotation
    {
        /// <summary>
        /// Rotation angle
        /// </summary>
        public RotationAngle? RotationAngle { get; set; }

        /// <summary>
        /// Page number to rotate
        /// </summary>  
        public int? PageNumber { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class PageRotation {\n");
            sb.Append("  PageNumber: ").Append(this.PageNumber).Append("\n");
            sb.Append("  RotationAngle: ").Append(this.RotationAngle).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
