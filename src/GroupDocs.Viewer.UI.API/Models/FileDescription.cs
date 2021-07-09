using System.Text.Json.Serialization;

namespace GroupDocs.Viewer.UI.Api.Models
{
    public class FileDescription
    {
        [JsonPropertyName("guid")]
        public string Guid { get; }
        
        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("isDirectory")]
        public bool IsDirectory { get; }

        [JsonPropertyName("size")]
        public long Size { get; }

        public FileDescription(string guid, string name, bool isDirectory, long size)
        {
            Guid = guid;
            Name = name;
            IsDirectory = isDirectory;
            Size = size;
        }
    }
}