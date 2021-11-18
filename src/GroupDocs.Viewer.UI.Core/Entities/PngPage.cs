using System;

namespace GroupDocs.Viewer.UI.Core.Entities
{
    public class PngPage : Page
    {
        public static string Extension => ".png";

        public override string GetContent() =>
            "data:image/png;base64," + Convert.ToBase64String(Data);

        public PngPage(int pageNumber, byte[] data) 
            : base(pageNumber, data)
        {
            
        }
    }
}