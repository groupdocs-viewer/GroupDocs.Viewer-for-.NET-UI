using System;
using GroupDocs.Viewer.UI.Core;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Storage
{
    public class StoragePathTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("a.docx")]
        [InlineData("folder/a.docx")]
        [InlineData("folder/sub/a.docx")]
        [InlineData("./a.docx")]
        [InlineData(".hidden")]
        [InlineData("a..b.docx")]
        [InlineData("folder/..docx")]
        [InlineData("...")]
        public void EnsureNoParentSegments_WithPathInsideStorage_ShouldReturnPath(string path)
        {
            Assert.Equal(path, StoragePath.EnsureNoParentSegments(path));
        }

        [Theory]
        [InlineData("..")]
        [InlineData("../a.docx")]
        [InlineData("folder/..")]
        [InlineData("folder/../../other/a.docx")]
        [InlineData(@"..\other\a.docx")]
        [InlineData(@"folder\..\..\a.docx")]
        public void EnsureNoParentSegments_WithParentSegments_ShouldThrow(string path)
        {
            Assert.Throws<UnauthorizedAccessException>(() => StoragePath.EnsureNoParentSegments(path));
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("a.docx", "a.docx")]
        [InlineData("folder/a b.docx", "folder/a%20b.docx")]
        [InlineData("a.docx?storageName=other", "a.docx%3FstorageName%3Dother")]
        [InlineData("a#1.docx", "a%231.docx")]
        [InlineData("100%.docx", "100%25.docx")]
        public void EscapeUrlPath_ShouldEscapeEachSegment(string path, string expected)
        {
            Assert.Equal(expected, StoragePath.EscapeUrlPath(path));
        }
    }
}
