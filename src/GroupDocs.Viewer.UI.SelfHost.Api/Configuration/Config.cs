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
        internal readonly InternalCacheOptions InternalCacheOptions = InternalCacheOptions.CacheForFiveMinutes;

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
