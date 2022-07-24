using System.IO;
using GroupDocs.Viewer.UI.Configuration;

namespace GroupDocs.Viewer.UI.Core
{
    public class UIStylesheet
    {
        private const string StylesheetsPath = "css";
        public string FileName { get; }
        public byte[] Content { get; }
        public string ResourcePath { get; }
        public string ResourceRelativePath { get; }

        private UIStylesheet(Options options, string filePath)
        {
            FileName = Path.GetFileName(filePath);
            Content = File.ReadAllBytes(filePath);
            ResourcePath = $"{options.UIPath}/{StylesheetsPath}/{FileName}";
            ResourceRelativePath = $"{StylesheetsPath}/{FileName}";
        }

        public static UIStylesheet Create(Options options, string filePath)
        {
            return new UIStylesheet(options, filePath);
        }
    }
}