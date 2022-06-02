using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Infrastructure;
using GroupDocs.Viewer.UI.Api.Models;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Configuration;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.Api.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ViewerController : ControllerBase
    {
        private readonly IFileStorage _fileStorage;
        private readonly IViewer _viewer;
        private readonly ILogger<ViewerController> _logger;
        private readonly Config _config;

        public ViewerController(IFileStorage fileStorage, 
            IViewer viewer, 
            IOptions<Config> config, 
            ILogger<ViewerController> logger)
        {
            _fileStorage = fileStorage;
            _viewer = viewer;
            _logger = logger;
            _config = config.Value;
        }

        [HttpGet]
        public IActionResult LoadConfig()
        {
            var config = new LoadConfigResponse
            {
                PageSelector = _config.PageSelector,
                Download = _config.Download,
                Upload = _config.Upload,
                Print = _config.Print,
                Browse = _config.Browse,
                Rewrite = _config.Rewrite,
                EnableRightClick = _config.EnableRightClick,
                DefaultDocument = _config.DefaultDocument,
                PreloadPageCount = _config.PreloadPageCount,
                Zoom = _config.Zoom,
                Search = _config.Search,
                Thumbnails = _config.Thumbnails,
                HtmlMode = _config.HtmlMode,
                PrintAllowed = _config.PrintAllowed,
                Rotate = _config.Rotate,
                SaveRotateState = _config.SaveRotateState,
                DefaultLanguage = _config.DefaultLanguage,
                SupportedLanguages = _config.SupportedLanguages,
                ShowLanguageMenu = _config.ShowLanguageMenu
            };

            return OkJsonResult(config);
        }

        [HttpPost]
        public async Task<IActionResult> LoadFileTree([FromBody] LoadFileTreeRequest request)
        {
            if (!_config.Browse)
                return ErrorJsonResult("Browsing files is disabled.");

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
                _logger.LogError(ex, "Failed to load file tree.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDocument([FromQuery] string path)
        {
            if (!_config.Download)
                return ErrorJsonResult("Downloading files is disabled.");

            try
            {
                var fileName = Path.GetFileName(path);
                var bytes = await _fileStorage.ReadFileAsync(path);

                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download a document.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadDocumentPageResource([FromQuery] LoadDocumentPageResource request)
        {
            if (!_config.HtmlMode)
                return ErrorJsonResult("Loading page resources is disabled in image mode.");

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
                _logger.LogError(ex, "Failed to load document page resource.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument()
        {
            if (!_config.Upload)
                return ErrorJsonResult("Uploading files is disabled.");

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
                _logger.LogError(ex, "Failed to upload document.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PrintPdf([FromBody] PrintPdfRequest request)
        {
            if (!_config.Print)
                return ErrorJsonResult("Printing files is disabled.");

            try
            {
                var filename = Path.GetFileName(request.Guid);
                var pdfFileName = Path.ChangeExtension(filename, ".pdf");
                var pdfFileBytes = await _viewer.GetPdfAsync(request.Guid, request.Password);

                return File(pdfFileBytes, "application/pdf", pdfFileName);
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

                _logger.LogError(ex, "Failed to create PDF file.");

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

                _logger.LogError(ex, "Failed to read document description.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoadDocumentPages([FromBody] LoadDocumentPagesRequest request)
        {
            try
            {
                var pages = await _viewer.GetPagesAsync(request.Guid, request.Password, request.Pages);
                var pageContents = pages
                    .Select(page => new PageContent { Number = page.PageNumber, Data = page.GetContent() })
                    .ToList();

                return OkJsonResult(pageContents);
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

                _logger.LogError(ex, "Failed to retrieve document pages.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoadDocumentPage([FromBody] LoadDocumentPageRequest request)
        {
            try
            {
                throw new Exception("TEST");
                var page = await _viewer.GetPageAsync(request.Guid, request.Password, request.Page);
                var pageContent = new PageContent { Number = page.PageNumber, Data = page.GetContent() };

                return OkJsonResult(pageContent);
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

                _logger.LogError(ex, "Failed to retrieve document page.");

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
            new ViewerActionResult(new ErrorResponse(message))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

        private IActionResult ForbiddenJsonResult(string message) =>
            new ViewerActionResult(new ErrorResponse(message))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };

        private IActionResult NotFoundJsonResult(string message) =>
            new ViewerActionResult(new ErrorResponse(message))
            {
                StatusCode = StatusCodes.Status404NotFound
            };

        private IActionResult OkJsonResult(object result) =>
            new ViewerActionResult(result);
    }
}
