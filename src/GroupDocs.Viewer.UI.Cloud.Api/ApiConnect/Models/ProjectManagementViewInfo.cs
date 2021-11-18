using System;
using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models 
{
    /// <summary>
    /// Represents view information for MS Project document
    /// </summary>  
    public class ProjectManagementViewInfo 
    {                       
        /// <summary>
        /// The date time from which the project started
        /// </summary>  
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The date time when the project is to be completed
        /// </summary>  
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()  
        {
          var sb = new StringBuilder();
          sb.Append("class ProjectManagementViewInfo {\n");
          sb.Append("  StartDate: ").Append(this.StartDate).Append("\n");
          sb.Append("  EndDate: ").Append(this.EndDate).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
        }
    }
}
