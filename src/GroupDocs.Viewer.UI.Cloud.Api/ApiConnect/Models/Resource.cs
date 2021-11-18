using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Reference to resource
    /// </summary>  
    public class Resource
    {
        /// <summary>
        /// Path of resource file in storage
        /// </summary>  
        public string Path { get; set; }

        /// <summary>
        /// ULR to retrieve resource.
        /// </summary>  
        public string DownloadUrl { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Resource {\n");
            sb.Append("  Path: ").Append(this.Path).Append("\n");
            sb.Append("  DownloadUrl: ").Append(this.DownloadUrl).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
