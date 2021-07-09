using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class LoadDocumentDescriptionRequest
    {
        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}