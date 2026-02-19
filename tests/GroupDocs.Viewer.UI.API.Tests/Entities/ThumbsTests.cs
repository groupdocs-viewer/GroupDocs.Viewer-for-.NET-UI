using GroupDocs.Viewer.UI.Core.Entities;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Entities
{
    public class ThumbsTests
    {
        [Fact]
        public void Indexer_Set_ShouldReplaceExistingElement()
        {
            var thumb1 = new TestThumb(1);
            var thumb2 = new TestThumb(2);
            var replacement = new TestThumb(3);

            var thumbs = new Thumbs(new Thumb[] { thumb1, thumb2 });

            thumbs[0] = replacement;

            Assert.Equal(2, thumbs.Count());
            Assert.Same(replacement, thumbs[0]);
            Assert.Same(thumb2, thumbs[1]);
        }

        [Fact]
        public void Indexer_Set_ShouldNotGrowCollection()
        {
            var thumb1 = new TestThumb(1);
            var thumbs = new Thumbs(new Thumb[] { thumb1 });

            thumbs[0] = new TestThumb(2);

            Assert.Single(thumbs);
        }

        private class TestThumb : Thumb
        {
            public TestThumb(int pageNumber) : base(pageNumber, new byte[] { 0 }) { }
            public override string Extension => ".png";
            public override string ContentType => "image/png";
        }
    }
}
