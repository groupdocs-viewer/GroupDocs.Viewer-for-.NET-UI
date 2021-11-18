using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// The error result
    /// </summary>  
    public class ErrorResult
    {
        /// <summary>
        /// The error
        /// </summary>
        public Error Error { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ErrorResult {\n");
            sb.Append("  Error: ").Append(this.Error).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
