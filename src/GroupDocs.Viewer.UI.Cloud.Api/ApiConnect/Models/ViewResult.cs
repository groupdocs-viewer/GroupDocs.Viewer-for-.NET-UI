using System.Collections.Generic;
using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// View result information
    /// </summary>  
    public class ViewResult
    {
        /// <summary>
        /// View result pages
        /// </summary>  
        public List<PageView> Pages { get; set; } = new();

        /// <summary>
        /// Attachments
        /// </summary>  
        public List<AttachmentView> Attachments { get; set; } = new();

        /// <summary>
        /// ULR to retrieve file.
        /// </summary>  
        public Resource File { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ViewResult {\n");
            sb.Append("  Pages: ").Append(this.Pages).Append("\n");
            sb.Append("  Attachments: ").Append(this.Attachments).Append("\n");
            sb.Append("  File: ").Append(this.File).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
