using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api.Tests
{
    /// <summary>
    /// Records requests instead of sending them, responds with 404 unless another response is provided.
    /// </summary>
    public class CapturingHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

        public CapturingHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage>? respond = null)
        {
            _respond = respond ?? (_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        }

        public List<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(Send(request, cancellationToken));

        // NOTE: some SDK calls are synchronous, e.g. BlobContainerClient.CreateIfNotExists()
        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            lock (Requests)
                Requests.Add(request);

            return _respond(request);
        }
    }
}
