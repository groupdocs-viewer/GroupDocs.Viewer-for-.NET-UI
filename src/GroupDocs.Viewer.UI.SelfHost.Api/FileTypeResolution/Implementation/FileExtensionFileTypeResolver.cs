using System.IO;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace GroupDocs.Viewer.UI.SelfHost.Api
{
    public class FileExtensionFileTypeResolver : IFileTypeResolver
    {
        public Task<FileType> ResolveFileTypeAsync(string filePath)
        {
            string extension = Path.GetExtension(filePath);
            FileType fileType = FileType.FromExtension(extension);

            return Task.FromResult(fileType);
        }
    }
}