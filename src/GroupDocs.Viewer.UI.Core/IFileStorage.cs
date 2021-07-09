using System.Collections.Generic;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Core
{
    public interface IFileStorage
    {
        Task<IEnumerable<FileSystemEntry>> ListDirsAndFilesAsync(string dirPath);

        Task<byte[]> ReadFileAsync(string filePath);

        Task<string> WriteFileAsync(string fileName, byte[] bytes, bool rewrite);
    }
}