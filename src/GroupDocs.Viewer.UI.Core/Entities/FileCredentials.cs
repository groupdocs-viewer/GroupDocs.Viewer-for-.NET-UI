namespace GroupDocs.Viewer.UI.Core.Entities
{
    public class FileCredentials
    {
        public string FilePath { get; }
        public string FileType { get; }
        public string Password { get; }

        public FileCredentials(string filePath, string password)
        {
            FilePath = filePath;
            Password = password;
        }
    }
}