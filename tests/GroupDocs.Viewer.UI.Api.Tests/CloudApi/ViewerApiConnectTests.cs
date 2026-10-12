using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.CloudApi
{
    /// <summary>
    /// The cloud API uploads the file from the configured storage to GroupDocs Cloud using its path from the client.
    /// </summary>
    public class ViewerApiConnectTests
    {
        private readonly CapturingHttpMessageHandler _handler = new CapturingHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}", Encoding.UTF8, "application/json") });

        private readonly ViewerApiConnect _apiConnect;

        public ViewerApiConnectTests()
        {
            var httpClient = new HttpClient(_handler) { BaseAddress = new Uri("https://api.groupdocs.cloud/v2.0/") };
            _apiConnect = new ViewerApiConnect(httpClient);
        }

        [Fact]
        public async Task CheckFileExistsAsync_WithParentSegments_ShouldThrowWithoutSendingRequest()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _apiConnect.CheckFileExistsAsync("../../auth/connect/token", "MyStorage"));

            Assert.Empty(_handler.Requests);
        }

        [Fact]
        public async Task UploadFileAsync_WithParentSegments_ShouldThrowWithoutSendingRequest()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _apiConnect.UploadFileAsync("../evil.docx", "MyStorage", new byte[] { 1 }));

            Assert.Empty(_handler.Requests);
        }

        [Fact]
        public async Task UploadFileAsync_WithQueryInName_ShouldKeepItInFileName()
        {
            await _apiConnect.UploadFileAsync("folder/a.docx?storageName=Other", "MyStorage", new byte[] { 1 });

            var request = Assert.Single(_handler.Requests);
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.Equal("/v2.0/viewer/storage/file/folder/a.docx%3FstorageName%3DOther", request.RequestUri!.AbsolutePath);
            Assert.Equal("?storageName=MyStorage", request.RequestUri.Query);
        }
    }
}
