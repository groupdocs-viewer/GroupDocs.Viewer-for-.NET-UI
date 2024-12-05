using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class PageData
    {
        public PageData(int number, int width, int height)
        {
            Number = number;
            Width = width;
            Height = height;
        }

        public PageData(int number, int width, int height, string pageUrl)
        {
            Number = number;
            Width = width;
            Height = height;
            PageUrl = pageUrl;
        }

        public PageData(int number, int width, int height, string pageUrl, string thumbUrl)
        {
            Number = number;
            Width = width;
            Height = height;
            PageUrl = pageUrl;
            ThumbUrl = thumbUrl;
        }

        /// <summary>
        /// Page number.
        /// </summary>
        [JsonPropertyName("number")]
        public int Number { get; set; }

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
        /// The URL to retrieve the page.
        /// </summary>
        [JsonPropertyName("pageUrl")]
        public string PageUrl { get; set; }

        /// <summary>
        /// The URL to retrieve the page thumbnail.
        /// </summary>
        [JsonPropertyName("thumbUrl")]
        public string ThumbUrl { get; set; }

    }
}