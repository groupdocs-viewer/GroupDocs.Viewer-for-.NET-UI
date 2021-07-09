using GroupDocs.Viewer.UI.Core;

namespace GroupDocs.Viewer.UI.Configuration
{
    public class Config
    {
        internal string DefaultDocument { get; private set; } = string.Empty;
        internal int PreloadPageCount { get; private set; } = 3;
        internal bool PageSelector { get; private set; } = true;
        internal bool Thumbnails { get; private set; } = true;
        internal bool Download { get; private set; } = true;
        internal bool Upload { get; private set; } = true;
        internal bool Rewrite { get; private set; } = false;
        internal bool Print { get; private set; } = true;
        internal bool Browse { get; private set; } = true;
        internal bool Zoom { get; private set; } = true;
        internal bool Search { get; private set; } = true;
        internal bool PrintAllowed { get; private set; } = true;
        internal bool EnableRightClick { get; private set; } = true;
        internal bool HtmlMode { get; private set; } = true;
        //TODO: Not implemented
        internal bool Rotate = false;
        internal bool SaveRotateState  = false;

        public Config SetViewerType(ViewerType viewerType)
        {
            HtmlMode = viewerType == ViewerType.HtmlWithExternalResources ||
                       viewerType == ViewerType.HtmlWithEmbeddedResources;
            return this;
        }

        public Config SetPreloadPageCount(int countPages)
        {
            PreloadPageCount = countPages;
            return this;
        }

        public Config SetDefaultDocument(string filePath)
        {
            DefaultDocument = filePath;
            return this;
        }

        public Config HidePageSelectorControl()
        {
            PageSelector = false;
            return this;
        }

        public Config HideThumbnailsControl()
        {
            Thumbnails = false;
            return this;
        }

        public Config HideDownloadButton()
        {
            Download = false;
            return this;
        }

        public Config HideUploadButton()
        {
            Upload = false;
            return this;
        }

        public Config RewriteFilesOnUpload()
        {
            Rewrite = true;
            return this;
        }

        public Config HidePrintButton()
        {
            Print = false;
            PrintAllowed = false;
            return this;
        }

        public Config HideFileBrowserButton()
        {
            Browse = false;
            return this;
        }

        public Config HideZoomButton()
        {
            Zoom = false;
            return this;
        }

        public Config HideSearchControl()
        {
            Search = false;
            return this;
        }

        public Config HidePageRotationControl()
        {
            Rotate = false;
            return this;
        }

        public Config DisableRightClick()
        {
            EnableRightClick = false;
            return this;
        }
    }
}
