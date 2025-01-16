using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api
{
    public interface ISearchTermResolver
    {
        Task<string> ResolveSearchTermAsync(string filepath);
    }
}
