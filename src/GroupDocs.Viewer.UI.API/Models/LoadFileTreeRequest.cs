using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class LoadFileTreeRequest
    {
        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;
    }
}