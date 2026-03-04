using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class CreatePagesRequest
    {
        /// <summary>
        /// File unique ID.
        /// </summary>
        [JsonPropertyName("file")]
        public string File { get; set; }

        /// <summary>
        /// File type e.g. "docx".
        /// </summary>
        [JsonPropertyName("fileType")]
        public string FileType { get; set; }

        /// <summary>
        /// The password to open a document.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; }

        /// <summary>
        /// The pages to return.
        /// </summary>
        [JsonPropertyName("pages")]
        public int[] Pages { get; set; }
    }
}