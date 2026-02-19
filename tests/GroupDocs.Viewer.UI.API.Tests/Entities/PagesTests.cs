using GroupDocs.Viewer.UI.Core.Entities;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Entities
{
    public class PagesTests
    {
        [Fact]
        public void Indexer_Set_ShouldReplaceExistingElement()
        {
            var page1 = new HtmlPage(1, new byte[] { 1 });
            var page2 = new HtmlPage(2, new byte[] { 2 });
            var replacement = new HtmlPage(3, new byte[] { 3 });

            var pages = new Pages(new Page[] { page1, page2 });

            pages[0] = replacement;

            Assert.Equal(2, pages.Count());
            Assert.Same(replacement, pages[0]);
            Assert.Same(page2, pages[1]);
        }

        [Fact]
        public void Indexer_Set_ShouldNotGrowCollection()
        {
            var page1 = new HtmlPage(1, new byte[] { 1 });
            var pages = new Pages(new Page[] { page1 });

            pages[0] = new HtmlPage(2, new byte[] { 2 });

            Assert.Single(pages);
        }
    }
}
