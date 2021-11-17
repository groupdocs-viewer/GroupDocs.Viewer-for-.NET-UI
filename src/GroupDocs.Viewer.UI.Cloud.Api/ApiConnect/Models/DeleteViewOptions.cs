using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models 
{
    /// <summary>
    /// Delete view options
    /// </summary>  
    public class DeleteViewOptions 
    {                       
        /// <summary>
        /// File info
        /// </summary>  
        public FileInfo FileInfo { get; set; }

        /// <summary>
        /// The output path. Default value is "viewer"
        /// </summary>  
        public string OutputPath { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()  
        {
          var sb = new StringBuilder();
          sb.Append("class DeleteViewOptions {\n");
          sb.Append("  FileInfo: ").Append(this.FileInfo).Append("\n");
          sb.Append("  OutputPath: ").Append(this.OutputPath).Append("\n");
          sb.Append("}\n");
          return sb.ToString();
        }
    }
}
