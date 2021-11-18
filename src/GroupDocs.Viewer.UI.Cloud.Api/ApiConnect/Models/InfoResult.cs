using System.Collections.Generic;
using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// View result information
    /// </summary>  
    public class InfoResult
    {
        /// <summary>
        /// File format extension
        /// </summary>  
        public string FormatExtension { get; set; }

        /// <summary>
        /// File format
        /// </summary>  
        public string Format { get; set; }

        /// <summary>
        /// View result pages
        /// </summary>  
        public List<PageInfo> Pages { get; set; }

        /// <summary>
        /// Attachments
        /// </summary>  
        public List<AttachmentInfo> Attachments { get; set; }

        /// <summary>
        /// Represents view information for archive file
        /// </summary>  
        public ArchiveViewInfo ArchiveViewInfo { get; set; }

        /// <summary>
        /// Represents view information for CAD drawing
        /// </summary>  
        public CadViewInfo CadViewInfo { get; set; }

        /// <summary>
        /// Represents view information for MS Project document
        /// </summary>  
        public ProjectManagementViewInfo ProjectManagementViewInfo { get; set; }

        /// <summary>
        /// Represents view information for Outlook Data file
        /// </summary>  
        public OutlookViewInfo OutlookViewInfo { get; set; }

        /// <summary>
        /// Represents view information for PDF document
        /// </summary>  
        public PdfViewInfo PdfViewInfo { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class InfoResult {\n");
            sb.Append("  FormatExtension: ").Append(this.FormatExtension).Append("\n");
            sb.Append("  Format: ").Append(this.Format).Append("\n");
            sb.Append("  Pages: ").Append(this.Pages).Append("\n");
            sb.Append("  Attachments: ").Append(this.Attachments).Append("\n");
            sb.Append("  ArchiveViewInfo: ").Append(this.ArchiveViewInfo).Append("\n");
            sb.Append("  CadViewInfo: ").Append(this.CadViewInfo).Append("\n");
            sb.Append("  ProjectManagementViewInfo: ").Append(this.ProjectManagementViewInfo).Append("\n");
            sb.Append("  OutlookViewInfo: ").Append(this.OutlookViewInfo).Append("\n");
            sb.Append("  PdfViewInfo: ").Append(this.PdfViewInfo).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
