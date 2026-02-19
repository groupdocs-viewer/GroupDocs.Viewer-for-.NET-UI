using System;
using GroupDocs.Viewer.UI.Core.Entities;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Entities
{
    public class PngPageTests
    {
        private readonly byte[] _sampleImageBytes = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A };

        [Fact]
        public void SetContent_WithDataUri_ShouldRoundtripWithGetContent()
        {
            var page = new PngPage(1, _sampleImageBytes);

            string content = page.GetContent();
            page.SetContent(content);

            Assert.Equal(content, page.GetContent());
        }

        [Fact]
        public void SetContent_WithDataUri_ShouldStoreRawBytes()
        {
            var page = new PngPage(1, _sampleImageBytes);

            string content = page.GetContent();
            page.SetContent(content);

            Assert.Equal(_sampleImageBytes, page.PageData);
        }

        [Fact]
        public void SetContent_WithBase64Only_ShouldStoreRawBytes()
        {
            var page = new PngPage(1, Array.Empty<byte>());
            string base64 = Convert.ToBase64String(_sampleImageBytes);

            page.SetContent(base64);

            Assert.Equal(_sampleImageBytes, page.PageData);
        }
    }
}
