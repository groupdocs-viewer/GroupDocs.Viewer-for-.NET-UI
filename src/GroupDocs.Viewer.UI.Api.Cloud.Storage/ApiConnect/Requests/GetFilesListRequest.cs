namespace GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Requests
{
    /// <summary>
    /// Get files list request model.
    /// </summary>  
    public class GetFilesListRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetFilesListRequest"/> class.
        /// </summary>        
        public GetFilesListRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetFilesListRequest"/> class.
        /// </summary>
        /// <param name="path">Folder path e.g. &#39;/folder&#39;</param>
        /// <param name="storageName">Storage name</param>
        public GetFilesListRequest(string path, string storageName = null)
        {
            this.Path = path;
            this.StorageName = storageName;
        }

        /// <summary>
        /// Folder path e.g. '/folder'
        /// </summary>  
        public string Path { get; set; }

        /// <summary>
        /// Storage name
        /// </summary>  
        public string StorageName { get; set; }
    }
}