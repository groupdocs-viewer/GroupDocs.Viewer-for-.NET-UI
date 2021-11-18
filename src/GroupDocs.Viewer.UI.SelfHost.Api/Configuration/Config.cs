using System;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.UI.Core;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Configuration
{
    public class Config
    {
        internal string LicensePath = string.Empty;
        internal ViewerType ViewerType = ViewerType.HtmlWithEmbeddedResources;
        internal readonly HtmlViewOptions HtmlViewOptions = HtmlViewOptions.ForEmbeddedResources();
        internal readonly PngViewOptions PngViewOptions = new PngViewOptions();
        internal readonly JpgViewOptions JpgViewOptions = new JpgViewOptions();
        internal readonly PdfViewOptions PdfViewOptions = new PdfViewOptions();

        public Config SetLicensePath(string licensePath)
        {
            LicensePath = licensePath;
            return this;
        }

        public Config SetViewerType(ViewerType viewerType)
        {
            ViewerType = viewerType;
            return this;
        }

        public Config ConfigureHtmlViewOptions(Action<HtmlViewOptions> setupOptions)
        {
            setupOptions?.Invoke(HtmlViewOptions);
            return this;
        }

        public Config ConfigurePngViewOptions(Action<PngViewOptions> setupOptions)
        {
            setupOptions?.Invoke(PngViewOptions);
            return this;
        }
     
        public Config ConfigureJpgViewOptions(Action<JpgViewOptions> setupOptions)
        {
            setupOptions?.Invoke(JpgViewOptions);
            return this;
        }

        public Config ConfigurePdfViewOptions(Action<PdfViewOptions> setupOptions)
        {
            setupOptions?.Invoke(PdfViewOptions);
            return this;
        }
    }
}
