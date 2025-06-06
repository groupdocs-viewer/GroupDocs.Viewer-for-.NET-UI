﻿using GroupDocs.Viewer.Options;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Viewers.Extensions
{
    internal static class ViewOptionsExtensions
    {
        public static void CopyViewOptions(this HtmlViewOptions dst, HtmlViewOptions src)
        {
            dst.CopyBaseViewOptions(src);
            dst.CopyHtmlViewOptions(src);
        }

        public static void CopyViewOptions(this PdfViewOptions dst, PdfViewOptions src)
        {
            dst.CopyBaseViewOptions(src);
            dst.CopyPdfViewOptions(src);
        }

        public static void CopyViewOptions(this PngViewOptions dst, PngViewOptions src)
        {
            dst.CopyBaseViewOptions(src);
            dst.CopyPngViewOptions(src);
        }

        public static void CopyViewOptions(this JpgViewOptions dst, JpgViewOptions src)
        {
            dst.CopyBaseViewOptions(src);
            dst.CopyJpgViewOptions(src);
        }

        public static void CopyBaseViewOptions(this BaseViewOptions dst, BaseViewOptions src)
        {
            //dst.RemoveComments = src.RemoveComments;
            dst.RenderNotes = src.RenderNotes;
            dst.RenderHiddenPages = src.RenderHiddenPages;
            dst.DefaultFontName = src.DefaultFontName;
            dst.ArchiveOptions = src.ArchiveOptions;
            dst.CadOptions = src.CadOptions;
            dst.EmailOptions = src.EmailOptions;
            dst.OutlookOptions = src.OutlookOptions;
            dst.MailStorageOptions = src.MailStorageOptions;
            dst.PdfOptions = src.PdfOptions;
#if !CROSS_PLATFORM
            dst.ProjectManagementOptions = src.ProjectManagementOptions;
#endif
            dst.SpreadsheetOptions = src.SpreadsheetOptions;
            dst.WordProcessingOptions = src.WordProcessingOptions;
            dst.VisioRenderingOptions = src.VisioRenderingOptions;
            dst.TextOptions = src.TextOptions;
            dst.PresentationOptions = src.PresentationOptions;
            dst.WebDocumentOptions = src.WebDocumentOptions;
        }

        private static void CopyHtmlViewOptions(this HtmlViewOptions dst, HtmlViewOptions src)
        {
            dst.RenderResponsive = src.RenderResponsive;
            dst.Minify = src.Minify;
            dst.RenderToSinglePage = src.RenderToSinglePage;
            dst.ImageMaxWidth = src.ImageMaxWidth;
            dst.ImageMaxHeight = src.ImageMaxHeight;
            dst.ImageWidth = src.ImageWidth;
            dst.ImageHeight = src.ImageHeight;
            dst.ForPrinting = src.ForPrinting;
            dst.ExcludeFonts = src.ExcludeFonts;
            dst.FontsToExclude = src.FontsToExclude;
            dst.FontsToExclude = src.FontsToExclude;
        }

        private static void CopyPdfViewOptions(this PdfViewOptions dst, PdfViewOptions src)
        {
            dst.Security = src.Security;
            dst.ImageMaxWidth = src.ImageMaxWidth;
            dst.ImageMaxHeight = src.ImageMaxHeight;
            dst.ImageWidth = src.ImageWidth;
            dst.ImageHeight = src.ImageHeight;
        }

        private static void CopyPngViewOptions(this PngViewOptions dst, PngViewOptions src)
        {
            dst.ExtractText = src.ExtractText;
            dst.Width = src.Width;
            dst.Height = src.Height;
            dst.MaxWidth = src.MaxWidth;
            dst.MaxHeight = src.MaxHeight;
        }
        private static void CopyJpgViewOptions(this JpgViewOptions dst, JpgViewOptions src)
        {
            dst.Quality = src.Quality;
            dst.ExtractText = src.ExtractText;
            dst.Width = src.Width;
            dst.Height = src.Height;
            dst.MaxWidth = src.MaxWidth;
            dst.MaxHeight = src.MaxHeight;
        }
    }
}