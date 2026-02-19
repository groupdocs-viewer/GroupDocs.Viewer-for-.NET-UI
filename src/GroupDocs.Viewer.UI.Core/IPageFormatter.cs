using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Core
{
    /// <summary>
    /// Defines a contract for post-processing rendered pages before they are returned to the client.
    /// Use this to inject custom CSS, JavaScript, watermarks, or modify HTML content of rendered pages.
    /// </summary>
    /// <example>
    /// <code>
    /// public class WatermarkPageFormatter : IPageFormatter
    /// {
    ///     public Task&lt;Page&gt; FormatAsync(FileCredentials fileCredentials, Page page)
    ///     {
    ///         // Add a watermark to each rendered HTML page
    ///         var html = System.Text.Encoding.UTF8.GetString(page.PageData);
    ///         html = html.Replace("&lt;/body&gt;", "&lt;div class='watermark'&gt;CONFIDENTIAL&lt;/div&gt;&lt;/body&gt;");
    ///         return Task.FromResult(page.SetData(System.Text.Encoding.UTF8.GetBytes(html)));
    ///     }
    /// }
    ///
    /// // Register in DI:
    /// services.AddSingleton&lt;IPageFormatter, WatermarkPageFormatter&gt;();
    /// </code>
    /// </example>
    public interface IPageFormatter
    {
        /// <summary>
        /// Post-processes a rendered page before it is returned to the client.
        /// </summary>
        /// <param name="fileCredentials">The file credentials for the document being rendered.</param>
        /// <param name="page">The rendered page to format.</param>
        /// <returns>The formatted page.</returns>
        Task<Page> FormatAsync(FileCredentials fileCredentials, Page page);
    }
}