using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api
{
    /// <summary>
    /// Defines a contract for resolving file names in the GroupDocs.Viewer.UI application.
    /// This interface allows customizing how file names are resolved from file identifiers.
    /// </summary>
    /// <example>
    /// The following example demonstrates how to implement and use the IFileNameResolver:
    /// <code>
    /// public class CustomFileNameResolver : IFileNameResolver
    /// {
    ///     public async Task<string> ResolveFileNameAsync(string file)
    ///     {
    ///         // Example implementation that extracts filename from a path
    ///         return Path.GetFileName(file);
    ///     }
    /// }
    /// 
    /// // Usage in DI configuration:
    /// services.AddSingleton<IFileNameResolver, CustomFileNameResolver>();
    /// </code>
    /// </example>
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