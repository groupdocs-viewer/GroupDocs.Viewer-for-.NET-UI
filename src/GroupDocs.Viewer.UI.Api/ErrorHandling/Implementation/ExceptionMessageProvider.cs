using System;

namespace GroupDocs.Viewer.UI.Api
{
    /// Provides a default implementation for retrieving error messages from exceptions.
    /// The returned message is displayed on the UI.
    /// </summary>
    public class ExceptionMessageProvider : IErrorMessageProvider
    {
        /// <inheritdoc />
        public string GetErrorMessage(Exception exception, ErrorContext context) => exception.Message;
    }
}
