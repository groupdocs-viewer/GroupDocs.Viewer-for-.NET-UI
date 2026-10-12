using System;
using System.IO;

namespace GroupDocs.Viewer.UI.Api
{
    /// <summary>
    /// Provides a default implementation for retrieving error messages from exceptions.
    /// The returned message is displayed on the UI.
    /// </summary>
    /// <remarks>
    /// File system exceptions get generic messages because their messages contain absolute paths on the server.
    /// </remarks>
    public class ExceptionMessageProvider : IErrorMessageProvider
    {
        /// <inheritdoc />
        public string GetErrorMessage(Exception exception, ErrorContext context) =>
            exception switch
            {
                FileNotFoundException notFound when !string.IsNullOrEmpty(notFound.FileName) =>
                    $"File '{Path.GetFileName(notFound.FileName)}' was not found.",
                FileNotFoundException => "File was not found.",
                DirectoryNotFoundException => "File or folder was not found.",
                UnauthorizedAccessException => "Access to the file or folder is denied.",
                IOException => "File could not be accessed.",
                _ => exception.Message
            };
    }
}
