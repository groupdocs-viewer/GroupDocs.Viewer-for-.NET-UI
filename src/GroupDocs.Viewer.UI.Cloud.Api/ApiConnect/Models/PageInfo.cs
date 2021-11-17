using System.Collections.Generic;
using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Page information
    /// </summary>  
    public class PageInfo
    {
        /// <summary>
        /// The page number
        /// </summary>  
        public int? Number { get; set; }

        /// <summary>
        /// The width of the page in pixels when viewing as JPG or PNG
        /// </summary>  
        public int? Width { get; set; }

        /// <summary>
        /// The height of the page in pixels when viewing as JPG or PNG
        /// </summary>  
        public int? Height { get; set; }

        /// <summary>
        /// The page visibility indicator
        /// </summary>  
        public bool Visible { get; set; }

        /// <summary>
        /// The lines contained by the page when viewing as JPG or PNG with enabled Text Extraction
        /// </summary>  
        public List<Line> Lines { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class PageInfo {\n");
            sb.Append("  Number: ").Append(this.Number).Append("\n");
            sb.Append("  Width: ").Append(this.Width).Append("\n");
            sb.Append("  Height: ").Append(this.Height).Append("\n");
            sb.Append("  Visible: ").Append(this.Visible).Append("\n");
            sb.Append("  Lines: ").Append(this.Lines).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
