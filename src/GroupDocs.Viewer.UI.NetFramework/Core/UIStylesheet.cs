using System.IO;

namespace GroupDocs.Viewer.UI.NetFramework.Core
{
    internal class UIStylesheet
    {
        private const string StylesheetsPath = "css";
        public string FileName { get; }
        public byte[] Content { get; }
        public string ResourcePath { get; }
        public string ResourceRelativePath { get; }

        private UIStylesheet(ViewerUIConfig viewerUIOptions, string filePath)
        {
            FileName = Path.GetFileName(filePath);
            Content = File.ReadAllBytes(filePath);
            ResourcePath = $"{viewerUIOptions.UIPath}/{StylesheetsPath}/{FileName}";
            ResourceRelativePath = $"{StylesheetsPath}/{FileName}";
        }

        public static UIStylesheet Create(ViewerUIConfig options, string filePath)
        {
            return new UIStylesheet(options, filePath);
        }
    }
}