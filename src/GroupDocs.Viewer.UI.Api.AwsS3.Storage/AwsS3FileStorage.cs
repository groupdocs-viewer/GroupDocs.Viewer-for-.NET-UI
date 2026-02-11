using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Api.AwsS3.Storage
{
    public class AwsS3FileStorage : IFileStorage
    {
        private readonly AwsS3Options _awsS3Options;

        public AwsS3FileStorage(AwsS3Options awsS3Options)
        {
            if (awsS3Options == null)
                throw new ArgumentNullException(nameof(awsS3Options));

            if (awsS3Options.S3Config == null)
                throw new ArgumentNullException(nameof(awsS3Options.S3Config));

            if(string.IsNullOrEmpty(awsS3Options.Region))
                throw new ArgumentNullException(nameof(awsS3Options.Region));

            _awsS3Options = awsS3Options;

            _awsS3Options.S3Config.RegionEndpoint = RegionEndpoint.EnumerableAllRegions.FirstOrDefault(x => x.SystemName == _awsS3Options.Region);
        }

        public async Task<IEnumerable<FileSystemEntry>> ListDirsAndFilesAsync(string folderPath)
        {
            using (IAmazonS3 client = CreateS3Client())
            {
                var objects =
                    await ListingObjectsAsync(client, _awsS3Options.BucketName, folderPath);

                return objects;
            }
        }

        private IAmazonS3 CreateS3Client()
        {
            bool keysProvided = !string.IsNullOrEmpty(_awsS3Options.AccessKey) &&
                                !string.IsNullOrEmpty(_awsS3Options.SecretKey);

            AmazonS3Client client = keysProvided
                ? new AmazonS3Client(_awsS3Options.AccessKey, _awsS3Options.SecretKey, _awsS3Options.S3Config)
                : new AmazonS3Client(_awsS3Options.S3Config);

            return client;
        }

        private static async Task<IEnumerable<FileSystemEntry>> ListingObjectsAsync(
            IAmazonS3 client, string bucketName, string folderPath)
        {
            List<FileSystemEntry> fileSystemEntries = new List<FileSystemEntry>();

            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = folderPath,
                Delimiter = "/",
            };
            ListObjectsV2Response response;

            do
            {
                response = await client.ListObjectsV2Async(request);

                response.CommonPrefixes
                    .ForEach(prefix =>
                    {
                        fileSystemEntries.Add(
                            FileSystemEntry.Directory(prefix, prefix, 0L));
                    });

                response.S3Objects
                    .ForEach(obj =>
                    {
                        fileSystemEntries.Add(
                            FileSystemEntry.File(GetObjectName(obj.Key), obj.Key, obj.Size.GetValueOrDefault()));
                    });

                // If the response is truncated, set the request ContinuationToken
                // from the NextContinuationToken property of the response.
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated.GetValueOrDefault());

            return fileSystemEntries;
        }

        private static string GetObjectName(string key) =>
            key.Split('/', StringSplitOptions.RemoveEmptyEntries).Last();

        public async Task<byte[]> ReadFileAsync(string filePath)
        {
            using (IAmazonS3 client = CreateS3Client())
            {
                var bytes =
                    await ReadObjectDataAsync(client, _awsS3Options.BucketName, filePath);

                return bytes;
            }
        }

        private static async Task<byte[]> ReadObjectDataAsync(IAmazonS3 client, string bucketName, string keyName)
        {
            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
            };

            using (GetObjectResponse response = await client.GetObjectAsync(request))
            using (Stream responseStream = response.ResponseStream)
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await responseStream.CopyToAsync(memoryStream);

                return memoryStream.ToArray();
            }
        }

        public async Task<string> WriteFileAsync(string fileName, byte[] bytes, bool rewrite)
        {
			using (IAmazonS3 client = CreateS3Client())
            using (MemoryStream stream = new MemoryStream(bytes))
			{
			    var newFileName = rewrite ? fileName : await GetFreeFileName(client, fileName);

                PutObjectRequest request = new PutObjectRequest
                {
                    BucketName = _awsS3Options.BucketName,
                    Key = newFileName,
                    InputStream = stream,
                };
                
                await client.PutObjectAsync(request);

                return newFileName;
			}
        }

        private async Task<string> GetFreeFileName(IAmazonS3 client, string filePath)
        {
            string dirPath = Path.GetDirectoryName(filePath);

            IEnumerable<string> dirFiles = (await ListingObjectsAsync(client, _awsS3Options.BucketName, dirPath))
                .Where(x => !x.IsDirectory)
                .Select(x => x.FilePath);

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
    }
}