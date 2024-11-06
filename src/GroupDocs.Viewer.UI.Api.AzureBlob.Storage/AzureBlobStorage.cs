using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api.AzureBlob.Storage
{
    public class AzureBlobStorage : IFileStorage
    {
        private readonly AzureBlobOptions _options;

        public AzureBlobStorage(AzureBlobOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrEmpty(options.ContainerName))
                throw new ArgumentNullException(nameof(options.ContainerName));

            _options = options;
        }

        public async Task<IEnumerable<FileSystemEntry>> ListDirsAndFilesAsync(string dirPath)
        {
            BlobContainerClient client = CreateClient();

            var fileSystemEntries = new List<FileSystemEntry>();

            await foreach (var item in client.GetBlobsByHierarchyAsync(prefix: dirPath, delimiter: "/"))
            {
                if (item.IsPrefix)
                {
                    fileSystemEntries.Add(
                        FileSystemEntry.Directory(item.Prefix, item.Prefix, 0L));
                }
                else if (item.IsBlob)
                {
                    fileSystemEntries.Add(
                        FileSystemEntry.File(GetObjectName(item.Blob.Name), item.Blob.Name, item.Blob.Properties.ContentLength ?? 0L));
                }
            }

            return fileSystemEntries;
        }

        public async Task<byte[]> ReadFileAsync(string filePath)
        {
            BlobContainerClient client = CreateClient();
            BlobClient blob = client.GetBlobClient(filePath);

            using (Stream stream = await blob.OpenReadAsync())
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);

                return memoryStream.ToArray();
            }
        }

        public async Task<string> WriteFileAsync(string filePath, byte[] bytes, bool rewrite)
        {
            BlobContainerClient client = CreateClient();
            var newFilePah = rewrite ? filePath : await GetFreeFileName(client, filePath);
            BlobClient blob = client.GetBlobClient(newFilePah);
            await blob.UploadAsync(new BinaryData(bytes));
            return newFilePah;
        }

        private static string GetObjectName(string key) =>
            key.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();

        private static async Task<string> GetFreeFileName(BlobContainerClient client, string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);
            var dirFiles = new List<BlobItem>();
            await foreach (var blob in client.GetBlobsAsync(prefix: dirPath))
            {
                dirFiles.Add(blob);
            }

            if (dirFiles.All(x => x.Name != filePath))
            {
                return filePath;
            }

            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            var number = 1;
            string fileNameCandidate;
            do
            {
                string newFileName = $"{fileNameWithoutExtension} ({number})";
                fileNameCandidate = filePath.Replace(fileNameWithoutExtension, newFileName);
                ++number;
            }
            while (dirFiles.Any(x => x.Name == fileNameCandidate));
            return fileNameCandidate;
        }

        private BlobContainerClient CreateClient()
        {
            BlobContainerClient client = new BlobContainerClient(ConnectionString, _options.ContainerName, _options.ClientOptions);
            client.CreateIfNotExists();
            return client;
        }

        private string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_options.AccountName))
                {
                    throw new ArgumentNullException(nameof(_options.AccountName));
                }

                if (string.IsNullOrEmpty(_options.AccountKey))
                {
                    throw new ArgumentNullException(nameof(_options.AccountKey));
                }

                return $"DefaultEndpointsProtocol=https;AccountName={_options.AccountName};AccountKey={_options.AccountKey}";
            }
        }
    }
}
