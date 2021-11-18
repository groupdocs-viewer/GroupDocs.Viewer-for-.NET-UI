using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Provides options for rendering word processing documents
    /// </summary>  
    public class WordProcessingOptions
    {
        /// <summary>
        /// Enables tracked changes (revisions) rendering
        /// </summary>  
        public bool RenderTrackedChanges { get; set; }

        /// <summary>
        /// Left page margin (for HTML rendering only)
        /// </summary>  
        public double? LeftMargin { get; set; }

        /// <summary>
        /// Right page margin (for HTML rendering only)
        /// </summary>  
        public double? RightMargin { get; set; }

        /// <summary>
        /// Top page margin (for HTML rendering only)
        /// </summary>  
        public double? TopMargin { get; set; }

        /// <summary>
        /// Bottom page margin (for HTML rendering only)
        /// </summary>  
        public double? BottomMargin { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class WordProcessingOptions {\n");
            sb.Append("  RenderTrackedChanges: ").Append(this.RenderTrackedChanges).Append("\n");
            sb.Append("  LeftMargin: ").Append(this.LeftMargin).Append("\n");
            sb.Append("  RightMargin: ").Append(this.RightMargin).Append("\n");
            sb.Append("  TopMargin: ").Append(this.TopMargin).Append("\n");
            sb.Append("  BottomMargin: ").Append(this.BottomMargin).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
