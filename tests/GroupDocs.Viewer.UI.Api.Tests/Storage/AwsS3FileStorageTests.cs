using System;
using System.Net.Http;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.S3;
using GroupDocs.Viewer.UI.Api.AwsS3.Storage;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Storage
{
    public class AwsS3FileStorageTests
    {
        private readonly CapturingHttpMessageHandler _handler = new CapturingHttpMessageHandler();
        private readonly AwsS3FileStorage _storage;

        public AwsS3FileStorageTests()
        {
            _storage = new AwsS3FileStorage(new AwsS3Options
            {
                Region = "us-east-1",
                BucketName = "viewer",
                AccessKey = "AKIAEXAMPLE",
                SecretKey = "secret",
                S3Config = new AmazonS3Config
                {
                    HttpClientFactory = new CapturingHttpClientFactory(_handler),
                    MaxErrorRetry = 0
                }
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
        public async Task ReadFileAsync_WithNestedPath_ShouldRequestObjectFromBucket()
        {
            // The handler responds with 404 so reading fails, only the request matters here
            await Assert.ThrowsAnyAsync<Exception>(() => _storage.ReadFileAsync("folder/a.docx"));

            var request = Assert.Single(_handler.Requests);
            Assert.Equal("viewer.s3.us-east-1.amazonaws.com", request.RequestUri!.Host);
            Assert.Equal("/folder/a.docx", request.RequestUri.AbsolutePath);
        }

        private class CapturingHttpClientFactory : HttpClientFactory
        {
            private readonly HttpMessageHandler _handler;

            public CapturingHttpClientFactory(HttpMessageHandler handler) => _handler = handler;

            public override HttpClient CreateHttpClient(IClientConfig clientConfig) =>
                new HttpClient(_handler, disposeHandler: false);
        }
    }
}
