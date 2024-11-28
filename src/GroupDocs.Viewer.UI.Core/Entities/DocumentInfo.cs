using System.Collections.Generic;
using System.Linq;

namespace GroupDocs.Viewer.UI.Core.Entities
{
    public class DocumentInfo
    {
        public string FileType { get; set; }

        public bool PrintAllowed { get; set; }

        public IEnumerable<PageInfo> Pages { get; set; }

        public int TotalPagesCount => Pages.Count();
    }
}