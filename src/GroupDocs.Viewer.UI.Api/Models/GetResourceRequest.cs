namespace GroupDocs.Viewer.UI.Api.Models
{
    public class GetResourceRequest
    {
        /// <summary>
        /// File unique ID.
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// The page number which the resource belongs to.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// The resource name e.g. "s.css".
        /// </summary>
        public string Resource { get; set; }
    }
}