using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Rendering options
    /// </summary>  
    public class RenderOptions
    {
        /// <summary>
        /// Pages list to render. Ignored, if StartPageNumber and CountPagesToRender are provided
        /// </summary>  
        [JsonProperty]
        internal List<int> PagesToRender { get; set; }

        /// <summary>
        /// Page rotations
        /// </summary>  
        [JsonProperty]
        internal List<PageRotation> PageRotations { get; set; }

        /// <summary>
        /// Default font name may be specified in following cases: - You want to generally specify the default font to fall back on, if particular font   in the document cannot be found during rendering. - Your document uses fonts, that contain non-English characters and you want to make sure   any missing font is replaced with one that has the same character set available.
        /// </summary>  
        [JsonProperty]
        public string DefaultFontName { get; set; }

        /// <summary>
        /// Default encoding for the plain text files such as .csv, .txt and .eml files when encoding is not specified in header
        /// </summary>  
        [JsonProperty]
        public string DefaultEncoding { get; set; }

        /// <summary>
        /// When enabled comments will be rendered to the output
        /// </summary>  
        [JsonProperty]
        public bool RenderComments { get; set; }

        /// <summary>
        /// When enabled notes will be rendered to the output
        /// </summary>  
        [JsonProperty]
        public bool RenderNotes { get; set; }

        /// <summary>
        /// When enabled hidden pages, sheets or slides will be rendered to the output
        /// </summary>  
        [JsonProperty]
        public bool RenderHiddenPages { get; set; }

        /// <summary>
        /// Rendering options for Spreadsheet source file formats Spreadsheet file formats include files with extensions: .xls, .xlsx, .xlsm, .xlsb, .csv, .ods, .ots, .xltx, .xltm, .tsv 
        /// </summary>  
        [JsonProperty]
        public SpreadsheetOptions SpreadsheetOptions { get; set; }

        /// <summary>
        /// Rendering options for CAD source file formats CAD file formats include files with extensions: .dwg, .dxf, .dgn, .ifc, .stl
        /// </summary>  
        [JsonProperty]
        public CadOptions CadOptions { get; set; }

        /// <summary>
        /// Rendering options for Email source file formats Email file formats include files with extensions: .msg, .eml, .emlx, .ifc, .stl
        /// </summary>  
        [JsonProperty]
        public EmailOptions EmailOptions { get; set; }

        /// <summary>
        /// Rendering options for MS Project source file formats Project file formats include files with extensions: .mpt, .mpp
        /// </summary>  
        [JsonProperty]
        public ProjectManagementOptions ProjectManagementOptions { get; set; }

        /// <summary>
        /// Rendering options for PDF source file formats
        /// </summary>  
        [JsonProperty]
        public PdfDocumentOptions PdfDocumentOptions { get; set; }

        /// <summary>
        /// Rendering options for WordProcessing source file formats
        /// </summary>  
        [JsonProperty]
        public WordProcessingOptions WordProcessingOptions { get; set; }

        /// <summary>
        /// Rendering options for Outlook source file formats
        /// </summary>  
        [JsonProperty]
        public OutlookOptions OutlookOptions { get; set; }

        /// <summary>
        /// Rendering options for Archive source file formats
        /// </summary>  
        [JsonProperty]
        public ArchiveOptions ArchiveOptions { get; set; }

        /// <summary>
        /// Rendering options for Text source file formats
        /// </summary>  
        [JsonProperty]
        public TextOptions TextOptions { get; set; }

        /// <summary>
        /// Rendering options for Mail storage (Lotus Notes, MBox) data files.
        /// </summary>  
        [JsonProperty]
        public MailStorageOptions MailStorageOptions { get; set; }

        /// <summary>
        /// Rendering options for Visio source file formats
        /// </summary>  
        [JsonProperty]
        public VisioRenderingOptions VisioRenderingOptions { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class RenderOptions {\n");
            sb.Append("  PagesToRender: ").Append(this.PagesToRender).Append("\n");
            sb.Append("  PageRotations: ").Append(this.PageRotations).Append("\n");
            sb.Append("  DefaultFontName: ").Append(this.DefaultFontName).Append("\n");
            sb.Append("  DefaultEncoding: ").Append(this.DefaultEncoding).Append("\n");
            sb.Append("  RenderComments: ").Append(this.RenderComments).Append("\n");
            sb.Append("  RenderNotes: ").Append(this.RenderNotes).Append("\n");
            sb.Append("  RenderHiddenPages: ").Append(this.RenderHiddenPages).Append("\n");
            sb.Append("  SpreadsheetOptions: ").Append(this.SpreadsheetOptions).Append("\n");
            sb.Append("  CadOptions: ").Append(this.CadOptions).Append("\n");
            sb.Append("  EmailOptions: ").Append(this.EmailOptions).Append("\n");
            sb.Append("  ProjectManagementOptions: ").Append(this.ProjectManagementOptions).Append("\n");
            sb.Append("  PdfDocumentOptions: ").Append(this.PdfDocumentOptions).Append("\n");
            sb.Append("  WordProcessingOptions: ").Append(this.WordProcessingOptions).Append("\n");
            sb.Append("  OutlookOptions: ").Append(this.OutlookOptions).Append("\n");
            sb.Append("  ArchiveOptions: ").Append(this.ArchiveOptions).Append("\n");
            sb.Append("  TextOptions: ").Append(this.TextOptions).Append("\n");
            sb.Append("  MailStorageOptions: ").Append(this.MailStorageOptions).Append("\n");
            sb.Append("  VisioRenderingOptions: ").Append(this.VisioRenderingOptions).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
