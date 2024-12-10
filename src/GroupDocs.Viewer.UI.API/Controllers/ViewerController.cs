using GroupDocs.Viewer.UI.Api.Models;
using GroupDocs.Viewer.UI.Api.Utils;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Configuration;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ViewerController : ControllerBase
    {
        private readonly IViewer _viewer;
        private readonly IFileStorage _fileStorage;
        private readonly IFileNameResolver _fileNameResolver;
        private readonly ISearchTermResolver _searchTermResolver;
        private readonly IApiUrlBuilder _apiUrlBuilder;
        private readonly ILogger<ViewerController> _logger;
        private readonly Config _config;

        public ViewerController(
            IViewer viewer,
            IFileStorage fileStorage,
            IFileNameResolver fileNameResolver,
            ISearchTermResolver searchTermResolver,
            IOptions<Config> config,
            IApiUrlBuilder apiUrlBuilder,
            ILogger<ViewerController> logger)
        {
            _fileStorage = fileStorage;
            _fileNameResolver = fileNameResolver;
            _searchTermResolver = searchTermResolver;
            _apiUrlBuilder = apiUrlBuilder;
            _viewer = viewer;
            _logger = logger;
            _config = config.Value;
        }

        [HttpPost]
        public async Task<IActionResult> ListDir([FromBody] ListDirRequest request)
        {
            if (!_config.EnableFileBrowser)
                return ErrorJsonResult("Browsing files is disabled.");

            try
            {
                var files =
                    await _fileStorage.ListDirsAndFilesAsync(request.Path);

                var result = files
                    .Select(entity => new FileSystemItem(entity.FilePath, entity.FilePath, entity.IsDirectory, entity.Size))
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load file tree.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            if (!_config.EnableFileUpload)
                return ErrorJsonResult("Uploading files is disabled.");

            try
            {
                var (fileName, bytes) = await ReadOrDownloadFile();
                bool.TryParse(Request.Form["rewrite"], out var rewrite);

                var filePath = await _fileStorage.WriteFileAsync(fileName, bytes, rewrite);

                var result = new UploadFileResponse(filePath);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload document.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ViewData([FromBody] ViewDataRequest request)
        {
            try
            {
                var file = new FileCredentials(request.File, request.FileType, request.Password);

                var docInfo = await _viewer.GetDocumentInfoAsync(file);
                var pagesToCreate = GetPagesToCreate(docInfo.TotalPagesCount, _config.PreloadPages);

                var pages = await CreateViewDataPages(file, docInfo, pagesToCreate);

                var searchTerm = await _searchTermResolver.ResolveSearchTermAsync(request.File);
                var response = new ViewDataResponse
                {
                    File = request.File,
                    FileType = docInfo.FileType,
                    CanPrint = docInfo.PrintAllowed,
                    SearchTerm = searchTerm,
                    Pages = pages
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                {
                    var message = string.IsNullOrEmpty(request.Password)
                            ? "Password Required"
                            : "Incorrect Password";

                    return ForbiddenResult(message);
                }

                _logger.LogError(ex, "Failed to read document description.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePages([FromBody] CreatePagesRequest request)
        {
            try
            {
                var file = new FileCredentials(request.File, request.FileType, request.Password);
                var docInfo = await _viewer.GetDocumentInfoAsync(file);

                var pages = await CreatePagesAndThumbs(file, docInfo, request.Pages);

                return Ok(pages);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                {
                    var message = string.IsNullOrEmpty(request.Password)
                        ? "Password Required"
                        : "Incorrect Password";

                    return ForbiddenResult(message);
                }

                _logger.LogError(ex, "Failed to retrieve document pages.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePdf([FromBody] CreatePdfRequest request)
        {
            if (!_config.EnablePrint)
                return ErrorJsonResult("Printing files is disabled.");

            if (!_config.EnableDownloadPdf)
                return ErrorJsonResult("Downloading PDF files is disabled.");

            try
            {
                var fileCredentials = new FileCredentials(request.File, request.FileType, request.Password);

                await _viewer.GetPdfAsync(fileCredentials);

                var response = new CreatePdfResponse
                {
                    PdfUrl = _apiUrlBuilder.BuildPdfUrl(request.File)
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                {
                    var message = string.IsNullOrEmpty(request.Password)
                        ? "Password Required"
                        : "Incorrect Password";

                    return ForbiddenResult(message);
                }

                _logger.LogError(ex, "Failed to create PDF file.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPage([FromQuery] GetPageRequest request)
        {
            try
            {
                var fileCredentials = new FileCredentials(request.File);
                var page = await _viewer.GetPageAsync(fileCredentials, request.Page);

                return File(page.PageData, page.ContentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve document page.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetThumb([FromQuery] GetThumbRequest request)
        {
            try
            {
                var file = new FileCredentials(request.File);
                var thumb = await _viewer.GetThumbAsync(file, request.Page);

                return File(thumb.ThumbData, thumb.ContentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve document thumb.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPdf([FromQuery] GetPdfRequest request)
        {
            if (!_config.EnablePrint)
                return ErrorJsonResult("Printing files is disabled.");

            if (!_config.EnableDownloadPdf)
                return ErrorJsonResult("Downloading PDF files is disabled.");

            try
            {
                var fileCredentials = new FileCredentials(request.File);

                var fileName = await _fileNameResolver.ResolveFileNameAsync(request.File);
                var pdfFileName = Path.ChangeExtension(fileName, ".pdf");
                var pdfFileBytes = await _viewer.GetPdfAsync(fileCredentials);

                return File(pdfFileBytes, "application/pdf", pdfFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve PDF file.");

                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetResource([FromQuery] GetResourceRequest request)
        {
            if (_config.RenderingMode != RenderingMode.Html)
                return ErrorJsonResult("Loading page resources is disabled in image mode.");

            try
            {
                var fileCredentials = new FileCredentials(request.File);
                var bytes =
                    await _viewer.GetPageResourceAsync(fileCredentials, request.Page, request.Resource);

                if (bytes.Length == 0)
                    return NotFound($"Resource {request.Resource} was not found");

                var contentType = request.Resource.ContentTypeFromFileName();

                return File(bytes, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load document page resource.");

                return ErrorJsonResult(ex.Message);
            }
        }

        // NOTE: This method returns all of the pages including created and not
        private async Task<List<PageData>> CreateViewDataPages(FileCredentials file, DocumentInfo docInfo, int[] pagesToCreate)
        {
            await _viewer.GetPagesAsync(file, pagesToCreate);

            if (_config.EnableThumbnails)
            {
                await _viewer.GetThumbsAsync(file, pagesToCreate);
            }

            var pages = new List<PageData>();
            foreach (PageInfo page in docInfo.Pages)
            {
                var isPageCreated = pagesToCreate.Contains(page.Number);
                if (isPageCreated)
                {
                    var pageUrl = _apiUrlBuilder.BuildPageUrl(file.FilePath, page.Number);
                    var thumbUrl = _apiUrlBuilder.BuildThumbUrl(file.FilePath, page.Number);

                    var pageData = _config.EnableThumbnails
                        ? new PageData(page.Number, page.Width, page.Height, pageUrl, thumbUrl)
                        : new PageData(page.Number, page.Width, page.Height, pageUrl);

                    pages.Add(pageData);
                }
                else
                {
                    pages.Add(new PageData(page.Number, page.Width, page.Height));
                }
            }

            return pages;
        }

        // NOTE: This method returns only created pages
        private async Task<List<PageData>> CreatePagesAndThumbs(FileCredentials file, DocumentInfo docInfo, int[] pagesToCreate)
        {
            await _viewer.GetPagesAsync(file, pagesToCreate);

            if (_config.EnableThumbnails)
            {
                await _viewer.GetThumbsAsync(file, pagesToCreate);
            }

            var pages = new List<PageData>();
            foreach (int pageNumber in pagesToCreate)
            {
                var page = docInfo.Pages.First(p => p.Number == pageNumber);
                var pageUrl = _apiUrlBuilder.BuildPageUrl(file.FilePath, page.Number);
                var thumbUrl = _apiUrlBuilder.BuildThumbUrl(file.FilePath, page.Number);

                var pageData = _config.EnableThumbnails
                    ? new PageData(page.Number, page.Width, page.Height, pageUrl, thumbUrl)
                    : new PageData(page.Number, page.Width, page.Height, pageUrl);

                pages.Add(pageData);
            }

            return pages;
        }

        private int[] GetPagesToCreate(int totalPageCount, int preloadPageCount)
        {
            if (preloadPageCount == 0)
                return Enumerable.Range(1, totalPageCount).ToArray();

            var pageCount =
                Math.Min(totalPageCount, preloadPageCount);

            return Enumerable.Range(1, pageCount).ToArray();
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
            StatusCode(StatusCodes.Status500InternalServerError, message);

        private IActionResult ForbiddenResult(string message) =>
        new ObjectResult(new { error = message })
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }
}
