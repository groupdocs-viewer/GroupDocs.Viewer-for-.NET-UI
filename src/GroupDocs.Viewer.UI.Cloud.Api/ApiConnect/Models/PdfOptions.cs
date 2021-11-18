using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models 
{
    /// <summary>
    /// Options for rendering document into PDF
    /// </summary>  
    public class PdfOptions : RenderOptions 
    {                       
        /// <summary>
        /// The quality of the JPG images contained by output PDF document; Valid values are between 1 and 100; Default value is 90
        /// </summary>  
        [JsonProperty]
        public int? JpgQuality { get; set; }

        /// <summary>
        /// The password required to open the PDF document
        /// </summary>  
        [JsonProperty]
        public string DocumentOpenPassword { get; set; }

        /// <summary>
        /// The password required to change permission settings; Using a permissions password  you can restrict printing, modification and data extraction
        /// </summary>  
        [JsonProperty]
        public string PermissionsPassword { get; set; }

        /// <summary>
        /// The array of PDF document permissions. Allowed values are: AllowAll, DenyPrinting, DenyModification, DenyDataExtraction, DenyAll Default value is AllowAll, if now permissions are set.
        /// </summary>  
        [JsonProperty]
        public List<string> Permissions { get; set; }

        /// <summary>
        /// Max width of an output image in pixels. (When converting single image to HTML only)
        /// </summary>  
        [JsonProperty]
        public int? ImageMaxWidth { get; set; }

        /// <summary>
        /// Max height of an output image in pixels. (When converting single image to HTML only)
        /// </summary>  
        [JsonProperty]
        public int? ImageMaxHeight { get; set; }

        /// <summary>
        /// The width of the output image in pixels. (When converting single image to HTML only)
        /// </summary>  
        [JsonProperty]
        public int? ImageWidth { get; set; }

        /// <summary>
        /// The height of an output image in pixels. (When converting single image to HTML only)
        /// </summary>  
        [JsonProperty]
        public int? ImageHeight { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()  
        {
          var sb = new StringBuilder();
          sb.Append("class PdfOptions {\n");
          sb.Append("  JpgQuality: ").Append(this.JpgQuality).Append("\n");
          sb.Append("  DocumentOpenPassword: ").Append(this.DocumentOpenPassword).Append("\n");
          sb.Append("  PermissionsPassword: ").Append(this.PermissionsPassword).Append("\n");
          sb.Append("  Permissions: ").Append(this.Permissions).Append("\n");
          sb.Append("  ImageMaxWidth: ").Append(this.ImageMaxWidth).Append("\n");
          sb.Append("  ImageMaxHeight: ").Append(this.ImageMaxHeight).Append("\n");
          sb.Append("  ImageWidth: ").Append(this.ImageWidth).Append("\n");
          sb.Append("  ImageHeight: ").Append(this.ImageHeight).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
        }
    }
}
