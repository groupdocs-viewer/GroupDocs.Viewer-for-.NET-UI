using System;

namespace GroupDocs.Viewer.UI.Core.Entities
{
    public class JpgPage : Page
    {
        public static string Extension => ".jpeg";

        public override string GetContent() =>
            "data:image/jpeg;base64," + Convert.ToBase64String(Data);

        public JpgPage(int pageNumber, byte[] data) 
            : base(pageNumber, data)
        {
            
        }
    }
}