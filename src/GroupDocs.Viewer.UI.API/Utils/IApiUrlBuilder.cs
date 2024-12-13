namespace GroupDocs.Viewer.UI.Api.Utils
{
    public interface IApiUrlBuilder
    {
        string BuildPageUrl(string file, int page, string extension);
        string BuildThumbUrl(string file, int page, string extension);
        string BuildPdfUrl(string file);
        string BuildResourceUrl(string file, int page, string resource);
        string BuildResourceUrl(string file, string pageTemplate, string resourceTemplate);
    }
}