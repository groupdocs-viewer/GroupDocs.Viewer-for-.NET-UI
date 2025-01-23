using GroupDocs.Viewer.UI.Api.Models;
using GroupDocs.Viewer.UI.Api.Shared.Controllers;
using GroupDocs.Viewer.UI.Core.Configuration;
using GroupDocs.Viewer.UI.Core.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace GroupDocs.Viewer.UI.Api.NetFramework.Controllers
{
    public class ViewerController : ApiController
    {
        private readonly IViewerService _viewerService;
        private readonly Config _config;

        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented // Optional, for pretty formatting
        };

        public ViewerController(IViewerService viewerService, Config config)
        {
            _viewerService = viewerService;
            _config = config;
        }

        [HttpPost]
        public async Task<IHttpActionResult> ListDir(ListDirRequest request)
        {
            if (!_config.EnableFileBrowser)
                return ErrorJsonResult("Browsing files is disabled.");

            try
            {
                List<FileSystemItem> result = await _viewerService.ListDirAsync(request.Path);
                return Json(result, _jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> UploadFile()
        {
            if (!_config.EnableFileUpload)
                return BadRequest("Uploading files is disabled.");

            try
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[0] ?? throw new ArgumentNullException("file", "missing file.");
                string fileName = file.FileName;

                using (Stream stream = file.InputStream)
                {
                    byte[] fileBytes = new byte[file.ContentLength];
                    using MemoryStream ms = new MemoryStream();
                    int read;

                    while ((read = await stream.ReadAsync(fileBytes, 0, fileBytes.Length)) > 0)
                    {
                        await ms.WriteAsync(fileBytes, 0, count: read);
                    }

                    bool.TryParse(HttpContext.Current.Request.Form["rewrite"], out bool rewrite);
                    UploadFileResponse result = await _viewerService.UploadFileAsync(fileName, fileBytes, rewrite);
                    return Ok(result);
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> ViewerData(ViewDataRequest request)
        {
            try
            {
                ViewDataResponse result = await _viewerService.GetViewerDataAsync(request);
                return Json(result, _jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreatePages(CreatePagesRequest request)
        {
            try
            {
                List<PageData> result = await _viewerService.CreatePagesAsync(request);
                return Json(result, _jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IHttpActionResult> CreatePdf(CreatePdfRequest request)
        {
            if (!_config.EnableDownloadPdf && !_config.EnablePrint)
                return ErrorJsonResult("Creating PDF files is disabled.");

            try
            {
                CreatePdfResponse result = await _viewerService.CreatePdfAsync(request);
                return Json(result, _jsonSerializerSettings);
            }
            catch (Exception ex)
            {
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetPage([FromUri] GetPageRequest request)
        {
            try
            {
                byte[] result = await _viewerService.GetPageAsync(request);
                return new FileResult(result, "application/octet-stream", Path.GetFileName(request.File));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetThumb([FromUri] GetThumbRequest request)
        {
            try
            {
                byte[] result = await _viewerService.GetThumbAsync(request);
                return new FileResult(result, "image/jpeg", Path.GetFileName(request.File));
            }
            catch (Exception ex)
            {
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetPdf([FromUri] GetPdfRequest request)
        {
            if (!_config.EnableDownloadPdf && !_config.EnablePrint)
                return ErrorJsonResult("Creating PDF files is disabled.");

            try
            {
                byte[] result = await _viewerService.GetPdfAsync(request);
                return new FileResult(result, "application/pdf", $"{Path.GetFileNameWithoutExtension(request.File)}.pdf");
            }
            catch (Exception ex)
            {
                return ErrorJsonResult(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetResource([FromUri] GetResourceRequest request)
        {
            try
            {
                byte[] result = await _viewerService.GetResourceAsync(request);
                string contentType = request.Resource.ContentTypeFromFileName();
                return new FileResult(result, contentType, Path.GetFileName(request.File));
            }
            catch (Exception ex)
            {
                return ErrorJsonResult(ex.Message);
            }
        }

        private IHttpActionResult ErrorJsonResult(string message)
        {
            return Json(new { error = message }, _jsonSerializerSettings);
        }
    }
}
