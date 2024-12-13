namespace GroupDocs.Viewer.UI.Core.Entities
{
    class PngThumb : Thumb
    {
        public PngThumb(int pageNumber, byte[] thumbData) : base(pageNumber, thumbData)
        {
        }

        public static string DefaultExtension => ".png";

        public override string Extension => DefaultExtension;

        public override string ContentType => "image/png";
    }
}