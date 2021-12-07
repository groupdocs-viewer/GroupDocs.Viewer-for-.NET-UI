using System;
using System.Text;

namespace GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Responses
{
    /// <summary>
    /// File or folder information
    /// </summary>  
    public class StorageFile
    {
        /// <summary>
        /// File or folder name.
        /// </summary>  
        public string Name { get; set; }

        /// <summary>
        /// True if it is a folder.
        /// </summary>  
        public bool IsFolder { get; set; }

        /// <summary>
        /// File or folder last modified DateTime.
        /// </summary>  
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// File or folder size.
        /// </summary>  
        public long Size { get; set; }

        /// <summary>
        /// File or folder path.
        /// </summary>  
        public string Path { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class StorageFile {\n");
            sb.Append("  Name: ").Append(this.Name).Append("\n");
            sb.Append("  IsFolder: ").Append(this.IsFolder).Append("\n");
            sb.Append("  ModifiedDate: ").Append(this.ModifiedDate).Append("\n");
            sb.Append("  Size: ").Append(this.Size).Append("\n");
            sb.Append("  Path: ").Append(this.Path).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}