using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models;

public class CreatePdfFileRequest
{
    /// <summary>
    /// Unique file ID.
    /// </summary>
    [JsonPropertyName("guid")]
    public string Guid { get; set; }

    /// <summary>
    /// Password to open the document.
    /// </summary>
    [JsonPropertyName("password")]
    public string Password { get; set; }
}