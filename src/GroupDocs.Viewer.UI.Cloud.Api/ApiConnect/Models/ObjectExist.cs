using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Object exists
    /// </summary>  
    public class ObjectExist
    {
        /// <summary>
        /// Indicates that the file or folder exists.
        /// </summary>  
        public bool Exists { get; set; }

        /// <summary>
        /// True if it is a folder, false if it is a file.
        /// </summary>  
        public bool IsFolder { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ObjectExist {\n");
            sb.Append("  Exists: ").Append(this.Exists).Append("\n");
            sb.Append("  IsFolder: ").Append(this.IsFolder).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
