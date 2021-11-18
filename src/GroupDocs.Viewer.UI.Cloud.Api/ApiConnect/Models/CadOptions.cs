using System.Collections.Generic;
using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Rendering options for CAD file formats. CAD file formats include files with extensions: .dwg, .dxf, .dgn, .ifc, .stl
    /// </summary>  
    public class CadOptions
    {
        /// <summary>
        /// Scale factor allows to change the size of the output document. Values higher than 1 will enlarge output result and values between 0 and 1 will make output result smaller. This option is ignored when either Height or Width options are set. 
        /// </summary>  
        public double? ScaleFactor { get; set; }

        /// <summary>
        /// Width of the output result in pixels        
        /// </summary>  
        public int? Width { get; set; }

        /// <summary>
        /// Height of the output result in pixels     
        /// </summary>  
        public int? Height { get; set; }

        /// <summary>
        /// The drawing regions to render This option supported only for DWG and DWT file types The RenderLayouts and LayoutName options are ignored when rendering by tiles
        /// </summary>  
        public List<Tile> Tiles { get; set; } = new List<Tile>();

        /// <summary>
        /// Indicates whether layouts from CAD document should be rendered
        /// </summary>  
        public bool RenderLayouts { get; set; }

        /// <summary>
        /// The name of the specific layout to render. Layout name is case-sensitive
        /// </summary>  
        public string LayoutName { get; set; }

        /// <summary>
        /// The CAD drawing layers to render By default all layers are rendered; Layer names are case-sensitive
        /// </summary>  
        public List<string> Layers { get; set; } = new List<string>();

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class CadOptions {\n");
            sb.Append("  ScaleFactor: ").Append(this.ScaleFactor).Append("\n");
            sb.Append("  Width: ").Append(this.Width).Append("\n");
            sb.Append("  Height: ").Append(this.Height).Append("\n");
            sb.Append("  Tiles: ").Append(this.Tiles).Append("\n");
            sb.Append("  RenderLayouts: ").Append(this.RenderLayouts).Append("\n");
            sb.Append("  LayoutName: ").Append(this.LayoutName).Append("\n");
            sb.Append("  Layers: ").Append(this.Layers).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
