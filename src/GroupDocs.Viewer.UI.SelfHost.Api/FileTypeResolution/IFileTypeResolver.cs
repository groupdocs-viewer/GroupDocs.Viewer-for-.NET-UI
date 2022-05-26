using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.SelfHost.Api
{
    public interface IFileTypeResolver
    {
        Task<FileType> ResolveFileTypeAsync(string filePath);
    }
}