using GroupDocs.Viewer.UI.Api.Models;
using GroupDocs.Viewer.UI.Api.Shared.Controllers;
using GroupDocs.Viewer.UI.Core.Configuration;
using GroupDocs.Viewer.UI.Core.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api.Controllers
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class ViewerController : ControllerBase
    {
        private readonly IViewerService _viewerService;
        private readonly ILogger<ViewerController> _logger;
        private readonly Config _config;

        public ViewerController(IViewerService viewerService, IOptions<Config> config, ILogger<ViewerController> logger)
        {
            _viewerService = viewerService;
            _config = config.Value;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> ListDir([FromBody] ListDirRequest request)
        {
            if (!_config.EnableFileBrowser)
                return ErrorJsonResult("Browsing files is disabled.");

            try
            {
                var result = await _viewerService.ListDirAsync(request.Path);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to list directory.");
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
                var file = Request.Form.Files[0];
                var rewrite = bool.TryParse(Request.Form["rewrite"], out var rewriteFlag) && rewriteFlag;

                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);

                var result = await _viewerService.UploadFileAsync(file.FileName, stream.ToArray(), rewrite);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload file.");
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ViewData([FromBody] ViewDataRequest request)
        {
            try
            {
                var result = await _viewerService.GetViewerDataAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve view data.");
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePages([FromBody] CreatePagesRequest request)
        {
            try
            {
                var result = await _viewerService.CreatePagesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create pages.");
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePdf([FromBody] CreatePdfRequest request)
        {
            if (!_config.EnableDownloadPdf && !_config.EnablePrint)
                return ErrorJsonResult("Creating PDF files is disabled.");

            try
            {
                var result = await _viewerService.CreatePdfAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create PDF.");
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPage([FromQuery] GetPageRequest request)
        {
            try
            {
                var result = await _viewerService.GetPageAsync(request);
                return File(result, "application/octet-stream");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve page.");
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetThumb([FromQuery] GetThumbRequest request)
        {
            try
            {
                var result = await _viewerService.GetThumbAsync(request);
                return File(result, "image/jpeg");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve thumbnail.");
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPdf([FromQuery] GetPdfRequest request)
        {
            if (!_config.EnableDownloadPdf && !_config.EnablePrint)
                return ErrorJsonResult("Creating PDF files is disabled.");

            try
            {
                var result = await _viewerService.GetPdfAsync(request);
                return File(result, "application/pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve PDF.");
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetResource([FromQuery] GetResourceRequest request)
        {
            try
            {
                var result = await _viewerService.GetResourceAsync(request);
                var contentType = request.Resource.ContentTypeFromFileName();
                return File(result, contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve resource.");
                return ErrorJsonResult(ex.Message);
            }
        }

        private IActionResult ErrorJsonResult(string message)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { error = message });
        }
    }
}
