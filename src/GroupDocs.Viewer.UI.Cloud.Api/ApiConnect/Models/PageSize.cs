using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// The size of the output page when rendering as PDF or image.
    /// </summary>
    /// <value>The size of the output page when rendering as PDF or image.</value>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PageSize
    {
        /// <summary>
        /// Enum Unspecified for "Unspecified"
        /// </summary>            
        Unspecified,

        /// <summary>
        /// Enum Letter for "Letter"
        /// </summary>            
        Letter,

        /// <summary>
        /// Enum Ledger for "Ledger"
        /// </summary>            
        Ledger,

        /// <summary>
        /// Enum A0 for "A0"
        /// </summary>            
        A0,

        /// <summary>
        /// Enum A1 for "A1"
        /// </summary>            
        A1,

        /// <summary>
        /// Enum A2 for "A2"
        /// </summary>            
        A2,

        /// <summary>
        /// Enum A3 for "A3"
        /// </summary>            
        A3,

        /// <summary>
        /// Enum A4 for "A4"
        /// </summary>            
        A4
    }
}