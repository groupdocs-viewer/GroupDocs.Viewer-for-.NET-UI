using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Provides options for rendering archive files
    /// </summary>  
    public class ArchiveOptions
    {
        /// <summary>
        /// The folder inside the archive to be rendered
        /// </summary>  
        public string Folder { get; set; }

        /// <summary>
        /// The filename to display in the header. By default the name of the source file is displayed.
        /// </summary>  
        public string FileName { get; set; }

        /// <summary>
        /// Number of records per page (for rendering to HTML only)             
        /// </summary>  
        public int? ItemsPerPage { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ArchiveOptions {\n");
            sb.Append("  Folder: ").Append(this.Folder).Append("\n");
            sb.Append("  FileName: ").Append(this.FileName).Append("\n");
            sb.Append("  ItemsPerPage: ").Append(this.ItemsPerPage).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
