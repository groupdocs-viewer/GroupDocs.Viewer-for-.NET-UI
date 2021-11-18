using System.Text;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models
{
    /// <summary>
    /// Provides options for rendering Mail storage (Lotus Notes, MBox) data files.
    /// </summary>  
    public class MailStorageOptions
    {
        /// <summary>
        /// The keywords used to filter messages.
        /// </summary>  
        public string TextFilter { get; set; }

        /// <summary>
        /// The email-address used to filter messages by sender or recipient.
        /// </summary>  
        public string AddressFilter { get; set; }

        /// <summary>
        /// The maximum number of messages or items for render. Default value is 0 - all messages will be rendered
        /// </summary>  
        public int? MaxItems { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class MailStorageOptions {\n");
            sb.Append("  TextFilter: ").Append(this.TextFilter).Append("\n");
            sb.Append("  AddressFilter: ").Append(this.AddressFilter).Append("\n");
            sb.Append("  MaxItems: ").Append(this.MaxItems).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}
