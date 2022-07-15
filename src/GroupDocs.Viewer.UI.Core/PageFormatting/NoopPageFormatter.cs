using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Core.PageFormatting
{
    public class NoopPageFormatter : IPageFormatter
    {
        public Task<Page> FormatAsync(FileCredentials fileCredentials, Page page) => 
            Task.FromResult(page);
    }
}