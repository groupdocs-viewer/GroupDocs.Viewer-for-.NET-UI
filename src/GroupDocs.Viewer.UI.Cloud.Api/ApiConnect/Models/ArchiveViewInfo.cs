using System.Collections.Generic;
using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Represents view information for archive file
    /// </summary>  
    public class ArchiveViewInfo
    {
        /// <summary>
        /// The folders contained by the archive file
        /// </summary>  
        public List<string> Folders { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ArchiveViewInfo {\n");
            sb.Append("  Folders: ").Append(this.Folders).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
