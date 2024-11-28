using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class UploadFileResponse
    {
        /// <summary>
        /// Unique file ID.
        /// </summary>
        [JsonPropertyName("file")]
        public string File { get; }

        /// <summary>
        /// .ctor
        /// </summary>
        public UploadFileResponse(string file)
        {
            File = file;
        }
    }
}