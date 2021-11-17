using System.Collections.Generic;
using System.Text;

namespace GroupDocs.Viewer.UI.Core.Entities
{
    public class HtmlPage : Page
    {
        public static string Extension => ".html";

        public override string GetContent() =>
            Encoding.UTF8.GetString(Data);

        public HtmlPage(int pageNumber, byte[] data) 
            : base(pageNumber, data)
        {
        }
    }
}