using GroupDocs.Viewer.UI.Core.Configuration;

namespace GroupDocs.Viewer.UI.Core
{
    public static class ViewerTypeExtensions
    {
        public static RenderingMode ToRenderingMode(this ViewerType viewerType)
        {
            if(viewerType == ViewerType.HtmlWithExternalResources || viewerType == ViewerType.HtmlWithEmbeddedResources)
                return RenderingMode.Html;

            return RenderingMode.Image;
        } 
    }
}