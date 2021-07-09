using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Core
{
    public interface IViewer
    {
        Task<Pages> RenderPagesAsync(string filePath, string password, int[] pageNumbers);
        Task<DocumentInfo> GetDocumentInfoAsync(string filePath, string password);
        Task<byte[]> CreatePdfAsync(string filePath, string password);
        Task<byte[]> GetPageResourceAsync(string filePath, string password, int pageNumber, string resourceName);
    }
}