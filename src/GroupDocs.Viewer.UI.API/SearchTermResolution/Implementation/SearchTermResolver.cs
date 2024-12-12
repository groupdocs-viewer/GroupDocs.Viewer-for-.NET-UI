using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api
{
    public class SearchTermResolver : ISearchTermResolver
    {
        public Task<string> ResolveSearchTermAsync(string filepath)
        {
            string searchTerm = string.Empty;
            //string searchTerm = "background";
            return Task.FromResult(searchTerm);
        }
    }
}
