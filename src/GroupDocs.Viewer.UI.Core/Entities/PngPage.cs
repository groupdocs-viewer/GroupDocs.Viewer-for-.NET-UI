using System;

namespace GroupDocs.Viewer.UI.Core.Entities
{
    public class PngPage : Page
    {
        public PngPage(int number, byte[] data) 
            : base(number, "data:image/png;base64," + Convert.ToBase64String(data))
        {

        }
    }
}