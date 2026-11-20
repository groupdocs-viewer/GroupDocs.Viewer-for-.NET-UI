using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.UI.SelfHost.Api.Viewers.Extensions;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.SelfHostApi
{
    /// <summary>
    /// The options set with <c>ConfigureHtmlViewOptions</c> and <c>ConfigurePdfViewOptions</c> are copied
    /// to the options of every render, so a setting that isn't copied has no effect.
    /// </summary>
    public class ViewOptionsExtensionsTests
    {
        [Fact]
        public void CopyViewOptions_ShouldCopyPdfOptimizationOptions()
        {
            var configured = new PdfViewOptions
            {
                PdfOptimizationOptions = new PdfOptimizationOptions { OptimizeSpreadsheets = true }
            };
            var render = new PdfViewOptions();

            render.CopyViewOptions(configured);

            Assert.Same(configured.PdfOptimizationOptions, render.PdfOptimizationOptions);
        }

        [Fact]
        public void CopyViewOptions_ShouldCopyFontAndSpreadsheetSettingsOfHtmlPages()
        {
            var configured = HtmlViewOptions.ForEmbeddedResources();
            configured.DefaultFontName = "DejaVu Sans";
            configured.ExcludeFonts = true;
            configured.SpreadsheetOptions = SpreadsheetOptions.ForSplitSheetIntoPages(100);
            var render = HtmlViewOptions.ForEmbeddedResources();

            render.CopyViewOptions(configured);

            Assert.Equal("DejaVu Sans", render.DefaultFontName);
            Assert.True(render.ExcludeFonts);
            Assert.Same(configured.SpreadsheetOptions, render.SpreadsheetOptions);
        }

        [Fact]
        public void CopyViewOptions_ShouldCopyPdfAndCadOptionsOfHtmlPages()
        {
            // Options added in GroupDocs.Viewer 26.10
            var configured = HtmlViewOptions.ForEmbeddedResources();
            configured.PdfOptions.FixThaiMarks = true;
            configured.CadOptions.EnableLayeredRendering = true;
            var render = HtmlViewOptions.ForEmbeddedResources();

            render.CopyViewOptions(configured);

            Assert.True(render.PdfOptions.FixThaiMarks);
            Assert.True(render.CadOptions.EnableLayeredRendering);
        }
    }
}
