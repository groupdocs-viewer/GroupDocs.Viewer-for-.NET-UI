using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// The time unit to use as minimal point.
    /// </summary>
    /// <value>The time unit to use as minimal point.</value>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TimeUnit
    {
        /// <summary>
        /// Enum Unspecified for "Unspecified"
        /// </summary>
        Unspecified,

        /// <summary>
        /// Enum Days for "Days"
        /// </summary>
        Days,

        /// <summary>
        /// Enum ThirdsOfMonths for "ThirdsOfMonths"
        /// </summary>
        ThirdsOfMonths,

        /// <summary>
        /// Enum Months for "Months"
        /// </summary>
        Months
    }
}