using GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Requests;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Responses;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.Common;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect
{
    internal class StorageApiConnect : IStorageApiConnect
    {
        private readonly HttpClient _httpClient;

        private readonly JsonSerializerSettings _jsonSerializerSettings
            = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            };

        public StorageApiConnect(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Result<FilesList>> GetFilesList(GetFilesListRequest request)
        {
            var uri = $"viewer/storage/folder/{request.Path}?storageName={request.StorageName}";
            Result<FilesList> result = await Send<FilesList>(uri, HttpMethod.Get);
            return result;
        }

        public async Task<Result<byte[]>> DownloadFile(DownloadFileRequest request)
        {
            var requestUri = $"viewer/storage/file/{request.Path}?storageName={request.StorageName}";
            Result<byte[]> result = await Download<FilesList>(requestUri);
            return result;
        }

        public async Task<Result> UploadFile(UploadFileRequest request)
        {
            var requestUri = $"viewer/storage/file/{request.FileName}?storageName={request.StorageName}";
            var uploadResult = await Upload<UploadedFiles>(requestUri, request.Data);
            if (uploadResult.IsFailure)
                return Result.Fail(uploadResult.Message);

            if (uploadResult.Value.Errors.Any())
                return Result.Fail(string.Join("; ", uploadResult.Value.Errors));

            return Result.Ok();
        }

        public Result<string> FileLink(DownloadFileRequest request)
        {
            return Result.Ok($"viewer/storage/file/{request.Path}?storageName={request.StorageName}");
        }

        public async Task<Result<bool>> CheckObjectExistsAsync(ObjectExistRequest request)
        {
            var requestUri = $"viewer/storage/exist/{request.Path}?storageName={request.StorageName}";
            var existResult = await Send<ObjectExist>(requestUri, HttpMethod.Get);

            if (existResult.IsFailure)
                return Result.Fail<bool>(existResult);

            return Result.Ok(existResult.Value.Exists);
        }

        private async Task<Result<T>> Send<T>(string requestUri, HttpMethod method, object request = null)
        {
            var message = new HttpRequestMessage(method, requestUri);

            if (request != null)
            {
                var requestJson = JsonConvert.SerializeObject(request, _jsonSerializerSettings);
                message.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(message);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (TryDeserialize(responseJson, out ErrorResult errorResponse)
                    && errorResponse.Error != null)
                    return Result.Fail<T>(errorResponse.Error.Message);

                if (TryDeserialize(responseJson, out Error error)
                    && error.Message != null)
                    return Result.Fail<T>(error.Message);

                return Result.Fail<T>(responseJson);
            }

            var obj = JsonConvert.DeserializeObject<T>(responseJson, _jsonSerializerSettings);
            return Result.Ok(obj);
        }

        private async Task<Result<T>> Upload<T>(string requestUri, byte[] data)
        {
            var formDataContent = new MultipartFormDataContent();
            formDataContent.Add(new ByteArrayContent(data), "File", "File");

            var message = new HttpRequestMessage(HttpMethod.Put, requestUri);
            message.Content = formDataContent;

            var response = await _httpClient.SendAsync(message);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (TryDeserialize(responseJson, out ErrorResult errorResponse)
                    && errorResponse.Error != null)
                    return Result.Fail<T>(errorResponse.Error.Message);

                if (TryDeserialize(responseJson, out Error error)
                    && error.Message != null)
                    return Result.Fail<T>(error.Message);

                return Result.Fail<T>(responseJson);
            }

            var obj = JsonConvert.DeserializeObject<T>(responseJson, _jsonSerializerSettings);
            return Result.Ok(obj);
        }

        private async Task<Result<byte[]>> Download<T>(string requestUri)
        {
            var response = await _httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();

                if (TryDeserialize(responseJson, out ErrorResult errorResponse)
                    && errorResponse.Error != null)
                    return Result.Fail<byte[]>(errorResponse.Error.Message);

                if (TryDeserialize(responseJson, out Error error)
                    && error.Message != null)
                    return Result.Fail<byte[]>(error.Message);

                return Result.Fail<byte[]>(responseJson);
            }

            var bytes = await response.Content.ReadAsByteArrayAsync();
            return Result.Ok(bytes);
        }

        private bool TryDeserialize<T>(string json, out T obj)
        {
            try
            {
                obj = JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
            }
            catch (JsonSerializationException)
            {
                obj = default(T);
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// The error result
    /// </summary>
    public class ErrorResult
    {
        /// <summary>
        /// The error
        /// </summary>
        public Error Error { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ErrorResult {\n");
            sb.Append("  Error: ").Append(this.Error).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Object exists
    /// </summary>
    public class ObjectExist
    {
        /// <summary>
        /// Indicates that the file or folder exists.
        /// </summary>
        public bool Exists { get; set; }

        /// <summary>
        /// True if it is a folder, false if it is a file.
        /// </summary>
        public bool IsFolder { get; set; }

        /// <summary>
        /// Get the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class ObjectExist {\n");
            sb.Append("  Exists: ").Append(this.Exists).Append("\n");
            sb.Append("  IsFolder: ").Append(this.IsFolder).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
    }
}