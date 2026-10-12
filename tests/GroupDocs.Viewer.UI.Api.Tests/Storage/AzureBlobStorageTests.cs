using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using Azure.Storage.Blobs;
using GroupDocs.Viewer.UI.Api.AzureBlob.Storage;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Storage
{
    /// <summary>
    /// The Azure SDK resolves ".." in blob names, e.g. "../other/file.docx" reads "file.docx" from the "other" container.
    /// </summary>
    public class AzureBlobStorageTests
    {
        private readonly CapturingHttpMessageHandler _handler = new CapturingHttpMessageHandler(request =>
        {
            // The container is created on first use, "already exists" keeps the SDK from parsing the response
            if (request.Method == HttpMethod.Put && request.RequestUri!.Query.Contains("restype=container"))
            {
                var response = new HttpResponseMessage(HttpStatusCode.Conflict);
                response.Headers.Add("x-ms-error-code", "ContainerAlreadyExists");
                return response;
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        });

        private readonly AzureBlobStorage _storage;

        public AzureBlobStorageTests()
        {
            var clientOptions = new BlobClientOptions { Transport = new HttpClientTransport(new HttpClient(_handler)) };
            clientOptions.Retry.MaxRetries = 0;

            _storage = new AzureBlobStorage(new AzureBlobOptions
            {
                ConnectionString = "DefaultEndpointsProtocol=https;AccountName=account;AccountKey=a2V5;EndpointSuffix=core.windows.net",
                ContainerName = "viewer",
                ClientOptions = clientOptions
            });
        }

        [Theory]
        [InlineData("..")]
        [InlineData("../other/secret.txt")]
        [InlineData("folder/../../other/secret.txt")]
        public async Task ReadFileAsync_WithParentSegments_ShouldThrowWithoutSendingRequest(string filePath)
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _storage.ReadFileAsync(filePath));

            Assert.Empty(_handler.Requests);
        }

        [Fact]
        public async Task ReadFileStreamAsync_WithParentSegments_ShouldThrowWithoutSendingRequest()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _storage.ReadFileStreamAsync("../other/secret.txt"));

            Assert.Empty(_handler.Requests);
        }

        [Fact]
        public async Task WriteFileAsync_WithParentSegments_ShouldThrowWithoutSendingRequest()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _storage.WriteFileAsync("../other/evil.txt", new byte[] { 1 }, rewrite: true));

            Assert.Empty(_handler.Requests);
        }

        [Fact]
        public async Task ReadFileAsync_WithNestedPath_ShouldRequestBlobFromContainer()
        {
            // The handler responds with 404 so reading fails, only the request matters here
            await Assert.ThrowsAnyAsync<Exception>(() => _storage.ReadFileAsync("folder/a.docx"));

            Assert.Contains(_handler.Requests, request =>
                request.Method != HttpMethod.Put && request.RequestUri!.AbsolutePath == "/viewer/folder/a.docx");
            Assert.All(_handler.Requests, request => Assert.StartsWith("/viewer", request.RequestUri!.AbsolutePath));
        }
    }
}
