using System.Collections.Generic;
using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Represents relatively positioned rectangle which contains single word
    /// </summary>  
    public class Word : TextElement
    {
        /// <summary>
        /// The characters contained by the word
        /// </summary>  
        public List<Character> Characters { get; set; } = new();

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Word {\n");
            sb.Append("  Characters: ").Append(this.Characters).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
