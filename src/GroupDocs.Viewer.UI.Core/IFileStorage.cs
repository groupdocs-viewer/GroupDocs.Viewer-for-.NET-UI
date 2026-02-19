using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Core
{
    public interface IFileStorage
    {
        Task<IEnumerable<FileSystemEntry>> ListDirsAndFilesAsync(string dirPath, CancellationToken cancellationToken = default);

        Task<byte[]> ReadFileAsync(string filePath, CancellationToken cancellationToken = default);

        /// <summary>
        /// Reads the file as a stream. The caller is responsible for disposing the returned stream.
        /// </summary>
        Task<Stream> ReadFileStreamAsync(string filePath, CancellationToken cancellationToken = default);

        Task<string> WriteFileAsync(string fileName, byte[] bytes, bool rewrite, CancellationToken cancellationToken = default);
    }
}
