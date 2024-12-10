using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Watermark position. Default value is \&quot;Diagonal\&quot;.
    /// </summary>
    /// <value>Watermark position. Default value is \&quot;Diagonal\&quot;.</value>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Position
    {
        /// <summary>
        /// Enum Diagonal for "Diagonal"
        /// </summary>
        Diagonal,

        /// <summary>
        /// Enum TopLeft for "TopLeft"
        /// </summary>
        TopLeft,

        /// <summary>
        /// Enum TopCenter for "TopCenter"
        /// </summary>
        TopCenter,

        /// <summary>
        /// Enum TopRight for "TopRight"
        /// </summary>
        TopRight,

        /// <summary>
        /// Enum BottomLeft for "BottomLeft"
        /// </summary>
        BottomLeft,

        /// <summary>
        /// Enum BottomCenter for "BottomCenter"
        /// </summary>
        BottomCenter,

        /// <summary>
        /// Enum BottomRight for "BottomRight"
        /// </summary>
        BottomRight
    }
}