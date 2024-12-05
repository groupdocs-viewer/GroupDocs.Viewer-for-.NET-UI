using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class CreatePdfResponse
    {
        /// <summary>
        /// Url to download PDF file.
        /// </summary>
        [JsonPropertyName("pdfUrl")]
        public string PdfUrl { get; set; }
    }
}