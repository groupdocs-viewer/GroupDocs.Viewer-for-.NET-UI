using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.SelfHost.Api
{
    /// <summary>
    /// Defines a contract for resolving the file type of a document.
    /// The default implementation uses the file extension. Override this to support
    /// files without extensions or to detect the file type by content inspection.
    /// </summary>
    /// <example>
    /// <code>
    /// public class ContentBasedFileTypeResolver : IFileTypeResolver
    /// {
    ///     public Task&lt;FileType&gt; ResolveFileTypeAsync(string filePath)
    ///     {
    ///         // Resolve file type by inspecting file content
    ///         return Task.FromResult(FileType.DOCX);
    ///     }
    /// }
    ///
    /// // Register in DI:
    /// services.AddSingleton&lt;IFileTypeResolver, ContentBasedFileTypeResolver&gt;();
    /// </code>
    /// </example>
    public interface IFileTypeResolver
    {
        /// <summary>
        /// Resolves the file type for the specified file path.
        /// </summary>
        /// <param name="filePath">The path or identifier of the file.</param>
        /// <returns>The resolved file type.</returns>
        Task<FileType> ResolveFileTypeAsync(string filePath);
    }
}