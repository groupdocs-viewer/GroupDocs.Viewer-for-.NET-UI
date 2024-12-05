using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class ViewDataResponse
    {
        /// <summary>
        /// File unique ID.
        /// </summary>
        [JsonPropertyName("file")]
        public string File { get; set; }

        /// <summary>
        /// File type e.g "docx".
        /// </summary>
        [JsonPropertyName("fileType")]
        public string FileType { get; set; }

        /// <summary>
        /// Indicates if printing of the document is allowed.
        /// </summary>
        [JsonPropertyName("canPrint")]
        public bool CanPrint { get; set; }

        /// <summary>
        /// Search term from back to UI search after load document.
        /// </summary>
        [JsonPropertyName("searchTerm")]
        public string SearchTerm { get; set; }

        /// <summary>
        /// Document pages.
        /// </summary>
        [JsonPropertyName("pages")]
        public List<PageData> Pages { get; set; }
    }
}