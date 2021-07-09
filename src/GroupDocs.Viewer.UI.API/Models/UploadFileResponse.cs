using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class UploadFileResponse
    {
        [JsonPropertyName("guid")]
        public string Guid { get; }

        public UploadFileResponse(string filePath)
        {
            Guid = filePath;
        }
    }
}