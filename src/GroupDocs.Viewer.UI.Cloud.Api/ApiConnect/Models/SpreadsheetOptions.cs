using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Rendering options for Spreadsheet file formats. Spreadsheet file formats include files with extensions: .xls, .xlsx, .xlsm, .xlsb, .csv, .ods, .ots, .xltx, .xltm, .tsv 
    /// </summary>  
    public class SpreadsheetOptions
    {
        /// <summary>
        /// The text overflow mode for rendering spreadsheet documents into HTML
        /// </summary>
        public TextOverflowMode? TextOverflowMode { get; set; }

        /// <summary>
        /// Allows to enable worksheets pagination. By default one worksheet is rendered into one page.
        /// </summary>  
        public bool PaginateSheets { get; set; }

        /// <summary>
        /// The number of rows rendered into one page when PaginateSheets is enabled. Default value is 50.
        /// </summary>  
        public int? CountRowsPerPage { get; set; }

        /// <summary>
        /// The columns count to include into each page when splitting worksheet into pages.
        /// </summary>  
        public int? CountColumnsPerPage { get; set; }

        /// <summary>
        /// Indicates whether to render grid lines
        /// </summary>  
        public bool RenderGridLines { get; set; }

        /// <summary>
        /// By default empty rows are skipped. Enable this option in case you want to render empty rows.
        /// </summary>  
        public bool RenderEmptyRows { get; set; }

        /// <summary>
        /// By default empty columns are skipped. Enable this option in case you want to render empty columns.
        /// </summary>  
        public bool RenderEmptyColumns { get; set; }

        /// <summary>
        /// Enables rendering of hidden rows.
        /// </summary>  
        public bool RenderHiddenRows { get; set; }

        /// <summary>
        /// Enables rendering of hidden columns.
        /// </summary>  
        public bool RenderHiddenColumns { get; set; }

        /// <summary>
        /// Enables headings rendering.
        /// </summary>  
        public bool RenderHeadings { get; set; }

        /// <summary>
        /// Enables rendering worksheet(s) sections which is defined as print area. Renders each print area in a worksheet as a separate page.
        /// </summary>  
        public bool RenderPrintAreaOnly { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SpreadsheetOptions {\n");
            sb.Append("  PaginateSheets: ").Append(this.PaginateSheets).Append("\n");
            sb.Append("  CountRowsPerPage: ").Append(this.CountRowsPerPage).Append("\n");
            sb.Append("  CountColumnsPerPage: ").Append(this.CountColumnsPerPage).Append("\n");
            sb.Append("  RenderGridLines: ").Append(this.RenderGridLines).Append("\n");
            sb.Append("  RenderEmptyRows: ").Append(this.RenderEmptyRows).Append("\n");
            sb.Append("  RenderEmptyColumns: ").Append(this.RenderEmptyColumns).Append("\n");
            sb.Append("  RenderHiddenRows: ").Append(this.RenderHiddenRows).Append("\n");
            sb.Append("  RenderHiddenColumns: ").Append(this.RenderHiddenColumns).Append("\n");
            sb.Append("  RenderHeadings: ").Append(this.RenderHeadings).Append("\n");
            sb.Append("  RenderPrintAreaOnly: ").Append(this.RenderPrintAreaOnly).Append("\n");
            sb.Append("  TextOverflowMode: ").Append(this.TextOverflowMode).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
