using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// View options
    /// </summary>  
    public class ViewOptions
    {
        /// <summary>
        /// View format (HTML, PNG, JPG, or PDF) Default value is HTML.
        /// </summary>
        public ViewFormat ViewFormat { get; set; }

        /// <summary>
        /// File info
        /// </summary>  
        public FileInfo FileInfo { get; set; }

        /// <summary>
        /// The output path Default value is 'viewer\\{input file path}_{file extension}\\'
        /// </summary>  
        public string OutputPath { get; set; }

        /// <summary>
        /// The path to directory containing custom fonts in storage
        /// </summary>  
        public string FontsPath { get; set; }

        /// <summary>
        /// Text watermark
        /// </summary>  
        public Watermark Watermark { get; set; }

        /// <summary>
        /// Rendering options
        /// </summary>  
        public RenderOptions RenderOptions { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ViewOptions {\n");
            sb.Append("  FileInfo: ").Append(this.FileInfo).Append("\n");
            sb.Append("  ViewFormat: ").Append(this.ViewFormat).Append("\n");
            sb.Append("  OutputPath: ").Append(this.OutputPath).Append("\n");
            sb.Append("  FontsPath: ").Append(this.FontsPath).Append("\n");
            sb.Append("  Watermark: ").Append(this.Watermark).Append("\n");
            sb.Append("  RenderOptions: ").Append(this.RenderOptions).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
