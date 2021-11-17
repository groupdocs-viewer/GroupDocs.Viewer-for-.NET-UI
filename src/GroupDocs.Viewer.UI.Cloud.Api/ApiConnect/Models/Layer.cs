using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Represents layer contained by the CAD drawing
    /// </summary>  
    public class Layer
    {
        /// <summary>
        /// The name of the layer
        /// </summary>  
        public string Name { get; set; }

        /// <summary>
        /// The layer visibility indicator
        /// </summary>  
        public bool Visible { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Layer {\n");
            sb.Append("  Name: ").Append(this.Name).Append("\n");
            sb.Append("  Visible: ").Append(this.Visible).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
