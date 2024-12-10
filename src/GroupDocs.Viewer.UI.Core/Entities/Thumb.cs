namespace GroupDocs.Viewer.UI.Core.Entities
{
    public abstract class Thumb
    {
        protected Thumb(int pageNumber, byte[] thumbData)
        {
            PageNumber = pageNumber;
            ThumbData = thumbData;
        }

        public int PageNumber { get; }

        public byte[] ThumbData { get; protected set; }

        public abstract string ContentType { get; }
    }
} 