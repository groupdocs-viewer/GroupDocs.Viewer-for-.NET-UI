using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class FileSystemItem
    {
        /// <summary>
        /// File unique ID.
        /// </summary>
        [JsonPropertyName("path")]
        public string Path { get; }
        
        /// <summary>
        /// File file name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// <value>True</value> when it is a directory.
        /// </summary>
        [JsonPropertyName("isDir")]
        public bool IsDir { get; }

        /// <summary>
        /// Size in bytes.
        /// </summary>
        [JsonPropertyName("size")]
        public long Size { get; }

        /// <summary>
        /// .ctor
        /// </summary>
        public FileSystemItem(string path, string name, bool isDir, long size)
        {
            Path = path;
            Name = name;
            IsDir = isDir;
            Size = size;
        }
    }
}