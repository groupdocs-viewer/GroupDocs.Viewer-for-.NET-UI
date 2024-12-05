using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// View format (HTML, PNG, JPG, or PDF) Default value is HTML.
    /// </summary>
    /// <value>View format (HTML, PNG, JPG, or PDF) Default value is HTML.</value>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ViewFormat
    {
        /// <summary>
        /// Enum HTML for "HTML"
        /// </summary>
        HTML,

        /// <summary>
        /// Enum PNG for "PNG"
        /// </summary>
        PNG,

        /// <summary>
        /// Enum JPG for "JPG"
        /// </summary>
        JPG,

        /// <summary>
        /// Enum PDF for "PDF"
        /// </summary>
        PDF
    }
}