using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Core
{
    public interface IViewer
    {
        string PageExtension { get; }
        Page CreatePage(int pageNumber, byte[] data);
        Task<Page> GetPageAsync(string filePath, string password, int pageNumber);
        Task<Pages> GetPagesAsync(string filePath, string password, int[] pageNumbers);
        Task<DocumentInfo> GetDocumentInfoAsync(string filePath, string password);
        Task<byte[]> GetPdfAsync(string filePath, string password);
        Task<byte[]> GetPageResourceAsync(string filePath, string password, int pageNumber, string resourceName);
    }
}