using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Rotation angle
    /// </summary>
    /// <value>Rotation angle</value>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum RotationAngle
    {
        /// <summary>
        /// Enum On90Degree for "On90Degree"
        /// </summary>
        On90Degree,

        /// <summary>
        /// Enum On180Degree for "On180Degree"
        /// </summary>
        On180Degree,

        /// <summary>
        /// Enum On270Degree for "On270Degree"
        /// </summary>
        On270Degree
    }
}