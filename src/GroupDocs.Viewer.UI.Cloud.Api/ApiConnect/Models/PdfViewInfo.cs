using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models 
{
    /// <summary>
    /// Represents view information for PDF document
    /// </summary>  
    public class PdfViewInfo 
    {                       
        /// <summary>
        /// Indicates if printing of the document is allowed
        /// </summary>  
        public bool PrintingAllowed { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()  
        {
          var sb = new StringBuilder();
          sb.Append("class PdfViewInfo {\n");
          sb.Append("  PrintingAllowed: ").Append(this.PrintingAllowed).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
        }
    }
}
