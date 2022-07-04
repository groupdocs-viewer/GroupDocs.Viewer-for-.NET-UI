using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class FileDescription
    {
        /// <summary>
        /// File unique ID.
        /// </summary>
        [JsonPropertyName("guid")]
        public string Guid { get; }
        
        /// <summary>
        /// File file name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; }

        /// <summary>
        /// <value>True</value> when it is a directory.
        /// </summary>
        [JsonPropertyName("isDirectory")]
        public bool IsDirectory { get; }

        /// <summary>
        /// Size in bytes.
        /// </summary>
        [JsonPropertyName("size")]
        public long Size { get; }

        /// <summary>
        /// .ctor
        /// </summary>
        public FileDescription(string guid, string name, bool isDirectory, long size)
        {
            Guid = guid;
            Name = name;
            IsDirectory = isDirectory;
            Size = size;
        }
    }
}