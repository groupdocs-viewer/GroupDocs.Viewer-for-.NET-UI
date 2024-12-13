using System.Text;

namespace GroupDocs.Viewer.UI.Core.Entities
{
    public class HtmlPage : Page
    {
        public static string DefaultExtension => ".html";

        public override string Extension => DefaultExtension;

        public override string ContentType => "text/html";
        
        public override string GetContent() =>
            Encoding.UTF8.GetString(PageData);

        public override void SetContent(string contents)
        {
            PageData = Encoding.UTF8.GetBytes(contents);
        }

        public HtmlPage(int pageNumber, byte[] data) 
            : base(pageNumber, data)
        {
        }
    }
}