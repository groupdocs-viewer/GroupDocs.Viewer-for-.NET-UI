using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// The text overflow mode for rendering spreadsheet documents into HTML
    /// </summary>
    /// <value>The text overflow mode for rendering spreadsheet documents into HTML</value>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum TextOverflowMode
    {
        /// <summary>
        /// Enum Overlay for "Overlay"
        /// </summary>            
        Overlay,

        /// <summary>
        /// Enum OverlayIfNextIsEmpty for "OverlayIfNextIsEmpty"
        /// </summary>            
        OverlayIfNextIsEmpty,

        /// <summary>
        /// Enum AutoFitColumn for "AutoFitColumn"
        /// </summary>            
        AutoFitColumn,

        /// <summary>
        /// Enum HideText for "HideText"
        /// </summary>            
        HideText
    }
}