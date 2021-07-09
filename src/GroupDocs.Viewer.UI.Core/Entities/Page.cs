namespace GroupDocs.Viewer.UI.Core.Entities
{
    public class Page
    {
        public Page(int number, string data)
        {
            Number = number;
            Data = data;
        }

        public int Number { get; }

        public string Data { get; }
    }
}