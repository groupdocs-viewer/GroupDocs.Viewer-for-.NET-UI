using System.IO;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace GroupDocs.Viewer.UI.Api
{
    public class FilePathFileNameResolver : IFileNameResolver
    {
        public Task<string> ResolveFileNameAsync(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            return Task.FromResult(fileName);
        }
    }
}