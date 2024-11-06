namespace GroupDocs.Viewer.UI.Api.Configuration;

public class Options
{
    public string ApiPath { get; set; } = "/viewer-api";
    public string ApiEndpoint { get; set; } = string.Empty;
    public string ConfigEndpoint { get; set; } = string.Empty;
    public string CloseViewerUrl { get; set; } = "/";
}