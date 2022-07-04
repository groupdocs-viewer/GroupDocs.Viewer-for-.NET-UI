using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class UploadFileResponse
    {
        /// <summary>
        /// Unique file ID.
        /// </summary>
        [JsonPropertyName("guid")]
        public string Guid { get; }

        /// <summary>
        /// .ctor
        /// </summary>
        public UploadFileResponse(string filePath)
        {
            Guid = filePath;
        }
    }
}