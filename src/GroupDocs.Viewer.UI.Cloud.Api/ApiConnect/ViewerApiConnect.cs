using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GroupDocs.Viewer.Cloud.Sdk.Model;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Cloud.Api.Common;
using GroupDocs.Viewer.UI.Core.Entities;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect
{
    internal class ViewerApiConnect : IViewerApiConnect
    {
        private readonly HttpClient _httpClient;

        private readonly JsonSerializerOptions _jsonSerializerOptions
            = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
                IgnoreNullValues = true
            };

        public ViewerApiConnect(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Result<DocumentInfo>> GetDocumentInfoAsync(string filePath, string password, string storageName)
        {
            var viewOptions = new ViewOptions
            {
                FileInfo = new FileInfo
                {
                    FilePath = filePath,
                    Password = password,
                    StorageName = storageName
                },

                RenderOptions = new RenderOptions(),

                ViewFormat = ViewOptions.ViewFormatEnum.PNG
            };

            var result = await SendPost<InfoResult>("viewer/info", viewOptions);

            if (!result.IsSuccess)
                return Result.Fail<DocumentInfo>(result);

            var documentInfo = ToDocumentInfo(result.Value);
            return Result.Ok(documentInfo);
        }

        public async Task<Result<byte[]>> GetPdfFileAsync(string filePath, string password, string storageName)
        {
            var viewOptions = new ViewOptions
            {
                FileInfo = new FileInfo
                {
                    FilePath = filePath,
                    Password = password,
                    StorageName = storageName
                },

                RenderOptions = new RenderOptions(),

                ViewFormat = ViewOptions.ViewFormatEnum.PDF
            };

            var viewResult = await SendPost<ViewResult>("viewer/view", viewOptions);
            if (!viewResult.IsSuccess)
                return Result.Fail<byte[]>(viewResult);

            var pdfFile = viewResult.Value.File;
            var pdfBytesResult = await DownloadFileAsync(pdfFile, storageName);

            return pdfBytesResult;
        }

        private async Task<Result<byte[]>> DownloadFileAsync(Resource resource, string storageName)
        {
            var requestUri = $"viewer/storage/file/{resource.Path}?storageName={storageName}";
            var response = await _httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();

                if (TryDeserialize(responseJson, out ErrorResponse errorResponse) 
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

        private async Task<Result<T>> SendPost<T>(string requestUri, object request)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, requestUri);
            var requestJson = JsonSerializer.Serialize(request, _jsonSerializerOptions);
            message.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(message);
            var responseJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (TryDeserialize(responseJson, out ErrorResponse errorResponse) 
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
                documentInfo.PrintAllowed = infoResponse.PdfViewInfo.PrintingAllowed.Value;

            return documentInfo;
        }

        private bool TryDeserialize<T>(string json, out T obj)
        {
            try
            {
                obj = JsonSerializer.Deserialize<T>(json, _jsonSerializerOptions);
            }
            catch (JsonSerializationException)
            {
                obj = default(T);
                return false;
            }

            return true;
        }

        private class Error
        {
            public string Message { get; set; }
        }

        private class ErrorResponse
        {
            public Error Error { get; set; }
        }
    }
}