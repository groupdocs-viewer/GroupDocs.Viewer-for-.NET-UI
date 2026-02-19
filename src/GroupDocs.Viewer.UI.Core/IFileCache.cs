using System.Threading;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Core
{
    public interface IFileCache
    {
        TEntry TryGetValue<TEntry>(string cacheKey, string filePath);

        Task<TEntry> TryGetValueAsync<TEntry>(string cacheKey, string filePath, CancellationToken cancellationToken = default);

        void Set<TEntry>(string cacheKey, string filePath, TEntry entry);

        Task SetAsync<TEntry>(string cacheKey, string filePath, TEntry entry, CancellationToken cancellationToken = default);

        /// <summary>
        /// Removes all cache entries associated with the specified file path.
        /// </summary>
        /// <param name="filePath">The file path whose cache entries should be removed.</param>
        /// <param name="cancellationToken">A token to cancel the operation.</param>
        Task RemoveAsync(string filePath, CancellationToken cancellationToken = default);
    }
}
