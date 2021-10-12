using GroupDocs.Viewer.UI.Core;

namespace GroupDocs.Viewer.UI.Cloud.Api.Configuration
{
    public class Config
    {
        internal string ApiEndpoint = "https://api.groupdocs.cloud/v2.0/";
        internal string ClientId = string.Empty;
        internal string ClientSecret = string.Empty;
        internal string StorageName = string.Empty;
        internal ViewerType ViewerType = ViewerType.HtmlWithEmbeddedResources;
        // internal readonly HtmlViewOptions HtmlViewOptions = HtmlViewOptions.ForEmbeddedResources();
        // internal readonly PngViewOptions PngViewOptions = new PngViewOptions();
        // internal readonly JpgViewOptions JpgViewOptions = new JpgViewOptions();
        // internal readonly PdfViewOptions PdfViewOptions = new PdfViewOptions();

        public Config SetApiEndpoint(string apiEndpoint)
        {
            ApiEndpoint = apiEndpoint;
            return this;
        }

        public Config SetClientId(string clientId)
        {
            ClientId = clientId;
            return this;
        }

        public Config SetClientSecret(string clientSecret)
        {
            ClientSecret = clientSecret;
            return this;
        }

        public Config SetStorageName(string storageName)
        {
            StorageName = storageName;
            return this;
        }

        public Config SetViewerType(ViewerType viewerType)
        {
            ViewerType = viewerType;
            return this;
        }

        // public Config ConfigureHtmlViewOptions(Action<HtmlViewOptions> setupOptions)
        // {
        //     setupOptions?.Invoke(HtmlViewOptions);
        //     return this;
        // }

        // public Config ConfigurePngViewOptions(Action<PngViewOptions> setupOptions)
        // {
        //     setupOptions?.Invoke(PngViewOptions);
        //     return this;
        // }
     
        // public Config ConfigureJpgViewOptions(Action<JpgViewOptions> setupOptions)
        // {
        //     setupOptions?.Invoke(JpgViewOptions);
        //     return this;
        // }

        // public Config ConfigurePdfViewOptions(Action<PdfViewOptions> setupOptions)
        // {
        //     setupOptions?.Invoke(PdfViewOptions);
        //     return this;
        // }
    }
}
