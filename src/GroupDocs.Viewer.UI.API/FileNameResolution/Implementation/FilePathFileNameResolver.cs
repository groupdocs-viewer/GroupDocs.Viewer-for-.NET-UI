using System.IO;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api
{
    /// <summary>
    /// Resolves file names by extracting the filename from a file path.
    /// </summary>
    public class FilePathFileNameResolver : IFileNameResolver
    {
        /// <summary>
        /// Resolves the file name from the given file path.
        /// </summary>
        /// <param name="file">The full file path from which to extract the filename.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the extracted filename.</returns>
        public Task<string> ResolveFileNameAsync(string file)
        {
            string fileName = Path.GetFileName(file);
            return Task.FromResult(fileName);
        }
    }
}