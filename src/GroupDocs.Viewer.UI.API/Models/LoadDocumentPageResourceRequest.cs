using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class LoadDocumentPageResourceRequest
    {
        /// <summary>
        /// File unique ID.
        /// </summary>
        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// File type e.g. "docx".
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// The password to open a document.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The page number which the resource belongs to.
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// The resource name e.g. "s.css".
        /// </summary>
        public string ResourceName { get; set; }
    }
}