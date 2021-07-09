using System.Collections.Generic;

namespace GroupDocs.Viewer.UI.Core.Entities
{
    public class DocumentInfo
    {
        public bool PrintAllowed { get; set; }

        public IEnumerable<PageInfo> Pages { get; set; }
    }
}