using System;
using System.Text;

namespace GroupDocs.Viewer.UI.Core.Entities
{
    public class JpgPage : Page
    {
        public static string DefaultExtension => ".jpeg";

        const string DATA_IMAGE = "data:image/jpeg;base64,";

        public override string Extension => DefaultExtension;

        public override string ContentType => "image/jpeg";

        public override string GetContent()
        {
            return DATA_IMAGE + Convert.ToBase64String(PageData);
        }

        public override void SetContent(string content)
        {
            var base64 = content.StartsWith(DATA_IMAGE)
                ? content.Substring(DATA_IMAGE.Length)
                : content;

            this.PageData = Convert.FromBase64String(base64);
        }

        public JpgPage(int pageNumber, byte[] pageData) 
            : base(pageNumber, pageData)
        {
            
        }
    }
}