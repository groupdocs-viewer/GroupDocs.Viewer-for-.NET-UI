using System.Collections.Generic;
using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models 
{
    /// <summary>
    /// Page information
    /// </summary>  
    public class PageView : Resource 
    {                       
        /// <summary>
        /// Page number
        /// </summary>  
        public int Number { get; set; }

        /// <summary>
        /// HTML resources.
        /// </summary>  
        public List<HtmlResource> Resources { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()  
        {
          var sb = new StringBuilder();
          sb.Append("class PageView {\n");
          sb.Append("  Number: ").Append(this.Number).Append("\n");
          sb.Append("  Resources: ").Append(this.Resources).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
        }
    }
}
