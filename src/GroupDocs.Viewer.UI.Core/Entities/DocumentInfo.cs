using System.Collections.Generic;

namespace GroupDocs.Viewer.UI.Core.Entities
{
    public class DocumentInfo
    {
        public string FileType { get; set; }

        public bool PrintAllowed { get; set; }

        public IEnumerable<PageInfo> Pages { get; set; }
    }
}