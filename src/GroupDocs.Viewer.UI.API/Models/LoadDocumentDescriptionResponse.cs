using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class DocumentDescriptionResponse
    {
        /// <summary>
        /// File unique ID.
        /// </summary>
        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// Indicates if printing of the document is allowed.
        /// </summary>
        [JsonPropertyName("printAllowed")]
        public bool PrintAllowed { get; set; }

        /// <summary>
        /// Document pages.
        /// </summary>
        [JsonPropertyName("pages")]
        public IEnumerable<PageDescription> Pages { get; set; }

        /// <summary>
        /// Total number of pages in the document.
        /// </summary>
        [JsonPropertyName("totalPagesCount")]
        public int TotalPagesCount { get; set; }
    }
}