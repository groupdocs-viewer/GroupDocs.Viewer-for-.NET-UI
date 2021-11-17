using System;
using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models 
{
    /// <summary>
    /// Rendering options for Project file formats. Project file formats include files with extensions: .mpt, .mpp
    /// </summary>  
    public class ProjectManagementOptions 
    {
        /// <summary>
        /// The size of the page.
        /// </summary>
        public PageSize? PageSize { get; set; }

        /// <summary>
        /// The time unit to use as minimal point.
        /// </summary>
        public TimeUnit? TimeUnit { get; set; }

        /// <summary>
        /// The start date of a Gantt Chart View to render.        
        /// </summary>  
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// The end date of a Gantt Chart View to render.
        /// </summary>  
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()  
        {
          var sb = new StringBuilder();
          sb.Append("class ProjectManagementOptions {\n");
          sb.Append("  PageSize: ").Append(this.PageSize).Append("\n");
          sb.Append("  TimeUnit: ").Append(this.TimeUnit).Append("\n");
          sb.Append("  StartDate: ").Append(this.StartDate).Append("\n");
          sb.Append("  EndDate: ").Append(this.EndDate).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
        }
    }
}


