using System;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Core
{
    public interface IViewer
    {
        string PageExtension { get; }
        string ThumbExtension { get; }
        Page CreatePage(int pageNumber, byte[] data);
        Thumb CreateThumb(int pageNumber, byte[] data);
        Task<DocumentInfo> GetDocumentInfoAsync(FileCredentials fileCredentials);
        Task<Page> GetPageAsync(FileCredentials fileCredentials, int pageNumber);
        Task<Thumb> GetThumbAsync(FileCredentials fileCredentials, int pageNumber);
        Task<Pages> GetPagesAsync(FileCredentials fileCredentials, int[] pageNumbers);
        Task<Thumbs> GetThumbsAsync(FileCredentials fileCredentials, int[] pageNumbers);
        Task<byte[]> GetPdfAsync(FileCredentials fileCredentials);
        Task<byte[]> GetPageResourceAsync(FileCredentials fileCredentials, int pageNumber, string resourceName);
    }
}