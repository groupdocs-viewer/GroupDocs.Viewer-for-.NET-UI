namespace GroupDocs.Viewer.UI.Core.Entities
{
    class JpgThumb : Thumb
    {
        public JpgThumb(int pageNumber, byte[] thumbData) 
            : base(pageNumber, thumbData)
        {
        }

        public static string DefaultExtension => ".jpeg";

        public override string Extension => DefaultExtension;   

        public override string ContentType => "image/jpeg";
    }
}