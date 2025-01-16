using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileInfo = GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models.FileInfo;

namespace GroupDocs.Viewer.UI.Cloud.Api.Viewers
{
    internal abstract class BaseViewer : IViewer
    {
        protected readonly Config Config;
        private readonly IFileStorage _fileStorage;
        private readonly IViewerApiConnect _viewerApiConnect;
        private readonly IPageFormatter _pageFormatter;

        protected BaseViewer(
            IOptions<Config> config,
            IFileStorage fileStorage,
            IViewerApiConnect viewerApiConnect,
            IPageFormatter pageFormatter)
        {
            Config = config.Value;
            _fileStorage = fileStorage;
            _viewerApiConnect = viewerApiConnect;
            _pageFormatter = pageFormatter;
        }

        public abstract string PageExtension { get; }

        public abstract string ThumbExtension { get; }

        public abstract Page CreatePage(int pageNumber, byte[] data);

        public abstract Thumb CreateThumb(int pageNumber, byte[] data);

        public abstract ViewOptions CreatePagesViewOptions(FileInfo fileInfo);

        public abstract ViewOptions CreateThumbsViewOptions(FileInfo fileInfo);

        private ViewOptions CreatePdfViewOptions(FileInfo fileInfo)
        {
            var pdfOptions = new PdfOptions();

            Config.PdfViewOptionsSetupAction(pdfOptions);

            var viewOptions = new ViewOptions
            {
                FileInfo = fileInfo,
                ViewFormat = ViewFormat.PDF,
                RenderOptions = pdfOptions,
                OutputPath = Config.OutputFolderPath
            };

            return viewOptions;
        }

        public async Task<Page> GetPageAsync(FileCredentials fileCredentials, int pageNumber)
        {
            await UploadFileIfNotExists(fileCredentials.FilePath);

            var pages = await CreatePagesAsync(fileCredentials, new[] { pageNumber });
            var page = pages.FirstOrDefault();

            return page;
        }

        public async Task<Thumb> GetThumbAsync(FileCredentials fileCredentials, int pageNumber)
        {
            await UploadFileIfNotExists(fileCredentials.FilePath);

            var thumbs = await CreateThumbsAsync(fileCredentials, new[] { pageNumber });
            var thumb = thumbs.FirstOrDefault();

            return thumb;
        }

        public async Task<Pages> GetPagesAsync(FileCredentials fileCredentials, int[] pageNumbers)
        {
            await UploadFileIfNotExists(fileCredentials.FilePath);

            var pages = await CreatePagesAsync(fileCredentials, pageNumbers);

            return new Pages(pages);
        }

        public async Task<Thumbs> GetThumbsAsync(FileCredentials fileCredentials, int[] pageNumbers)
        {
            await UploadFileIfNotExists(fileCredentials.FilePath);

            var thumbs = await CreateThumbsAsync(fileCredentials, pageNumbers);

            return new Thumbs(thumbs);
        }

        private async Task<List<Page>> CreatePagesAsync(FileCredentials fileCredentials, int[] pagesToCreate)
        {
            var pages = new List<Page>();

            var fileInfo = CreateFileInfo(fileCredentials);
            var viewOptions = CreatePagesViewOptions(fileInfo);

            var createPagesResult = await _viewerApiConnect.CreatePagesAsync(fileInfo, pagesToCreate, viewOptions);
            if (createPagesResult.IsFailure)
                throw new Exception(createPagesResult.Message);

            foreach (var pageView in createPagesResult.Value.Pages)
            {
                var downloadResult = await _viewerApiConnect.DownloadResourceAsync(pageView,
                    Config.StorageName);

                if (downloadResult.IsFailure)
                    throw new Exception(downloadResult.Message);

                var page = CreatePage(pageView.Number, downloadResult.Value);

                if (pageView.Resources != null && pageView.Resources.Any())
                {
                    var resources = await DownloadResourcesAsync(pageView.Resources,
                        Config.StorageName);
                    resources.ForEach(resource => page.AddResource(resource));
                }

                page = await _pageFormatter.FormatAsync(fileCredentials, page);

                pages.Add(page);
            }

            if (Config.DeleteOutput)
                await _viewerApiConnect.DeleteView(fileInfo, Config.StorageName);

            return pages;
        }

        private async Task<List<Thumb>> CreateThumbsAsync(FileCredentials fileCredentials, int[] pagesToCreate)
        {
            var thumbs = new List<Thumb>();

            var fileInfo = CreateFileInfo(fileCredentials);
            var viewOptions = CreateThumbsViewOptions(fileInfo);

            var createThumbsResult = await _viewerApiConnect.CreateThumbsAsync(fileInfo, pagesToCreate, viewOptions);
            if (createThumbsResult.IsFailure)
                throw new Exception(createThumbsResult.Message);

            foreach (var thumbView in createThumbsResult.Value.Pages)
            {
                var downloadResult = await _viewerApiConnect.DownloadResourceAsync(thumbView,
                    Config.StorageName);

                if (downloadResult.IsFailure)
                    throw new Exception(downloadResult.Message);

                var thumb = CreateThumb(thumbView.Number, downloadResult.Value);
                thumbs.Add(thumb);
            }

            if (Config.DeleteOutput)
                await _viewerApiConnect.DeleteView(fileInfo, Config.StorageName);

            return thumbs;
        }

        private async Task<List<PageResource>> DownloadResourcesAsync(List<HtmlResource> htmlResources, string storageName)
        {
            List<PageResource> resources = new List<PageResource>();
            foreach (var htmlResource in htmlResources)
            {
                var result = await _viewerApiConnect.DownloadResourceAsync(htmlResource, storageName);
                if (result.IsFailure)
                    throw new Exception(result.Message);

                var resource = new PageResource(htmlResource.Name, result.Value);
                resources.Add(resource);
            }

            return resources;
        }

        public async Task<DocumentInfo> GetDocumentInfoAsync(FileCredentials fileCredentials)
        {
            await UploadFileIfNotExists(fileCredentials.FilePath);

            var fileInfo = CreateFileInfo(fileCredentials);
            var viewOptions = CreatePagesViewOptions(fileInfo);
            var result = await _viewerApiConnect.GetDocumentInfoAsync(fileInfo, viewOptions);

            if (result.IsFailure)
                throw new Exception(result.Message);

            return result.Value;
        }

        private FileInfo CreateFileInfo(FileCredentials fileCredentials)
        {
            var fileInfo = new FileInfo
            {
                FilePath = fileCredentials.FilePath,
                Password = fileCredentials.Password,
                StorageName = Config.StorageName
            };
            return fileInfo;
        }

        public async Task<byte[]> GetPdfAsync(FileCredentials fileCredentials)
        {
            await UploadFileIfNotExists(fileCredentials.FilePath);

            var fileInfo = CreateFileInfo(fileCredentials);
            var viewOptions = CreatePdfViewOptions(fileInfo);

            var result = await _viewerApiConnect
                .GetPdfFileAsync(fileInfo, viewOptions);

            if (result.IsFailure)
                throw new Exception(result.Message);

            if (Config.DeleteOutput)
                await _viewerApiConnect.DeleteView(fileInfo, Config.StorageName);

            return result.Value;
        }

        public async Task<byte[]> GetPageResourceAsync(FileCredentials fileCredentials, int pageNumber,
            string resourceName)
        {
            var page = await GetPageAsync(fileCredentials, pageNumber);
            var bytes =
                page.Resources
                    .Where(resource => resource.ResourceName == resourceName)
                    .Select(resource => resource.Data)
                    .FirstOrDefault();

            return bytes;
        }

        private async Task UploadFileIfNotExists(string filePath)
        {
            var existsResult = await _viewerApiConnect.CheckFileExistsAsync(filePath, Config.StorageName);
            if (existsResult.IsFailure)
                throw new Exception(existsResult.Message);

            if (!existsResult.Value)
            {
                var bytes = await _fileStorage.ReadFileAsync(filePath);
                var uploadResult = await _viewerApiConnect.UploadFileAsync(filePath, Config.StorageName, bytes);
                if (uploadResult.IsFailure)
                    throw new Exception(uploadResult.Message);
            }
        }
    }
}