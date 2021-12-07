using System.Text;

namespace GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Responses
{
    /// <summary>
    /// Error model
    /// </summary>  
    public class Error
    {
        /// <summary>
        /// Code             
        /// </summary>  
        public string Code { get; set; }

        /// <summary>
        /// Message             
        /// </summary>  
        public string Message { get; set; }

        /// <summary>
        /// Description             
        /// </summary>  
        public string Description { get; set; }

        /// <summary>
        /// Inner Error             
        /// </summary>  
        public ErrorDetails InnerError { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Error {\n");
            sb.Append("  Code: ").Append(this.Code).Append("\n");
            sb.Append("  Message: ").Append(this.Message).Append("\n");
            sb.Append("  Description: ").Append(this.Description).Append("\n");
            sb.Append("  InnerError: ").Append(this.InnerError).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
