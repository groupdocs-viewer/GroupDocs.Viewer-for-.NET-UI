using System;
using System.Linq;

namespace GroupDocs.Viewer.UI.Core
{
    /// <summary>
    /// Checks for file and folder paths received from the client. Used by the storages that pass
    /// the path to a blob, object or cloud storage rather than to the local file system.
    /// </summary>
    internal static class StoragePath
    {
        private static readonly char[] Separators = { '/', '\\' };

        /// <summary>
        /// Throws when the path contains ".." segments. Storage URLs resolve such segments, e.g. with Azure Blob Storage
        /// "../other/file.docx" reads "file.docx" from the "other" container of the same storage account.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">The path contains ".." segments.</exception>
        public static string EnsureNoParentSegments(string path)
        {
            if (path != null && path.Split(Separators).Any(segment => segment == ".."))
                throw new UnauthorizedAccessException(
                    "Access to the path is denied. The path must not contain '..' segments.");

            return path;
        }

        /// <summary>
        /// Escapes each segment of the path for a URL, so that characters such as '?' and '#' stay part of the name.
        /// </summary>
        public static string EscapeUrlPath(string path) =>
            string.Join("/", (path ?? string.Empty).Split('/').Select(Uri.EscapeDataString));
    }
}
