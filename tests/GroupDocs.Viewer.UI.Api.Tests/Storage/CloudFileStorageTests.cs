using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Cloud.Storage;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect;
using Xunit;
using CloudStorageConfig = GroupDocs.Viewer.UI.Api.Cloud.Storage.Configuration.Config;

namespace GroupDocs.Viewer.UI.Api.Tests.Storage
{
    /// <summary>
    /// File paths are part of the GroupDocs Cloud API URLs, e.g. "viewer/storage/file/{path}?storageName={storage}".
    /// </summary>
    public class CloudFileStorageTests
    {
        private readonly CapturingHttpMessageHandler _handler = new CapturingHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(new byte[] { 1, 2, 3 }) });

        private readonly CloudFileStorage _storage;

        public CloudFileStorageTests()
        {
            var httpClient = new HttpClient(_handler) { BaseAddress = new Uri("https://api.groupdocs.cloud/v2.0/") };
            var config = new CloudStorageConfig().SetStorageName("MyStorage");

            _storage = new CloudFileStorage(MSOptions.Create(config), new StorageApiConnect(httpClient));
        }

        [Theory]
        [InlineData("../../auth/connect/token")]
        [InlineData(@"..\..\auth\connect\token")]
        [InlineData("folder/../../viewer/storage/file/secret.docx")]
        public async Task ReadFileAsync_WithParentSegments_ShouldThrowWithoutSendingRequest(string filePath)
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _storage.ReadFileAsync(filePath));

            Assert.Empty(_handler.Requests);
        }

        [Fact]
        public async Task ListDirsAndFilesAsync_WithParentSegments_ShouldThrowWithoutSendingRequest()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _storage.ListDirsAndFilesAsync("folder/../.."));

            Assert.Empty(_handler.Requests);
        }

        [Fact]
        public async Task WriteFileAsync_WithParentSegments_ShouldThrowWithoutSendingRequest()
        {
            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _storage.WriteFileAsync("../evil.txt", new byte[] { 1 }, rewrite: true));

            Assert.Empty(_handler.Requests);
        }

        [Fact]
        public async Task ReadFileAsync_WithQueryAndFragmentInName_ShouldKeepThemInFileName()
        {
            var bytes = await _storage.ReadFileAsync("folder/a b.docx?storageName=Other#x");

            var request = Assert.Single(_handler.Requests);
            Assert.Equal(new byte[] { 1, 2, 3 }, bytes);
            Assert.Equal("/v2.0/viewer/storage/file/folder/a%20b.docx%3FstorageName%3DOther%23x", request.RequestUri!.AbsolutePath);
            Assert.Equal("?storageName=MyStorage", request.RequestUri.Query);
        }
    }
}
