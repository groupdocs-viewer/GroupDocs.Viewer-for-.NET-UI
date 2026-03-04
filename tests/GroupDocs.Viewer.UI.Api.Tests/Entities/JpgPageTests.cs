using System;
using GroupDocs.Viewer.UI.Core.Entities;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Entities
{
    public class JpgPageTests
    {
        private readonly byte[] _sampleImageBytes = { 0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10 };

        [Fact]
        public void SetContent_WithDataUri_ShouldRoundtripWithGetContent()
        {
            var page = new JpgPage(1, _sampleImageBytes);

            string content = page.GetContent();
            page.SetContent(content);

            Assert.Equal(content, page.GetContent());
        }

        [Fact]
        public void SetContent_WithDataUri_ShouldStoreRawBytes()
        {
            var page = new JpgPage(1, _sampleImageBytes);

            string content = page.GetContent();
            page.SetContent(content);

            Assert.Equal(_sampleImageBytes, page.PageData);
        }

        [Fact]
        public void SetContent_WithBase64Only_ShouldStoreRawBytes()
        {
            var page = new JpgPage(1, Array.Empty<byte>());
            string base64 = Convert.ToBase64String(_sampleImageBytes);

            page.SetContent(base64);

            Assert.Equal(_sampleImageBytes, page.PageData);
        }
    }
}
