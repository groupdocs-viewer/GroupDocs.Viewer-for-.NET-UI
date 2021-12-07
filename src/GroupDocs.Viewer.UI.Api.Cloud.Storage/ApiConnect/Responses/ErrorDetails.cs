using System;
using System.Text;

namespace GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Responses
{
    /// <summary>
    /// The error details
    /// </summary>  
    public class ErrorDetails
    {
        /// <summary>
        /// The request id
        /// </summary>  
        public string RequestId { get; set; }

        /// <summary>
        /// Date
        /// </summary>  
        public DateTime Date { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ErrorDetails {\n");
            sb.Append("  RequestId: ").Append(this.RequestId).Append("\n");
            sb.Append("  Date: ").Append(this.Date).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
