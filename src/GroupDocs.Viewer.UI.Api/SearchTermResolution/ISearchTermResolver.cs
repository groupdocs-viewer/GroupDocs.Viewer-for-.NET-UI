using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api
{
    /// <summary>
    /// Defines a contract for providing search terms that will be automatically highlighted in the document when it is loaded.
    /// This feature works only when rendering to HTML.
    /// </summary>
    /// <example>
    /// The following example shows how to implement a custom search term resolver:
    /// <code>
    /// public class MySearchTermResolver : ISearchTermResolver
    /// {
    ///     public Task<string> ResolveSearchTermAsync(string file)
    ///     {
    ///         return Task.FromResult("review");
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface ISearchTermResolver
    {
        /// <summary>
        /// Resolves a search term for the specified file.
        /// The returned search term will be automatically highlighted in the document when it is loaded.
        /// </summary>
        /// <param name="file">The path or identifier of the file.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the search term to be highlighted.</returns>
        Task<string> ResolveSearchTermAsync(string file);
    }
}
