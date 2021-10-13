using System.Collections.Generic;
using System.Threading.Tasks;
using GroupDocs.Viewer.Cloud.Sdk.Model;
using GroupDocs.Viewer.UI.Cloud.Api.Common;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts
{
    internal interface IViewerApiConnect
    {
        Task<Result<DocumentInfo>> GetDocumentInfoAsync(ViewOptions.ViewFormatEnum viewFormat, FileInfo fileInfo);
        Task<Result<byte[]>> GetPdfFileAsync(FileInfo fileInfo);
        Task<Result<ViewResult>> CreatePagesAsync(int[] pageNumbers, ViewOptions.ViewFormatEnum viewFormat,  FileInfo fileInfo);
        Task<Result<byte[]>> DownloadResourceAsync(Resource resource, string storageName);
        Task<Result<bool>> CheckFileExistsAsync(string filePath, string storageName);
        Task<Result> UploadFileAsync(string filePath, string configStorageName, byte[] bytes);
    }
}