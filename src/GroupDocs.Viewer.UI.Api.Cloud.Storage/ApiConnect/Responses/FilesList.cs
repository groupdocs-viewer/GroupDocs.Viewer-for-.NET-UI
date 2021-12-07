using System.Collections.Generic;
using System.Text;

namespace GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Responses
{
    /// <summary>
    /// Files list
    /// </summary>  
    public class FilesList
    {
        /// <summary>
        /// Files and folders contained by folder StorageFile.
        /// </summary>  
        public List<StorageFile> Value { get; set; } = new List<StorageFile>();

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class FilesList {\n");
            sb.Append("  Value: ").Append(this.Value).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
