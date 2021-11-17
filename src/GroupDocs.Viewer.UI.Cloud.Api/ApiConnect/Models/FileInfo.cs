using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// File info
    /// </summary>  
    public class FileInfo
    {
        /// <summary>
        /// File path in storage
        /// </summary>  
        public string FilePath { get; set; }

        /// <summary>
        /// Storage name
        /// </summary>  
        public string StorageName { get; set; }

        /// <summary>
        /// Version ID
        /// </summary>  
        public string VersionId { get; set; }

        /// <summary>
        /// Password to open file
        /// </summary>  
        public string Password { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class FileInfo {\n");
            sb.Append("  FilePath: ").Append(this.FilePath).Append("\n");
            sb.Append("  StorageName: ").Append(this.StorageName).Append("\n");
            sb.Append("  VersionId: ").Append(this.VersionId).Append("\n");
            sb.Append("  Password: ").Append(this.Password).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
