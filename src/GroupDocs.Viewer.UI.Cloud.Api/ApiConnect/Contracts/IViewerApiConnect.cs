using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models;
using GroupDocs.Viewer.UI.Cloud.Api.Common;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts
{
    /// <summary>
    /// Viewer API
    /// </summary>
    internal interface IViewerApiConnect
    {
        /// <summary>
        /// Retrieves document info
        /// </summary>
        /// <param name="fileInfo">File info</param>
        /// <param name="viewOptions">View options</param>
        /// <returns>Document info</returns>
        Task<Result<DocumentInfo>> GetDocumentInfoAsync(FileInfo fileInfo, ViewOptions viewOptions);
        /// <summary>
        /// Creates and retrieves PDF file
        /// </summary>
        /// <param name="fileInfo">File info</param>
        /// <param name="viewOptions">view options</param>
        /// <returns>PDF file bytes</returns>
        Task<Result<byte[]>> GetPdfFileAsync(FileInfo fileInfo, ViewOptions viewOptions);
        /// <summary>
        /// Creates pages
        /// </summary>
        /// <param name="fileInfo">File info</param>
        /// <param name="pageNumbers">Pages to create</param>
        /// <param name="viewOptions">View options</param>
        /// <returns>View result</returns>
        Task<Result<ViewResult>> CreatePagesAsync(FileInfo fileInfo, int[] pageNumbers, ViewOptions viewOptions);
        /// <summary>
        /// Retrieves resource
        /// </summary>
        /// <param name="resource">The resource</param>
        /// <param name="storageName">Storage name</param>
        /// <returns>Resource bytes</returns>
        Task<Result<byte[]>> DownloadResourceAsync(Resource resource, string storageName);
        /// <summary>
        /// Checks if file exists 
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="storageName">Storage name</param>
        /// <returns><value>true</value> when file exists</returns>
        Task<Result<bool>> CheckFileExistsAsync(string filePath, string storageName);
        /// <summary>
        /// Uploads file to storage
        /// </summary>
        /// <param name="filePath">File path</param>
        /// <param name="storageName">Storage name</param>
        /// <param name="bytes">File bytes</param>
        /// <returns>Operation result</returns>
        Task<Result> UploadFileAsync(string filePath, string storageName, byte[] bytes);
        /// <summary>
        /// Deletes output from the output path
        /// </summary>
        /// <param name="fileInfo">The file info</param>
        /// <param name="outputPath">The path where the output was stored</param>
        /// <returns>Operation result</returns>
        Task<Result> DeleteView(FileInfo fileInfo, string outputPath);
    }
}