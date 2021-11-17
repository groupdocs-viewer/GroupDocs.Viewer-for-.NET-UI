using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Specifies output image quality for image resources when rendering into HTML. The default value is Low
    /// </summary>
    /// <value>Specifies output image quality for image resources when rendering into HTML. The default value is Low</value>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ImageQuality
    {
        /// <summary>
        /// Enum Low for "Low"
        /// </summary>            
        Low,

        /// <summary>
        /// Enum Medium for "Medium"
        /// </summary>            
        Medium,

        /// <summary>
        /// Enum High for "High"
        /// </summary>            
        High
    }
}