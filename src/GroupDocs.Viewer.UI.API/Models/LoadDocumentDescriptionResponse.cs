using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class LoadDocumentDescriptionResponse
    {
        [JsonPropertyName("guid")]
        public string Guid { get; set; }

        [JsonPropertyName("printAllowed")]
        public bool PrintAllowed { get; set; }
        
        [JsonPropertyName("pages")]
        public List<PageDescription> Pages { get; set; }
    }
}