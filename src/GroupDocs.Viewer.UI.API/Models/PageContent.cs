using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class PageContent
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }
    }
}