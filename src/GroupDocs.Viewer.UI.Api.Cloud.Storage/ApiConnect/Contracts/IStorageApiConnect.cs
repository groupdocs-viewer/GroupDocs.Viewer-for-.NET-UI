using GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Requests;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Responses;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.Common;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Contracts
{
    /// <summary>
    /// Viewer API
    /// </summary>
    public interface IStorageApiConnect
    {
        Task<Result<bool>> CheckObjectExistsAsync(ObjectExistRequest request);

        Task<Result<FilesList>> GetFilesList(GetFilesListRequest request);

        Task<Result<byte[]>> DownloadFile(DownloadFileRequest request);

        Task<Result> UploadFile(UploadFileRequest request);
    }
}