using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Core
{
    public interface IFileStorage
    {
        Task<IEnumerable<FileSystemEntry>> ListDirsAndFilesAsync(string dirPath, CancellationToken cancellationToken = default);

        Task<byte[]> ReadFileAsync(string filePath, CancellationToken cancellationToken = default);

        Task<string> WriteFileAsync(string fileName, byte[] bytes, bool rewrite, CancellationToken cancellationToken = default);
    }
}
