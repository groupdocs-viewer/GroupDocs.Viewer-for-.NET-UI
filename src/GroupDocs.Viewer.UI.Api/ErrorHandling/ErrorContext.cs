using System;
using Microsoft.AspNetCore.Http;

namespace GroupDocs.Viewer.UI.Api
{
    /// <summary>
    /// Represents contextual information for an error message,  
    /// including the API method where the exception occurred and the associated HTTP context.
    /// </summary>
    public record ErrorContext
    {
        /// <summary>
        /// The name of the API method where the exception occurred.
        /// </summary>
        public string ApiMethod { get; init; }

        /// <summary>
        /// The HTTP context associated with the request.
        /// </summary>
        public HttpContext HttpContext { get; init; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorMessageContext"/> record.
        /// </summary>
        /// <param name="apiMethod">The API method name.</param>
        /// <param name="httpContext">The associated HTTP context.</param>
        /// <exception cref="ArgumentNullException">Thrown if any parameter is null.</exception>
        public ErrorContext(string apiMethod, HttpContext httpContext)
        {
            ApiMethod = apiMethod ?? throw new ArgumentNullException(nameof(apiMethod));
            HttpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }
    }
}
