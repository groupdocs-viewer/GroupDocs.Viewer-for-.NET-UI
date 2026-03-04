using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class ListDirRequest
    {
        /// <summary>
        /// Directory path to list
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; set; } = string.Empty;
    }
}