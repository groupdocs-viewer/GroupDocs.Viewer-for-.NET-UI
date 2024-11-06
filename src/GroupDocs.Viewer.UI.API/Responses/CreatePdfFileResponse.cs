using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Responses;

public class CreatePdfFileResponse
{
    [JsonPropertyName("downloadUrl")]
    public string DownloadUrl { get; set; }
}