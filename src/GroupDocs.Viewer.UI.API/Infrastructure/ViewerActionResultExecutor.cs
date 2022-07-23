using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;

namespace GroupDocs.Viewer.UI.Api.Infrastructure
{
    internal class ViewerActionResultExecutor : ActionResult, IStatusCodeActionResult
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private static readonly string DefaultContentType = new MediaTypeHeaderValue("application/json")
        {
            Encoding = Encoding.UTF8
        }.ToString();

        public async Task ExecuteAsync(ActionContext context, ViewerActionResult result)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var response = context.HttpContext.Response;

            response.ContentType = DefaultContentType;

            if (result.StatusCode != null)
                response.StatusCode = result.StatusCode.Value;

            var value = result.Value;
            var objectType = value?.GetType() ?? typeof(object);

            // Keep this code in sync with SystemTextJsonOutputFormatter
            var responseStream = response.Body;

            try
            {
                await JsonSerializer.SerializeAsync(responseStream, value, objectType, _jsonSerializerOptions, context.HttpContext.RequestAborted);
                await responseStream.FlushAsync(context.HttpContext.RequestAborted);
            }
            catch (OperationCanceledException) when (context.HttpContext.RequestAborted.IsCancellationRequested) { }

        }

        public int? StatusCode { get; }
    }
}