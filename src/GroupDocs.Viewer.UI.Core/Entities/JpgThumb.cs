namespace GroupDocs.Viewer.UI.Core.Entities
{
    class JpgThumb : Thumb
    {
        public JpgThumb(int pageNumber, byte[] thumbData) 
            : base(pageNumber, thumbData)
        {
        }

        public static string Extension => ".jpeg";

        public override string ContentType => "image/jpeg";
    }
}