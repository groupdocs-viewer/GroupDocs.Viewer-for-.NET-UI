using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api
{
    public class SearchTermResolver : ISearchTermResolver
    {
        public Task<string> ResolveSearchTermAsync(string filepath)
        {
            string searchTerm = string.Empty;
            return Task.FromResult(searchTerm);
        }
    }
}
