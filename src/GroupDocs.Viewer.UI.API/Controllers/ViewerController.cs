using GroupDocs.Viewer.UI.Api.Models;
using GroupDocs.Viewer.UI.Api.Responses;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using static System.Boolean;

namespace GroupDocs.Viewer.UI.Api.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ViewerController : ControllerBase
    {
        private readonly IFileStorage _fileStorage;
        private readonly IFileNameResolver _fileNameResolver;
        private readonly IViewer _viewer;
        private readonly ILogger<ViewerController> _logger;
        private ViewerConfiguration _configuration;

        public ViewerController(IFileStorage fileStorage,
            IFileNameResolver fileNameResolver,
            IViewer viewer,
            IOptionsMonitor<ViewerConfiguration> configMonitor,
            ILogger<ViewerController> logger)
        {
            _fileStorage = fileStorage;
            _fileNameResolver = fileNameResolver;
            _viewer = viewer;
            _logger = logger;
            _configuration = configMonitor.CurrentValue;

            // Subscribe to configuration changes
            configMonitor.OnChange(configuration => _configuration = configuration);
        }

        [HttpGet]
        public IActionResult LoadConfig()
        {
            return Ok(_configuration);
        }

        [HttpPost]
        public async Task<IActionResult> LoadFileTree([FromBody] LoadFileTreeRequest request)
        {
            if (!_configuration.Browse)
            {
                return BadRequest("Browsing files is disabled.");
            }

            try
            {
                var files =
                    await _fileStorage.ListDirsAndFilesAsync(request.Path);

                var result = files
                    .Select(entity => new FileDescription(entity.FilePath, entity.FilePath, entity.IsDirectory, entity.Size))
                    .ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load file tree.");

                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument([FromForm(Name = "files")] IFormFile file, [FromForm(Name = "url")] string url, CancellationToken cancellationToken)
        {
            if (!_configuration.Upload)
                return BadRequest("Uploading files is disabled.");

            try
            {
                (string fileName, byte[] bytes) fileTuple;
                if (string.IsNullOrWhiteSpace(url))
                {
                    fileTuple = await DownloadFileAsync(url);

                }
                else
                {
                    using var stream = new MemoryStream();
                    await file.CopyToAsync(stream, cancellationToken);
                    fileTuple = (file.FileName, stream.ToArray());
                }


                var filePath = await _fileStorage.WriteFileAsync(fileTuple.fileName, fileTuple.bytes, TryParse(Request.Form["rewrite"], out var rewrite) && rewrite);
                var result = new UploadFileResponse { FileName = filePath, FolderName = string.Empty };
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload document.");

                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoadDocumentDescription([FromBody] LoadDocumentDescriptionRequest request)
        {
            try
            {
                var fileCredentials =
                    new FileCredentials(request.Guid, request.Password);
                var documentDescription =
                    await _viewer.GetDocumentInfoAsync(fileCredentials);

                var pageNumbers = GetPageNumbers(documentDescription.Pages.Count());
                var pagesData = await _viewer.GetPagesAsync(fileCredentials, pageNumbers);

                var pages = documentDescription.Pages.Select(pageInfo => new PageDescription
                {
                    Width = pageInfo.Width,
                    Height = pageInfo.Height,
                    Number = pageInfo.PageNumber,
                    SheetName = pageInfo.PageName,
                    HtmlData = _configuration.HtmlMode ? pagesData.FirstOrDefault(a => a.PageNumber == pageInfo.PageNumber)?.GetContent() : null,
                    Data = $"storage/{request.Guid}/{pageInfo.PageNumber}.html"
                })
                    .ToList();

                var result = new DocumentDescriptionResponse
                {
                    Guid = request.Guid,
                    PrintAllowed = documentDescription.PrintingAllowed,
                    Pages = pages,
                    TotalPagesCount = pages.Count
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                {
                    var message = string.IsNullOrEmpty(request.Password)
                            ? "Password Required"
                            : "Incorrect Password";

                    return Forbid(message);
                }

                _logger.LogError(ex, "Failed to read document description.");

                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LoadDocumentPages([FromBody] DocumentPagesRequest request)
        {
            try
            {
                var fileCredentials =
                    new FileCredentials(request.Guid, request.Password);
                var pages = await _viewer.GetPagesAsync(fileCredentials, request.Pages);
                var pageContents = pages
                    .Select(page => new PageDescription
                    {
                        Number = page.PageNumber,
                        HtmlData = _configuration.HtmlMode ? page.GetContent() : null,
                        Data = $"storage/{request.Guid}/{page.PageNumber}.html"
                    });

                return Ok(pageContents);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                {
                    var message = string.IsNullOrEmpty(request.Password)
                        ? "Password Required"
                        : "Incorrect Password";

                    return NotFound(message);
                }

                _logger.LogError(ex, "Failed to retrieve document pages.");

                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadDocument([FromQuery] string path)
        {
            if (!_configuration.Download)
                return BadRequest("Downloading files is disabled.");

            try
            {
                var fileName = await _fileNameResolver.ResolveFileNameAsync(path);
                var bytes = await _fileStorage.ReadFileAsync(path);

                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download a document.");

                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> PrintPdf([FromBody] PrintPdfRequest request)
        {
            if (!_configuration.Print)
                return BadRequest("Printing files is disabled.");

            try
            {
                var fileCredentials =
                    new FileCredentials(request.Guid, request.Password);

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

                    return NotFound(message);
                }

                _logger.LogError(ex, "Failed to create PDF file.");

                return NotFound(ex.Message);
            }
        }

        private int[] GetPageNumbers(int totalPageCount)
        {
            if (_configuration.PreloadPageCount == 0)
                return Enumerable.Range(1, totalPageCount).ToArray();

            var pageCount =
                Math.Min(totalPageCount, _configuration.PreloadPageCount);

            return Enumerable.Range(1, pageCount).ToArray();
        }



        private static async Task<(string, byte[])> DownloadFileAsync(string url)
        {
            using HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");

            Uri uri = new Uri(url);
            string fileName = Path.GetFileName(uri.LocalPath);
            byte[] bytes = await httpClient.GetByteArrayAsync(uri);

            return (fileName, bytes);
        }
    }
}
