using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// File-format
    /// </summary>  
    public class Format
    {
        /// <summary>
        /// File extension
        /// </summary>  
        public string Extension { get; set; }

        /// <summary>
        /// File format
        /// </summary>  
        public string FileFormat { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Format {\n");
            sb.Append("  Extension: ").Append(this.Extension).Append("\n");
            sb.Append("  FileFormat: ").Append(this.FileFormat).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
