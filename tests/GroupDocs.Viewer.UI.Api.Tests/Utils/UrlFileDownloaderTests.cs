using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Utils;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Utils
{
    public class UrlFileDownloaderTests
    {
        [Theory]
        [InlineData("0.0.0.0")]
        [InlineData("10.0.0.1")]
        [InlineData("100.64.0.1")]
        [InlineData("127.0.0.1")]
        [InlineData("127.255.255.254")]
        [InlineData("169.254.169.254")]
        [InlineData("172.16.0.1")]
        [InlineData("172.31.255.255")]
        [InlineData("192.0.0.1")]
        [InlineData("192.0.2.1")]
        [InlineData("192.168.1.1")]
        [InlineData("198.18.0.1")]
        [InlineData("224.0.0.1")]
        [InlineData("255.255.255.255")]
        [InlineData("::")]
        [InlineData("::1")]
        [InlineData("fc00::1")]
        [InlineData("fd12:3456::1")]
        [InlineData("fe80::1")]
        [InlineData("ff02::1")]
        [InlineData("2001:db8::1")]
        [InlineData("::ffff:127.0.0.1")]
        [InlineData("::ffff:169.254.169.254")]
        [InlineData("64:ff9b::a00:1")] // NAT64 for 10.0.0.1
        [InlineData("2002:a00:1::1")] // 6to4 for 10.0.0.1
        public void IsPublicAddress_WithInternalAddress_ShouldReturnFalse(string address)
        {
            Assert.False(UrlFileDownloader.IsPublicAddress(IPAddress.Parse(address)));
        }

        [Theory]
        [InlineData("8.8.8.8")]
        [InlineData("1.1.1.1")]
        [InlineData("100.128.0.1")] // next to 100.64.0.0/10
        [InlineData("172.32.0.1")] // next to 172.16.0.0/12
        [InlineData("192.169.0.1")]
        [InlineData("2001:4860:4860::8888")]
        [InlineData("2606:4700:4700::1111")]
        [InlineData("::ffff:8.8.8.8")]
        [InlineData("64:ff9b::808:808")] // NAT64 for 8.8.8.8
        public void IsPublicAddress_WithPublicAddress_ShouldReturnTrue(string address)
        {
            Assert.True(UrlFileDownloader.IsPublicAddress(IPAddress.Parse(address)));
        }

        [Theory]
        [InlineData("http://127.0.0.1/file.docx")]
        [InlineData("http://localhost:5000/file.docx")]
        [InlineData("http://[::1]/file.docx")]
        [InlineData("http://169.254.169.254/latest/meta-data/")]
        [InlineData("https://10.0.0.1/file.docx")]
        [InlineData("http://0.0.0.0/file.docx")]
        [InlineData("http://2130706433/file.docx")] // 127.0.0.1 written as a number
        public async Task DownloadAsync_WithInternalAddress_ShouldThrow(string url)
        {
            var exception = await Assert.ThrowsAsync<HttpRequestException>(
                () => UrlFileDownloader.DownloadAsync(url, null, CancellationToken.None));

            Assert.Contains("internal network addresses is not allowed", exception.Message);
        }

        [Theory]
        [InlineData("file:///C:/Windows/win.ini")]
        [InlineData("file:///etc/passwd")]
        [InlineData("ftp://example.com/file.docx")]
        [InlineData("/file.docx")]
        [InlineData("not a url")]
        public async Task DownloadAsync_WithUnsupportedUrl_ShouldThrow(string url)
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => UrlFileDownloader.DownloadAsync(url, null, CancellationToken.None));
        }

        [Fact]
        public async Task ReadAllBytesAsync_WhenStreamIsLargerThanLimit_ShouldThrow()
        {
            using var stream = new MemoryStream(new byte[200_000]);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => UrlFileDownloader.ReadAllBytesAsync(stream, 150_000, CancellationToken.None));
        }

        [Theory]
        [InlineData(200_000L)]
        [InlineData(null)]
        public async Task ReadAllBytesAsync_WhenStreamFitsLimit_ShouldReturnAllBytes(long? maxFileSize)
        {
            var data = Enumerable.Range(0, 200_000).Select(i => (byte)i).ToArray();
            using var stream = new MemoryStream(data);

            var bytes = await UrlFileDownloader.ReadAllBytesAsync(stream, maxFileSize, CancellationToken.None);

            Assert.Equal(data, bytes);
        }
    }
}
