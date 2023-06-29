using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        private readonly IFileNameResolver _fileNameResolver;
        private readonly ISearchTermResolver _searchTermResolver;
        private readonly IUIConfigProvider _uiConfigProvider;
        private readonly IViewer _viewer;
        private readonly ILogger<ViewerController> _logger;
        private readonly Config _config;

        public ViewerController(IFileStorage fileStorage,
            IFileNameResolver fileNameResolver,
            ISearchTermResolver searchTermResolver,
            IUIConfigProvider uiConfigProvider,
            IViewer viewer,
            IOptions<Config> config,
            ILogger<ViewerController> logger)
        {
            _fileStorage = fileStorage;
            _fileNameResolver = fileNameResolver;
            _searchTermResolver = searchTermResolver;
            _viewer = viewer;
            _logger = logger;
            _config = config.Value;
            _uiConfigProvider = uiConfigProvider;
        }

        [HttpGet]
        public IActionResult LoadConfig()
        {
            _uiConfigProvider.ConfigureUI(_config);

            var config = new LoadConfigResponse
            {
                PageSelector = _config.IsPageSelector,
                Download = _config.IsDownload,
                Upload = _config.IsUpload,
                Print = _config.IsPrint,
                Browse = _config.IsBrowse,
                Rewrite = _config.Rewrite,
                EnableRightClick = _config.IsEnableRightClick,
                DefaultDocument = _config.DefaultDocument,
                PreloadPageCount = _config.PreloadPageCount,
                Zoom = _config.IsZoom,
                Search = _config.IsSearch,
                Thumbnails = _config.IsThumbnails,
                HtmlMode = _config.HtmlMode,
                PrintAllowed = _config.IsPrintAllowed,
                Rotate = _config.IsRotate,
                SaveRotateState = _config.SaveRotateState,
                DefaultLanguage = _config.DefaultLanguage,
                SupportedLanguages = _config.SupportedLanguages,
                ShowLanguageMenu = _config.IsShowLanguageMenu,
                ShowToolBar = _config.IsShowToolBar,
            };
            
            return OkJsonResult(config);
        }

        [HttpPost]
        public async Task<IActionResult> LoadFileTree([FromBody] LoadFileTreeRequest request)
        {
            if (!_config.IsBrowse)
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
            if (!_config.IsDownload)
                return ErrorJsonResult("Downloading files is disabled.");

            try
            {
                var fileName = await _fileNameResolver.ResolveFileNameAsync(path);
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
        public async Task<IActionResult> LoadDocumentPageResource([FromQuery] LoadDocumentPageResourceRequest request)
        {
            if (!_config.HtmlMode)
                return ErrorJsonResult("Loading page resources is disabled in image mode.");

            try
            {
                var fileCredentials =
                    new FileCredentials(request.Guid, request.FileType, request.Password);
                var bytes =
                    await _viewer.GetPageResourceAsync(fileCredentials, request.PageNumber, request.ResourceName);

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
            if (!_config.IsUpload)
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
            if (!_config.IsPrint)
                return ErrorJsonResult("Printing files is disabled.");

            try
            {
                var fileCredentials =
                    new FileCredentials(request.Guid, request.FileType, request.Password);

                var fileName = await _fileNameResolver.ResolveFileNameAsync(request.Guid);
                var pdfFileName = Path.ChangeExtension(fileName, ".pdf");
                var pdfFileBytes = await _viewer.GetPdfAsync(fileCredentials);

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
                var fileCredentials =
                    new FileCredentials(request.Guid, request.FileType, request.Password);
                var documentDescription =
                    await _viewer.GetDocumentInfoAsync(fileCredentials);

                var pageNumbers = GetPageNumbers(documentDescription.Pages.Count());
                var pagesData = await _viewer.GetPagesAsync(fileCredentials, pageNumbers);

                var pages = new List<PageDescription>();
                var searchTerm = await _searchTermResolver.ResolveSearchTermAsync(request.Guid);
                foreach (PageInfo pageInfo in documentDescription.Pages)
                {
                    var pageData = pagesData.FirstOrDefault(p => p.PageNumber == pageInfo.Number);
                    var pageDescription = new PageDescription
                    {
                        Width = pageInfo.Width,
                        Height = pageInfo.Height,
                        Number = pageInfo.Number,
                        SheetName = pageInfo.Name,
                        Data = pageData?.GetContent()
                    };

                    pages.Add(pageDescription);
                }

                var result = new LoadDocumentDescriptionResponse
                {
                    Guid = request.Guid,
                    FileType = documentDescription.FileType,
                    PrintAllowed = documentDescription.PrintAllowed,
                    Pages = pages,
                    SearchTerm = searchTerm
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

        private int[] GetPageNumbers(int totalPageCount)
        {
            if (_config.PreloadPageCount == 0)
                return Enumerable.Range(1, totalPageCount).ToArray();

            var pageCount =
                Math.Min(totalPageCount, _config.PreloadPageCount);

            return Enumerable.Range(1, pageCount).ToArray();
        }

        [HttpPost]
        public async Task<IActionResult> LoadDocumentPages([FromBody] LoadDocumentPagesRequest request)
        {
            try
            {
                var fileCredentials =
                    new FileCredentials(request.Guid, request.FileType, request.Password);
                var pages = await _viewer.GetPagesAsync(fileCredentials, request.Pages);
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
                var fileCredentials =
                    new FileCredentials(request.Guid, request.FileType, request.Password);
                var page = await _viewer.GetPageAsync(fileCredentials, request.Page);
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
            using HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");

            Uri uri = new Uri(url);
            string fileName = Path.GetFileName(uri.LocalPath);
            byte[] bytes = await httpClient.GetByteArrayAsync(uri);

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
