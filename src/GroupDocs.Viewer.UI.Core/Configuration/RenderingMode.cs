namespace GroupDocs.Viewer.UI.Core.Configuration
{
    public class RenderingMode
    {
        public static readonly RenderingMode Html = new RenderingMode("html");
        public static readonly RenderingMode Image = new RenderingMode("image");
        
        public string Value { get; }

        public RenderingMode(string value)
        {
            Value = value;
        }
    }
}