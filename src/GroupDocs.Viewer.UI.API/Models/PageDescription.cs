﻿using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class PageDescription : PageContent
    {
        /// <summary>
        /// Page with in pixels.
        /// </summary>
        [JsonPropertyName("width")]
        public int Width { get; set; }

        /// <summary>
        /// Page height in pixels.
        /// </summary>
        [JsonPropertyName("height")]
        public int Height { get; set; }

        /// <summary>
        /// Worksheet name for spreadsheets.
        /// </summary>
        [JsonPropertyName("sheetName")]
        public string SheetName { get; set; }
    }
}