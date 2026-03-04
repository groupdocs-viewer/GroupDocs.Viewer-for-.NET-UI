using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace GroupDocs.Viewer.UI.Api.Filters
{
    /// <summary>
    /// An MVC exception filter that gracefully handles client-disconnection exceptions.
    /// When the UI cancels in-flight requests (e.g. navigating away while pages are loading),
    /// Kestrel throws during request body deserialization. Without this filter those exceptions
    /// propagate as unhandled errors. This filter intercepts them, logs at Information level,
    /// and returns HTTP 499 (Client Closed Request).
    /// </summary>
    internal class HandleClientDisconnectAttribute : ExceptionFilterAttribute
    {
        /// <inheritdoc />
        public override void OnException(ExceptionContext context)
        {
            if (IsClientDisconnect(context))
            {
                var logger = (ILogger<HandleClientDisconnectAttribute>)context.HttpContext
                    .RequestServices.GetService(typeof(ILogger<HandleClientDisconnectAttribute>));

                logger?.LogInformation(
                    "Request {Method} {Path} was cancelled by the client.",
                    context.HttpContext.Request.Method,
                    context.HttpContext.Request.Path);

                context.Result = new StatusCodeResult(499);
                context.ExceptionHandled = true;
            }
        }

        /// <summary>
        /// Determines whether the exception was caused by the client disconnecting.
        /// Detects two scenarios:
        /// <list type="bullet">
        ///   <item><description>
        ///     <see cref="OperationCanceledException"/> thrown when the request's
        ///     cancellation token is triggered (client abort during async work).
        ///   </description></item>
        ///   <item><description>
        ///     <see cref="BadHttpRequestException"/> with "Unexpected end of request content"
        ///     thrown by Kestrel when the client closes the connection mid-request body.
        ///   </description></item>
        /// </list>
        /// </summary>
        private static bool IsClientDisconnect(ExceptionContext context)
        {
            var exception = context.Exception;

            if (exception is OperationCanceledException &&
                context.HttpContext.RequestAborted.IsCancellationRequested)
            {
                return true;
            }

            if (exception is BadHttpRequestException badRequest &&
                (context.HttpContext.RequestAborted.IsCancellationRequested ||
                 badRequest.Message.Contains("Unexpected end of request content", StringComparison.Ordinal)))
            {
                return true;
            }

            return false;
        }
    }
}
