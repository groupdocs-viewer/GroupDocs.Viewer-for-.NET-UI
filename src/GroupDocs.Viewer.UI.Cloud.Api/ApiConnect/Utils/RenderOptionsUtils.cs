using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Utils
{
    public class RenderOptionsUtils
    {
        public static void CopyRenderOptions(RenderOptions dst, RenderOptions src)
        {
            dst.PagesToRender = src.PagesToRender;
            dst.PageRotations = src.PageRotations;
            dst.DefaultFontName = src.DefaultFontName;
            dst.DefaultEncoding = src.DefaultEncoding;
            dst.RenderComments = src.RenderComments;
            dst.RenderNotes = src.RenderNotes;
            dst.RenderHiddenPages = src.RenderHiddenPages;
            dst.SpreadsheetOptions = src.SpreadsheetOptions;
            dst.CadOptions = src.CadOptions;
            dst.EmailOptions = src.EmailOptions;
            dst.ProjectManagementOptions = src.ProjectManagementOptions;
            dst.PdfDocumentOptions = src.PdfDocumentOptions;
            dst.WordProcessingOptions = src.WordProcessingOptions;
            dst.OutlookOptions = src.OutlookOptions;
            dst.ArchiveOptions = src.ArchiveOptions;
            dst.TextOptions = src.TextOptions;
            dst.MailStorageOptions = src.MailStorageOptions;
            dst.VisioRenderingOptions = src.VisioRenderingOptions;
            dst.VisioRenderingOptions = src.VisioRenderingOptions;
        }
    }
}