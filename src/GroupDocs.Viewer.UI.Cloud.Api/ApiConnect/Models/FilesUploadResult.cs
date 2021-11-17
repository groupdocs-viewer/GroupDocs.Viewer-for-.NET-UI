using System.Collections.Generic;
using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// File upload result
    /// </summary>  
    public class FilesUploadResult
    {
        /// <summary>
        /// List of uploaded file names
        /// </summary>  
        public List<string> Uploaded { get; set; } = new();

        /// <summary>
        /// List of errors.
        /// </summary>  
        public List<Error> Errors { get; set; } = new();

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class FilesUploadResult {\n");
            sb.Append("  Uploaded: ").Append(this.Uploaded).Append("\n");
            sb.Append("  Errors: ").Append(this.Errors).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
