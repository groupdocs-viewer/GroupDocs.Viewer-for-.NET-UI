using System.Text;
using System.Text.Json;

namespace GroupDocs.Viewer.UI.Sample.StaticContentMode.Generator
{
    public static class Utils
    {
        public static string SerializeToJson(object obj)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string json = JsonSerializer.Serialize(obj, jsonOptions);
            return json;
        }

        public static Task SaveFileAsync(string filePath, string json)
        {
            var dirPath = Path.GetDirectoryName(filePath);
            if (dirPath != null && !Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            return File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
        }

        public static Task SaveFileAsync(string filePath, byte[] bytes)
        {
            var dirPath = Path.GetDirectoryName(filePath);
            if (dirPath != null && !Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            return File.WriteAllBytesAsync(filePath, bytes);
        }
    }
}