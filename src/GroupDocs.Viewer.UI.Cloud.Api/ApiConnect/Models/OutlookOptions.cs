using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Provides options for rendering Outlook data files
    /// </summary>  
    public class OutlookOptions
    {
        /// <summary>
        /// The name of the folder (e.g. Inbox, Sent Item or Deleted Items) to render
        /// </summary>  
        public string Folder { get; set; }

        /// <summary>
        /// The keywords used to filter messages
        /// </summary>  
        public string TextFilter { get; set; }

        /// <summary>
        /// The email-address used to filter messages by sender or recipient
        /// </summary>  
        public string AddressFilter { get; set; }

        /// <summary>
        /// The maximum number of messages or items, that can be rendered from one folder
        /// </summary>  
        public int? MaxItemsInFolder { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class OutlookOptions {\n");
            sb.Append("  Folder: ").Append(this.Folder).Append("\n");
            sb.Append("  TextFilter: ").Append(this.TextFilter).Append("\n");
            sb.Append("  AddressFilter: ").Append(this.AddressFilter).Append("\n");
            sb.Append("  MaxItemsInFolder: ").Append(this.MaxItemsInFolder).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
