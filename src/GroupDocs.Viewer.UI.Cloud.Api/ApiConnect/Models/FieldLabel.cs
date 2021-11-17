using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Represents field label 
    /// </summary>  
    public class FieldLabel
    {
        /// <summary>
        /// The field name e.g. \"From\"
        /// </summary>  
        public string Field { get; set; }

        /// <summary>
        /// The label e.g. \"Sender\"
        /// </summary>  
        public string Label { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class FieldLabel {\n");
            sb.Append("  Field: ").Append(this.Field).Append("\n");
            sb.Append("  Label: ").Append(this.Label).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
