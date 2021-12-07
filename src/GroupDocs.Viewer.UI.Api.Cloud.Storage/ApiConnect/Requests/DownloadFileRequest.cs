namespace GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Requests
{
    /// <summary>
    /// Request model for download file operation.
    /// </summary>  
    public class DownloadFileRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadFileRequest"/> class.
        /// </summary>        
        public DownloadFileRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DownloadFileRequest"/> class.
        /// </summary>
        /// <param name="path">File path e.g. '/folder/file.ext'</param>
        /// <param name="storageName">Storage name</param>
        public DownloadFileRequest(string path, string storageName = null)
        {
            this.Path = path;
            this.StorageName = storageName;
        }

        /// <summary>
        /// File path e.g. '/folder/file.ext'
        /// </summary>  
        public string Path { get; set; }

        /// <summary>
        /// Storage name
        /// </summary>  
        public string StorageName { get; set; }
    }
}