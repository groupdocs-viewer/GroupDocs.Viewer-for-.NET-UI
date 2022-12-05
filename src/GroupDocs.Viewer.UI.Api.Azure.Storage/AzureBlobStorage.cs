using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Azure.Storage;
using Azure.Storage.Blobs;

using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Api.Azure.Storage
{
	public class AzureBlobStorage : IFileStorage
	{
		private readonly AzureBlobOptions _options;

		public AzureBlobStorage(AzureBlobOptions options)
		{
			if(options == null)
				throw new ArgumentNullException(nameof(options));

			if(string.IsNullOrEmpty(options.ContainerName))
				throw new ArgumentNullException(nameof(options.ContainerName));

			_options = options;
		}

		public Task<IEnumerable<FileSystemEntry>> ListDirsAndFilesAsync(string dirPath)
		{
			BlobContainerClient client = CreateClient();

			return Task.FromResult(
				client.GetBlobs(prefix: dirPath).Select(
					x => FileSystemEntry.File(x.Name, dirPath, x.Properties.ContentLength ?? 0L)
			) );
		}

		public async Task<byte[]> ReadFileAsync(string filePath)
		{
			BlobContainerClient client = CreateClient();
			BlobClient blob = client.GetBlobClient(filePath);

			using(Stream stream = await blob.OpenReadAsync())
			using(MemoryStream memoryStream = new MemoryStream())
			{
				await stream.CopyToAsync(memoryStream);

				return memoryStream.ToArray();
			}
		}

		public async Task<string> WriteFileAsync(string filePath, byte[] bytes, bool rewrite)
		{
			BlobContainerClient client = CreateClient();
			
            var newFilePah = rewrite ? filePath : GetFreeFileName(client, filePath);

			BlobClient blob = client.GetBlobClient(newFilePah);

			await blob.UploadAsync(new BinaryData(bytes));

			return newFilePah;
		}

		private string GetFreeFileName(BlobContainerClient client, string filePath)
		{
			string dirPath = Path.GetDirectoryName(filePath);

			IEnumerable<string> dirFiles = client.GetBlobs(prefix: dirPath).Select(x => x.Name);

			if(!dirFiles.Contains(filePath))
				return filePath;

			var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
			var number = 1;
			string fileNameCandidate;

			do
			{
				string newFileName = $"{fileNameWithoutExtension} ({number})";
				fileNameCandidate = filePath.Replace(fileNameWithoutExtension, newFileName);
				++number;
			}
			while(dirFiles.Contains(fileNameCandidate));

			return fileNameCandidate;
		}

		private BlobContainerClient CreateClient()
		{
			BlobContainerClient client = new BlobContainerClient(_connectionString, _options.ContainerName, _options.ClientOptions);

			client.CreateIfNotExists();

			return client;
		}

		private string _connectionString
		{
			get
			{
				if(string.IsNullOrEmpty(_options.AccountName))
					throw new ArgumentNullException(nameof(_options.AccountName));

				if(string.IsNullOrEmpty(_options.AccountKey))
					throw new ArgumentNullException(nameof(_options.AccountKey));

				return $"DefaultEndpointsProtocol=https;AccountName={_options.AccountName};AccountKey={_options.AccountKey}";
			}
		}
	}
}
