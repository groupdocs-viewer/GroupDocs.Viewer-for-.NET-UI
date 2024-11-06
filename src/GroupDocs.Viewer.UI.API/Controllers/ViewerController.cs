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
        public async Task<IActionResult> UploadDocument([FromForm] IFormFile file, [FromForm(Name = "url")] string url, CancellationToken cancellationToken)
        {
            if (!_configuration.Upload)
                return BadRequest("Uploading files is disabled.");

            try
            {
                (string fileName, byte[] bytes) fileTuple;
                if (!string.IsNullOrWhiteSpace(url))
                {
                    fileTuple = await DownloadFileAsync(url);

                }
                else
                {
                    using MemoryStream stream = new();
                    await file.CopyToAsync(stream, cancellationToken);
                    fileTuple = (file.FileName, stream.ToArray());
                }


                string filePath = await _fileStorage.WriteFileAsync(fileTuple.fileName, fileTuple.bytes, TryParse(Request.Form["rewrite"], out bool rewrite) && rewrite);
                UploadFileResponse result = new() { FileName = filePath, FolderName = string.Empty };
                return this.Ok(result);
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
                FileCredentials fileCredentials = new(request.Guid, request.Password);
                DocumentInfo documentDescription =
                    await _viewer.GetDocumentInfoAsync(fileCredentials);
                var pages = documentDescription.Pages.Select(pageInfo => new PageDescription
                {
                    Width = pageInfo.Width,
                    Height = pageInfo.Height,
                    Number = pageInfo.PageNumber,
                    SheetName = pageInfo.PageName,
                    Data = $"storage/page/{request.Guid}/{pageInfo.PageNumber}",
                    //Thumbnail = $"storage/thumbnail/{request.Guid}/{pageInfo.PageNumber}.png"
                })
                    .ToList();

                DocumentDescriptionResponse result = new()
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
                    string message = string.IsNullOrEmpty(request.Password)
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
                FileCredentials fileCredentials = new(request.Guid, request.Password);
                Pages pages = await _viewer.GetPagesAsync(fileCredentials, request.Pages);
                var pageContents = pages
                    .Select(page => new PageDescription
                    {
                        Number = page.PageNumber,
                        Data = $"storage/page/{request.Guid}/{page.PageNumber}.html",
                        //Thumbnail = $"storage/thumbnail/{request.Guid}/{page.PageNumber}.png"
                    });

                return Ok(pageContents);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                {
                    string message = string.IsNullOrEmpty(request.Password)
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
                string fileName = await _fileNameResolver.ResolveFileNameAsync(path);
                byte[] bytes = await _fileStorage.ReadFileAsync(path);

                return File(bytes, "application/octet-stream", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download a document.");

                return NotFound(ex.Message);
            }
        }
        [HttpPost]
        public IActionResult CreatePdf([FromBody] CreatePdfFileRequest request)
        {
            return this.Ok(new CreatePdfFileResponse() { DownloadUrl = $"storage/pdf/{request.Guid}/{Path.GetFileNameWithoutExtension(request.Guid)}.pdf" });
        }

        [HttpPost]
        public async Task<IActionResult> PrintPdf([FromBody] PrintPdfRequest request)
        {
            if (!_configuration.Print)
                return BadRequest("Printing files is disabled.");

            try
            {
                FileCredentials fileCredentials = new(request.Guid, request.Password);

                string fileName = await _fileNameResolver.ResolveFileNameAsync(request.Guid);
                string pdfFileName = Path.ChangeExtension(fileName, ".pdf");
                byte[] pdfFileBytes = await _viewer.GetPdfAsync(fileCredentials);

                return File(pdfFileBytes, "application/pdf", pdfFileName);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("password", StringComparison.InvariantCultureIgnoreCase))
                {
                    string message = string.IsNullOrEmpty(request.Password)
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

            int pageCount =
                Math.Min(totalPageCount, _configuration.PreloadPageCount);

            return Enumerable.Range(1, pageCount).ToArray();
        }



        private static async Task<(string, byte[])> DownloadFileAsync(string url)
        {
            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");

            Uri uri = new(url);
            string fileName = Path.GetFileName(uri.LocalPath);
            byte[] bytes = await httpClient.GetByteArrayAsync(uri);

            return (fileName, bytes);
        }
    }
}
