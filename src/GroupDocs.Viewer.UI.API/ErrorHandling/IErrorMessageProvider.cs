using System;
namespace GroupDocs.Viewer.UI.Api
{
    /// <summary>
    /// Defines a contract for providing user-friendly error messages based on exceptions.
    /// Implementations can customize how error messages are generated for different exception types.
    /// </summary>
    public interface IErrorMessageProvider
    {
        /// <summary>
        /// Retrieves an error message based on the given exception.  
        /// The returned message is intended to be displayed on the UI.  
        /// </summary>
        /// <param name="exception">The exception to process.</param>
        /// <param name="context">The exception message context.</param>
        /// <returns>A user-friendly error message.</returns>
        string GetErrorMessage(Exception exception, ErrorContext context);
    }
}
