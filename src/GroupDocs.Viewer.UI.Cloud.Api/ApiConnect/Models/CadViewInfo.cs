using System.Collections.Generic;
using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Represents view information for CAD drawing
    /// </summary>  
    public class CadViewInfo
    {
        /// <summary>
        /// The list of layers contained by the CAD drawing
        /// </summary>  
        public List<Layer> Layers { get; set; } = new();

        /// <summary>
        /// The list of layouts contained by the CAD drawing
        /// </summary>  
        public List<Layout> Layouts { get; set; } = new();

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CadViewInfo {\n");
            sb.Append("  Layers: ").Append(this.Layers).Append("\n");
            sb.Append("  Layouts: ").Append(this.Layouts).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
