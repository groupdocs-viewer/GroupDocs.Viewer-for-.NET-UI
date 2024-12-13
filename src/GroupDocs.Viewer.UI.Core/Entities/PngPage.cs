using System;
using System.Text;

namespace GroupDocs.Viewer.UI.Core.Entities
{
    public class PngPage : Page
    {
        const string DATA_IMAGE = "data:image/png;base64,";

        public static string DefaultExtension => ".png";

        public override string Extension => DefaultExtension;

        public override string ContentType => "image/png";

        public override string GetContent() =>
            DATA_IMAGE + Convert.ToBase64String(PageData);

        public override void SetContent(string content)
        {
            this.PageData = content.StartsWith(DATA_IMAGE) 
                ? Encoding.UTF8.GetBytes(content) 
                : Encoding.UTF8.GetBytes(content.Substring(DATA_IMAGE.Length - 1));
        }

        public PngPage(int pageNumber, byte[] pageData) 
            : base(pageNumber, pageData)
        {
            
        }
    }
}