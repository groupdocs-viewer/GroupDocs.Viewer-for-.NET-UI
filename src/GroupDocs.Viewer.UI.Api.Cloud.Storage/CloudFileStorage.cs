using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Requests;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.Configuration;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.Api.Cloud.Storage
{
    public class CloudFileStorage : IFileStorage
    {
        private readonly Config _config;
        private readonly IStorageApiConnect _apiConnect;

        public CloudFileStorage(IOptions<Config> config, IStorageApiConnect apiConnect)
        {
            _apiConnect = apiConnect;
            _config = config.Value;
        }

        public async Task<IEnumerable<FileSystemEntry>> ListDirsAndFilesAsync(string dirPath)
        {
            var request = new GetFilesListRequest(dirPath, _config.StorageName);
            var result = await _apiConnect.GetFilesList(request);

            if (result.IsFailure)
                throw new Exception(result.Message);

            var entries = result.Value.Value
                .Select(file =>
                {
                    var path = file.Path.StartsWith('/')
                        ? file.Path.Substring(1)
                        : file.Path;

                    return file.IsFolder
                        ? FileSystemEntry.Directory(file.Name, path, file.Size)
                        : FileSystemEntry.File(file.Name, path, file.Size);
                })
                .ToList();

            return entries;
        }

        public async Task<byte[]> ReadFileAsync(string filePath)
        {
            var request = new DownloadFileRequest(filePath, _config.StorageName);
            var result = await _apiConnect.DownloadFile(request);

            if (result.IsFailure)
                throw new Exception(result.Message);

            return result.Value;
        }

        public async Task<string> WriteFileAsync(string fileName, byte[] bytes, bool rewrite)
        {
            if (!rewrite)
                fileName = await GetFreeFileName(fileName);

            var request = new UploadFileRequest(fileName, bytes, _config.StorageName);
            var result = await _apiConnect.UploadFile(request);
            if (result.IsFailure)
                throw new Exception(result.Message);

            return fileName;
        }

        private async Task<string> GetFreeFileName(string fileName)
        {
            var exist = await FileExists(fileName);
            if (!exist) return fileName;

            var fileNameWithExtension = Path.GetFileName(fileName);
            var dirPath = fileName.Replace(fileNameWithExtension, string.Empty);
            var dirFiles = await ListDirsAndFilesAsync(dirPath);

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            var number = 1;
            string fileNameCandidate;
            do
            {
                string newFileName = $"{fileNameWithoutExtension} ({number})";
                fileNameCandidate = fileName.Replace(fileNameWithoutExtension, newFileName);
                number++;
            } while (dirFiles.Any(dirFile => 
                dirFile.FileName.Equals(fileNameCandidate, StringComparison.InvariantCultureIgnoreCase)));

            return fileNameCandidate;
        }

        private async Task<bool> FileExists(string fileName)
        {
            var request = new ObjectExistRequest(fileName, _config.StorageName);
            var result = await _apiConnect.CheckObjectExistsAsync(request);

            if (result.IsFailure)
                throw new Exception(result.Message);

            return result.Value;

        }
    }
}