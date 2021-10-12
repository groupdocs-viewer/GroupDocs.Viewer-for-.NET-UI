using System;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Caching;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.Cloud.Api.Viewers
{
    internal class PngViewer : IViewer
    {
        private readonly Config _config;
        private readonly IFileCache _fileCache;
        private readonly IViewerApiConnect _viewerApiConnect;

        public PngViewer(
            IOptions<Config> config,
            IFileCache fileCache, 
            IViewerApiConnect viewerApiConnect)
        {
            _config = config.Value;
            _fileCache = fileCache;
            _viewerApiConnect = viewerApiConnect;
        }

        public Task<Pages> RenderPagesAsync(string filePath, string password, int[] pageNumbers)
        {
            throw new System.NotImplementedException();
        }

        public Task<DocumentInfo> GetDocumentInfoAsync(string filePath, string password) =>
            _fileCache.GetValueAsync(CacheKeys.FILE_INFO_CACHE_KEY, filePath, async () =>
            {
                var result = await _viewerApiConnect
                    .GetDocumentInfoAsync(filePath, password, _config.StorageName);

                if (result.IsFailure)
                    throw new Exception(result.Message);

                return result.Value;
            });

        public Task<byte[]> CreatePdfAsync(string filePath, string password) =>
            _fileCache.GetValueAsync(CacheKeys.PDF_FILE_CACHE_KEY, filePath, async () =>
            {
                var result = await _viewerApiConnect
                    .GetPdfFileAsync(filePath, password, _config.StorageName);

                if (result.IsFailure)
                    throw new Exception(result.Message);

                return result.Value;
            });

        public Task<byte[]> GetPageResourceAsync(string filePath, string password, int pageNumber,  string resourceName) =>
            Task.FromResult(Array.Empty<byte>());
    }
}