using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models;
using GroupDocs.Viewer.UI.Cloud.Api.Common;
using GroupDocs.Viewer.UI.Core.Entities;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect
{
    internal class ViewerApiConnect : IViewerApiConnect
    {
        private readonly HttpClient _httpClient;

        private readonly JsonSerializerOptions _jsonSerializerOptions
            = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true
            };


        public ViewerApiConnect(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Result<DocumentInfo>> GetDocumentInfoAsync(FileInfo fileInfo, ViewOptions viewOptions)
        {
            var result = await Send<InfoResult>("viewer/info", HttpMethod.Post, viewOptions);

            if (!result.IsSuccess)
                return Result.Fail<DocumentInfo>(result);

            var documentInfo = ToDocumentInfo(result.Value);
            return Result.Ok(documentInfo);
        }

        public async Task<Result<byte[]>> GetPdfFileAsync(FileInfo fileInfo, ViewOptions viewOptions)
        {
            var viewResult = await Send<ViewResult>("viewer/view", HttpMethod.Post, viewOptions);
            if (!viewResult.IsSuccess)
                return Result.Fail<byte[]>(viewResult);

            var pdfFile = viewResult.Value.File;
            var pdfBytesResult = await DownloadResourceAsync(pdfFile, fileInfo.StorageName);

            return pdfBytesResult;
        }

        public async Task<Result<ViewResult>> CreatePagesAsync(FileInfo fileInfo, int[] pageNumbers,
            ViewOptions viewOptions)
        {
            viewOptions.RenderOptions.PagesToRender = pageNumbers.ToList();

            var viewResult = await Send<ViewResult>("viewer/view", HttpMethod.Post, viewOptions);
            if (!viewResult.IsSuccess)
                return Result.Fail<ViewResult>(viewResult);

            return Result.Ok(viewResult.Value);
        }

        public async Task<Result<ViewResult>> CreateThumbsAsync(FileInfo fileInfo, int[] pageNumbers,
            ViewOptions viewOptions)
        {
            viewOptions.RenderOptions.PagesToRender = pageNumbers.ToList();

            var viewResult = await Send<ViewResult>("viewer/view", HttpMethod.Post, viewOptions);
            if (!viewResult.IsSuccess)
                return Result.Fail<ViewResult>(viewResult);

            return Result.Ok(viewResult.Value);
        }

        public async Task<Result<byte[]>> DownloadResourceAsync(Resource resource, string storageName)
        {
            var requestUri = $"viewer/storage/file/{resource.Path}?storageName={storageName}";
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

        public async Task<Result<bool>> CheckFileExistsAsync(string filePath, string storageName)
        {
            var requestUri = $"viewer/storage/exist/{filePath}?storageName={storageName}";
            var existResult = await Send<ObjectExist>(requestUri, HttpMethod.Get);

            if (existResult.IsFailure)
                return Result.Fail<bool>(existResult);

            return Result.Ok(existResult.Value.Exists);
        }

        public async Task<Result> UploadFileAsync(string filePath, string storageName, byte[] bytes)
        {
            var requestUri = $"viewer/storage/file/{filePath}?storageName={storageName}";
            var uploadResult = await Upload<FilesUploadResult>(requestUri, bytes);
            return uploadResult;
        }

        public async Task<Result> DeleteView(FileInfo fileInfo, string outputPath)
        {
            var requestUri = "viewer/view";
            var request = new DeleteViewOptions
            {
                FileInfo = fileInfo,
                OutputPath = outputPath
            };
            var deleteResult = await Delete(requestUri, request);
            return deleteResult;
        }

        private async Task<Result<T>> Send<T>(string requestUri, HttpMethod method, object request = null)
        {
            var message = new HttpRequestMessage(method, requestUri);

            if (request != null)
            {
                var requestJson = JsonSerializer.Serialize(request, _jsonSerializerOptions);
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

            var obj = JsonSerializer.Deserialize<T>(responseJson, _jsonSerializerOptions);
            return Result.Ok(obj);
        }

        private async Task<Result<T>> Upload<T>(string requestUri, byte[] data)
        {
            var formData = new MultipartFormDataContent();
            formData.Add(new ByteArrayContent(data), "File", "File");

            var message = new HttpRequestMessage(HttpMethod.Put, requestUri);
            message.Content = formData;

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

            var obj = JsonSerializer.Deserialize<T>(responseJson, _jsonSerializerOptions);
            return Result.Ok(obj);
        }

        private async Task<Result> Delete(string requestUri, object request = null)
        {
            var message = new HttpRequestMessage(HttpMethod.Delete, requestUri);

            if (request != null)
            {
                var requestJson = JsonSerializer.Serialize(request, _jsonSerializerOptions);
                message.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            }

            var response = await _httpClient.SendAsync(message);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                if (TryDeserialize(responseJson, out ErrorResult errorResponse)
                    && errorResponse.Error != null)
                    return Result.Fail(errorResponse.Error.Message);

                if (TryDeserialize(responseJson, out Error error)
                    && error.Message != null)
                    return Result.Fail(error.Message);

                return Result.Fail(responseJson);
            }

            return Result.Ok();
        }

        private DocumentInfo ToDocumentInfo(InfoResult infoResponse)
        {
            var documentInfo = new DocumentInfo();

            documentInfo.Pages =
                infoResponse.Pages.Select(p => new Core.Entities.PageInfo
                {
                    Height = p.Height.GetValueOrDefault(),
                    Width = p.Width.GetValueOrDefault(),
                    Name = $"Page {p.Number.GetValueOrDefault()}",
                    Number = p.Number.GetValueOrDefault()
                });

            if (infoResponse.PdfViewInfo?.PrintingAllowed != null)
                documentInfo.PrintAllowed = infoResponse.PdfViewInfo.PrintingAllowed;

            return documentInfo;
        }

        private bool TryDeserialize<T>(string json, out T obj)
        {
            try
            {
                obj = JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
                return true;
            }
            catch (JsonException) // System.Text.Json-specific exception
            {
                obj = default;
                return false;
            }
        }

    }
}