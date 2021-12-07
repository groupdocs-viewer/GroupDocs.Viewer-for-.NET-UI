namespace GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Requests
{
    /// <summary>
    /// Request model for download file operation.
    /// </summary>  
    public class UploadFileRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UploadFileRequest"/> class.
        /// </summary>        
        public UploadFileRequest()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadFileRequest"/> class.
        /// </summary>
        /// <param name="fileName">File path e.g. '/folder/file.ext'</param>
        /// <param name="data">File bytes</param>
        /// <param name="storageName">Storage name</param>
        public UploadFileRequest(string fileName, byte[] data, string storageName = null)
        {
            this.FileName = fileName;
            this.Data = data;
            this.StorageName = storageName;
        }

        public byte[] Data { get; set; }

        /// <summary>
        /// File path e.g. '/folder/file.ext'
        /// </summary>  
        public string FileName { get; set; }

        /// <summary>
        /// Storage name
        /// </summary>  
        public string StorageName { get; set; }
    }
}