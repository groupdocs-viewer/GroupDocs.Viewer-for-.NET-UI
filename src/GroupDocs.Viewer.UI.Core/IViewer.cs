using System.Threading;
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
        Task<DocumentInfo> GetDocumentInfoAsync(FileCredentials fileCredentials, CancellationToken cancellationToken = default);
        Task<Page> GetPageAsync(FileCredentials fileCredentials, int pageNumber, CancellationToken cancellationToken = default);
        Task<Thumb> GetThumbAsync(FileCredentials fileCredentials, int pageNumber, CancellationToken cancellationToken = default);
        Task<Pages> GetPagesAsync(FileCredentials fileCredentials, int[] pageNumbers, CancellationToken cancellationToken = default);
        Task<Thumbs> GetThumbsAsync(FileCredentials fileCredentials, int[] pageNumbers, CancellationToken cancellationToken = default);
        Task<byte[]> GetPdfAsync(FileCredentials fileCredentials, CancellationToken cancellationToken = default);
        Task<byte[]> GetPageResourceAsync(FileCredentials fileCredentials, int pageNumber, string resourceName, CancellationToken cancellationToken = default);
    }
}
