namespace GroupDocs.Viewer.UI.Api.Models
{
    public class LoadDocumentPageResource
    {
        public string Guid { get; set; } = string.Empty;

        public string Password { get; set; }

        public int PageNumber { get; set; }

        public string ResourceName { get; set; }
    }
}