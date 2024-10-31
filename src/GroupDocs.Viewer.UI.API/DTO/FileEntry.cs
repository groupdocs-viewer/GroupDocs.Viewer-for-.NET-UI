using System;
using System.IO;

namespace GroupDocs.Viewer.UI.Api.DTO;

public class FileEntry
{
    public string FileName { get; set; }

    public string FolderName { get; set; }

    public string Password { get; set; }

    public FileEntry()
    {
    }

    public FileEntry(string fileName) => this.FileName = FileEntry.ToTrusted(fileName);

    public FileEntry(string fileName, string folderName)
    {
        this.FileName = FileEntry.ToTrusted(fileName);
        this.FolderName = folderName;
    }

    public FileEntry(string fileName, string folderName, string password)
    {
        this.FileName = FileEntry.ToTrusted(fileName);
        this.FolderName = folderName;
        this.Password = password;
    }

    public static string ToTrusted(string value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));
        return string.Concat(value.Split(Path.GetInvalidFileNameChars()));
    }

    public string GetExtension()
    {
        string extension = Path.GetExtension(this.FileName);
        return string.IsNullOrEmpty(extension)
            ? string.Empty
            : extension.Substring(1, extension.Length - 1).ToLowerInvariant();
    }

    public override string ToString() => this.FolderName + "/" + this.FileName;

    public bool Valid
    {
        get => !string.IsNullOrEmpty(this.FolderName) && !string.IsNullOrEmpty(this.FileName);
    }
}

public class FileInfo
{
    public bool PasswordProtected { get; set; }

    public bool NotProtected => !PasswordProtected;
}