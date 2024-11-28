using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace GroupDocs.Viewer.UI.Api
{
    public interface IFileNameResolver
    {
        /// <summary>
        /// Returns actual filename with extension based on file path or file ID
        /// </summary>
        /// <param name="file">File identifier</param>
        /// <returns>Actual file name with extension</returns>
        Task<string> ResolveFileNameAsync(string file);
    }
}