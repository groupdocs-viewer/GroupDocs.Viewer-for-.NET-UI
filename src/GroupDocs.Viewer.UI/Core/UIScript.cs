using System.IO;
using GroupDocs.Viewer.UI.Configuration;

namespace GroupDocs.Viewer.UI.Core
{
    public class UIScript
    {
        private const string ScriptsPath = "js";
        public string FileName { get; }
        public byte[] Content { get; }
        public string ResourcePath { get; }
        public string ResourceRelativePath { get; }

        private UIScript(Options options, string filePath)
        {
            FileName = Path.GetFileName(filePath);
            Content = File.ReadAllBytes(filePath);
            ResourcePath = $"{options.UIPath.TrimEnd('/')}/{ScriptsPath}/{FileName}";
            ResourceRelativePath = $"{ScriptsPath}/{FileName}";
        }

        public static UIScript Create(Options options, string filePath)
        {
            return new UIScript(options, filePath);
        }
    }
}
