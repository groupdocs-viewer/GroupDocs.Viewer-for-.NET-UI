using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api
{
    public class SearchTermResolver : ISearchTermResolver
    {
        public Task<string> ResolveSearchTermAsync(string file)
        {
            string searchTerm = string.Empty;
            //string searchTerm = "review";
            return Task.FromResult(searchTerm);
        }
    }
}
