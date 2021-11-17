using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Provides options for rendering text documents
    /// </summary>  
    public class TextOptions
    {
        /// <summary>
        /// Max chars per row on page. Default value is 85.
        /// </summary>  
        public int? MaxCharsPerRow { get; set; }

        /// <summary>
        /// Max rows per page. Default value is 55.
        /// </summary>  
        public int? MaxRowsPerPage { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class TextOptions {\n");
            sb.Append("  MaxCharsPerRow: ").Append(this.MaxCharsPerRow).Append("\n");
            sb.Append("  MaxRowsPerPage: ").Append(this.MaxRowsPerPage).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
