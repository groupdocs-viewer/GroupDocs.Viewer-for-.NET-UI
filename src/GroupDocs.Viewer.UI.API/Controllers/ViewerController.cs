using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Models;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GroupDocs.Viewer.UI.Api.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ViewerController : ControllerBase
    {
        private readonly IFileStorage _fileStorage;
        private readonly IViewer _viewer;

        private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
        {
            IgnoreNullValues = false
        };

        public ViewerController(IFileStorage fileStorage, IViewer viewer)
        {
            _fileStorage = fileStorage;
            _viewer = viewer;
        }

        [HttpPost]
        public async Task<IActionResult> LoadFileTree([FromBody] LoadFileTreeRequest request)
        {
            try
            {
                var files =
                await _fileStorage.ListDirsAndFilesAsync(request.Path);

                var result = files
                    .Select(entity => new FileDescription(entity.FilePath, entity.FilePath, entity.IsDirectory, entity.Size))
                    .ToList();

                return OkJsonResult(result);
            }
            catch (Exception ex)
            {
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDocument([FromQuery] string path)
        {
            try
            {
                var fileName = Path.GetFileName(path);
                var bytes = await _fileStorage.ReadFileAsync(path);

                return File(bytes, Keys.BINARY_FILE_CONTENT_TYPE, fileName);
            }
            catch (Exception ex)
            {
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadDocumentPageResource([FromQuery]LoadDocumentPageResource request)
        {
            try
            {
                var bytes = await _viewer.GetPageResourceAsync(request.Guid, request.Password, request.PageNumber, request.ResourceName);

                if (bytes.Length == 0)
                    return NotFoundJsonResult($"Resource {request.ResourceName} was not found");

                var contentType = request.ResourceName.ContentTypeFromFileName();

                return File(bytes, contentType);
            }
            catch (Exception ex)
            {
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument()
        {
            try
            {
                var (fileName, bytes) = await ReadOrDownloadFile();
                bool.TryParse(Request.Form["rewrite"], out var rewrite);

                var filePath = await _fileStorage.WriteFileAsync(fileName, bytes, rewrite);

                var result = new UploadFileResponse(filePath);

                return OkJsonResult(result);
            }
            catch (Exception ex)
            {
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PrintPdf([FromBody] PrintPdfRequest request)
        {
            try
            {
                var filename = Path.GetFileName(request.Guid);
                var pdfFileName = Path.ChangeExtension(filename, Keys.PDF_FILE_EXTENSION);
                var pdfFileBytes = await _viewer.CreatePdfAsync(request.Guid, request.Password);

                return File(pdfFileBytes, Keys.PDF_FILE_CONTENT_TYPE, pdfFileName);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                {
                    var message = string.IsNullOrEmpty(request.Password)
                        ? "Password Required"
                        : "Incorrect Password";

                    return ForbiddenJsonResult(message);
                }

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoadDocumentDescription([FromBody] LoadDocumentDescriptionRequest request)
        {
            try
            {
                DocumentInfo documentDescription =
                    await _viewer.GetDocumentInfoAsync(request.Guid, request.Password);

                var result = new LoadDocumentDescriptionResponse
                {
                    Guid = request.Guid,
                    PrintAllowed = documentDescription.PrintAllowed,
                    Pages = documentDescription.Pages.Select(p => new PageDescription
                    {
                        Width = p.Width,
                        Height = p.Height,
                        Number = p.Number,
                        SheetName = p.Name,
                    }).ToList()
                };

                return OkJsonResult(result);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                {
                    var message = string.IsNullOrEmpty(request.Password)
                            ? "Password Required"
                            : "Incorrect Password";

                    return ForbiddenJsonResult(message);
                }

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoadDocumentPages([FromBody] LoadDocumentPagesRequest request)
        {
            try
            {
                var pages =
                                (await _viewer.RenderPagesAsync(request.Guid, request.Password, request.Pages))
                                .Select(page => new PageContent { Number = page.Number, Data = page.Data })
                                .ToList();

                return OkJsonResult(pages);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                {
                    var message = string.IsNullOrEmpty(request.Password)
                        ? "Password Required"
                        : "Incorrect Password";

                    return ForbiddenJsonResult(message);
                }

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoadDocumentPage([FromBody] LoadDocumentPagesRequest request)
        {
            try
            {
                var pages = await _viewer.RenderPagesAsync(request.Guid, request.Password, request.Pages);
                var page = pages.FirstOrDefault();

                return OkJsonResult(page);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                {
                    var message = string.IsNullOrEmpty(request.Password)
                        ? "Password Required"
                        : "Incorrect Password";

                    return ForbiddenJsonResult(message);
                }

                return ErrorJsonResult(ex.Message);
            }
        }

        private Task<(string, byte[])> ReadOrDownloadFile()
        {
            var url = Request.Form["url"].ToString();

            return string.IsNullOrEmpty(url)
                ? ReadFileFromRequest()
                : DownloadFileAsync(url);
        }

        private async Task<(string, byte[])> ReadFileFromRequest()
        {
            var formFile = Request.Form.Files.First();
            var stream = new MemoryStream();

            await formFile.CopyToAsync(stream);

            return (formFile.FileName, stream.ToArray());
        }

        private async Task<(string, byte[])> DownloadFileAsync(string url)
        {
            Uri uri = new Uri(url);

            using WebClient client = new WebClient();
            client.Headers.Add("User-Agent: Other");

            byte[] bytes = await client.DownloadDataTaskAsync(uri);
            string fileName = Path.GetFileName(uri.LocalPath);

            return (fileName, bytes);
        }

        private IActionResult ErrorJsonResult(string message) =>
            new JsonResult(new ErrorResponse(message), _serializerOptions)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

        private IActionResult ForbiddenJsonResult(string message) =>
            new JsonResult(new ErrorResponse(message), _serializerOptions)
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

        private IActionResult NotFoundJsonResult(string message) =>
            new JsonResult(new ErrorResponse(message), _serializerOptions)
            {
                StatusCode = StatusCodes.Status404NotFound
            };

        private IActionResult OkJsonResult(object result) =>
            new JsonResult(result, _serializerOptions);
    }
}
