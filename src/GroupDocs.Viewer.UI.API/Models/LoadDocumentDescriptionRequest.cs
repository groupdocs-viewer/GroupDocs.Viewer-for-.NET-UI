using GroupDocs.Viewer.UI.Api.DTO;
using System.Linq;
using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class LoadDocumentDescriptionRequest
    {
        /// <summary>
        /// File unique ID.
        /// </summary>
        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        /// <summary>
        /// The password to open a document.
        /// </summary>
        [JsonPropertyName("password")]
        public string Password { get; set; }

        internal FileEntry ToFileEntry()
        {
            string[] names = Guid.Split("/");

            return new FileEntry(fileName: names.Last(), folderName: names.First());
        }
    }
}