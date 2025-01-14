using System;
using System.Text;

namespace GroupDocs.Viewer.UI.NetFramework.Core
{
    internal class UIResource
    {
        public byte[] Content { get; internal set; }
        public string ContentType { get; }
        public string FileName { get; }

        private UIResource(string fileName, byte[] content, string contentType)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content));
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        }

        public string GetContentString() => Encoding.UTF8.GetString(Content);

        public void SetContentString(string content)
        {
            Content = Encoding.UTF8.GetBytes(content);
        } 

        public static UIResource Create(string fileName, byte[] content, string contentType)
        {
            return new UIResource(fileName, content, contentType);
        }
    }
}
