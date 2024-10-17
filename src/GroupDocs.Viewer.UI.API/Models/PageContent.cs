using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class PageContent
    {
        /// <summary>
        /// Page number.
        /// </summary>
        [JsonPropertyName("number")]
        public int Number { get; set; }

        /// <summary>
        /// Page contents. It can be HTML or base64-encoded image.
        /// </summary>
        [JsonPropertyName("data")]
        public string Data { get; set; }

        [JsonPropertyName("htmlData")]
        public string HtmlData { get; set; }
    }
}