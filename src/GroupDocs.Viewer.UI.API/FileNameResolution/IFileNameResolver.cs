using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace GroupDocs.Viewer.UI.Api
{
    public interface IFileNameResolver
    {
        Task<string> ResolveFileNameAsync(string filePath);
    }
}