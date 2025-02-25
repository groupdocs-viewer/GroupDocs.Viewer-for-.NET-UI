﻿using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class CreatePdfRequest
    {
        /// <summary>
        /// Unique file ID.
        /// </summary>
        [JsonPropertyName("file")]
        public string File { get; set; }

        /// <summary>
        /// File type e.g. "docx".
        /// </summary>
        [JsonPropertyName("fileType")]
        public string FileType { get; set; }

        /// <summary>
        /// Password to open the document.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}