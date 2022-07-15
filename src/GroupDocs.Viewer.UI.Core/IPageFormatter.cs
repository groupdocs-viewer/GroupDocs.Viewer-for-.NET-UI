using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Core
{
    public interface IPageFormatter
    {
        Task<Page> FormatAsync(FileCredentials fileCredentials, Page page);
    }
}