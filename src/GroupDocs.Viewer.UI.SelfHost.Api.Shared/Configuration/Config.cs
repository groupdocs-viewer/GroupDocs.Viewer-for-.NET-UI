using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.UI.Core;
using System;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Configuration
{
    public class Config
    {
        internal string LicensePath = string.Empty;

        internal ViewerType ViewerType = ViewerType.HtmlWithEmbeddedResources;

        internal readonly SpreadsheetOptions SpreadsheetOptions = CreateSpreadsheetOptions();

        internal readonly HtmlViewOptions HtmlViewOptions = CreateHtmlViewOptions();

        internal readonly PngViewOptions PngViewOptions = new PngViewOptions
        {
            SpreadsheetOptions = CreateSpreadsheetOptions()
        };

        internal readonly JpgViewOptions JpgViewOptions = new JpgViewOptions
        {
            SpreadsheetOptions = CreateSpreadsheetOptions()
        };

        internal readonly PdfViewOptions PdfViewOptions = new PdfViewOptions
        {
            SpreadsheetOptions = CreateSpreadsheetOptions()
        };

        internal readonly InternalCacheOptions InternalCacheOptions = InternalCacheOptions.CacheForFiveMinutes;

        private static SpreadsheetOptions CreateSpreadsheetOptions()
        {
            SpreadsheetOptions spreadsheetOptions = SpreadsheetOptions.ForOnePagePerSheet();
            spreadsheetOptions.RenderGridLines = true;
            spreadsheetOptions.RenderHeadings = true;

            return spreadsheetOptions;
        }

        private static HtmlViewOptions CreateHtmlViewOptions()
        {
            HtmlViewOptions htmlViewOptions = HtmlViewOptions.ForEmbeddedResources();
            htmlViewOptions.SpreadsheetOptions = CreateSpreadsheetOptions();

            return htmlViewOptions;
        }

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

        /// <summary>
        /// Call this method to configure internal objects caching.
        /// Internal caching makes objects available between requests to speed up rendering when document is rendered in chunks.
        /// Default cache entry lifetime is 5 minutes.
        /// Internal cache is based on MemoryCache (ConcurrentDictionary), so the object are stored in memory.
        /// </summary>
        /// <param name="setupOptions">Setup delegate.</param>
        /// <returns>This instance.</returns>
        public Config ConfigureInternalCaching(Action<InternalCacheOptions> setupOptions)
        {
            setupOptions?.Invoke(InternalCacheOptions);
            return this;
        }
    }
}
