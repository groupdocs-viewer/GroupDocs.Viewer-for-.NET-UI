using GroupDocs.Viewer.UI.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api.Shared.Controllers
{
    public interface IViewerService
    {
        Task<List<FileSystemItem>> ListDirAsync(string path);
        Task<UploadFileResponse> UploadFileAsync(string fileName, byte[] fileData, bool rewrite);
        Task<ViewDataResponse> GetViewerDataAsync(ViewDataRequest request);
        Task<List<PageData>> CreatePagesAsync(CreatePagesRequest request);
        Task<CreatePdfResponse> CreatePdfAsync(CreatePdfRequest request);
        Task<byte[]> GetPageAsync(GetPageRequest request);
        Task<byte[]> GetThumbAsync(GetThumbRequest request);
        Task<byte[]> GetPdfAsync(GetPdfRequest request);
        Task<byte[]> GetResourceAsync(GetResourceRequest request);
    }
}
