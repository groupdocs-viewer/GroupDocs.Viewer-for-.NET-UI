using System.Collections.Generic;
using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Represents relatively positioned rectangle which contains single line
    /// </summary>  
    public class Line : TextElement
    {
        /// <summary>
        /// The words contained by the line
        /// </summary>  
        public List<Word> Words { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Line {\n");
            sb.Append("  Words: ").Append(this.Words).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
