namespace GroupDocs.Viewer.UI.Core.Caching
{
    internal class CachedThumb
    {
        /// <summary>
        /// The page number.
        /// </summary>
        public int PageNumber { get; }

        /// <summary>
        /// The data. Can be null.
        /// </summary>
        public byte[] Data { get; }

        public CachedThumb(int pageNumber, byte[] data)
        {
            PageNumber = pageNumber;
            Data = data;
        }
    }
}