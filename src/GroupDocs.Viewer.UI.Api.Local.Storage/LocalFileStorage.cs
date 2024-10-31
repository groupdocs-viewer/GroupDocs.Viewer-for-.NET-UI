using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api.Local.Storage
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly string _storagePath;
        private readonly TimeSpan _waitTimeout = TimeSpan.FromMilliseconds(100);

        public LocalFileStorage(string storagePath)
        {
            _storagePath = storagePath;
        }

        private IEnumerable<FileSystemEntry> ListFiles(string folderPath)
        {
            var folderFullPath = string.IsNullOrEmpty(folderPath)
                ? _storagePath
                : Path.Combine(_storagePath, folderPath);

            var dirs = Directory.GetDirectories(folderFullPath)
                .Select(file => new FileInfo(file))
                .Where(fileInfo => !fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                .OrderBy(fileInfo => fileInfo.Name)
                .ThenByDescending(fileInfo => fileInfo.CreationTime)
                .Select(directory =>
                     FileSystemEntry.Directory(directory.Name, Path.GetRelativePath(_storagePath, directory.FullName), 0L));

            var files = Directory
                .GetFiles(folderFullPath)
                .Select(file => new FileInfo(file))
                .Where(fileInfo => !fileInfo.Attributes.HasFlag(FileAttributes.Hidden))
                .OrderBy(fileInfo => fileInfo.Name)
                .ThenByDescending(fileInfo => fileInfo.CreationTime)
                .Select(file =>
                    FileSystemEntry.File(file.Name, Path.GetRelativePath(_storagePath, file.FullName), file.Length));

            var dirsAndFiles = dirs.Concat(files);
            return dirsAndFiles;
        }

        public Task<IEnumerable<FileSystemEntry>> ListDirsAndFilesAsync(string dirPath) =>
            Task.FromResult(ListFiles(dirPath));

        public async Task<byte[]> ReadFileAsync(string filePath)
        {
            var fullPath = Path.Combine(_storagePath, filePath);
            await using FileStream fs = GetStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.None);
            var memoryStream = new MemoryStream();
            await fs.CopyToAsync(memoryStream);

            return memoryStream.ToArray();
        }

        public async Task<string> WriteFileAsync(string fileName, byte[] bytes, bool rewrite)
        {
            var newFileName = rewrite ? fileName : GetFreeFileName(fileName);
            var fullPath = Path.Combine(_storagePath, newFileName);
            var fileMode = rewrite ? FileMode.Create : FileMode.CreateNew;

            await using FileStream fs = GetStream(fullPath, fileMode, FileAccess.Write, FileShare.None);
            await fs.WriteAsync(bytes, 0, bytes.Length);

            return newFileName;
        }

        public string GetFileLink(string fileKey, int pageNumber)
        {
            return $"storage/{fileKey}/{pageNumber}";
        }

        private FileStream GetStream(string path, FileMode mode, FileAccess access, FileShare share)
        {
            FileStream stream = null;
            TimeSpan interval = new TimeSpan(0, 0, 0, 0, 50);
            TimeSpan totalTime = new TimeSpan();

            while (stream == null)
            {
                try
                {
                    stream = File.Open(path, mode, access, share);
                }
                catch (IOException)
                {
                    Thread.Sleep(interval);
                    totalTime += interval;

                    if (_waitTimeout.Ticks != 0 && totalTime > _waitTimeout)
                    {
                        throw;
                    }
                }
            }

            return stream;
        }

        private string GetFreeFileName(string fileName)
        {
            var fullPath = Path.Combine(_storagePath, fileName);

            if (!File.Exists(fullPath))
                return fileName;

            List<string> dirFiles = Directory.GetFiles(_storagePath)
                .Select(Path.GetFileName)
                .ToList();

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var number = 1;
            string fileNameCandidate;
            do
            {
                string newFileName = $"{fileNameWithoutExtension} ({number})";
                fileNameCandidate = fileName.Replace(fileNameWithoutExtension, newFileName);
                number++;
            } while (dirFiles.Contains(fileNameCandidate));

            return fileNameCandidate;
        }
    }
}