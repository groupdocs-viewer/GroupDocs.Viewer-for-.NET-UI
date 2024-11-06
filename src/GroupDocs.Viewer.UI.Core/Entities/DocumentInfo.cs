using System.Collections.Generic;

namespace GroupDocs.Viewer.UI.Core.Entities
{
    public class DocumentInfo
    {
        public bool PrintingAllowed { get; set; }

        public bool CreateThumbs { get; set; }

        public bool CreateThumbsFromFile { get; set; }

        public IEnumerable<PageInfo> Pages { get; set; }
    }
}