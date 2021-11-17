using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models;
using GroupDocs.Viewer.UI.Cloud.Api.Configuration;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using Microsoft.Extensions.Options;
using FileInfo = GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Models.FileInfo;

namespace GroupDocs.Viewer.UI.Cloud.Api.Viewers
{
    internal abstract class BaseViewer : IViewer
    {
        protected readonly Config Config;
        private readonly IFileStorage _fileStorage;
        private readonly IViewerApiConnect _viewerApiConnect;

        protected BaseViewer(
            IOptions<Config> config,
            IFileStorage fileStorage,
            IViewerApiConnect viewerApiConnect)
        {
            Config = config.Value;
            _fileStorage = fileStorage;
            _viewerApiConnect = viewerApiConnect;
        }

        public abstract string PageExtension { get; }

        public abstract Page CreatePage(int pageNumber, byte[] data);

        public abstract ViewOptions CreateViewOptions(FileInfo fileInfo);

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

        public async Task<Page> GetPageAsync(string filePath, string password, int pageNumber)
        {
            await UploadFileIfNotExists(filePath);

            var pages = await CreatePagesAsync(filePath, password, new[] { pageNumber });
            var page = pages.FirstOrDefault();

            return page;
        }

        public async Task<Pages> GetPagesAsync(string filePath, string password, int[] pageNumbers)
        {
            await UploadFileIfNotExists(filePath);

            var pages = await CreatePagesAsync(filePath, password, pageNumbers);

            return new Pages(pages);
        }

        private async Task<List<Page>> CreatePagesAsync(string filePath, string password, int[] pagesToCreate)
        {
            var pages = new List<Page>();

            var fileInfo = CreateFileInfo(filePath, password);
            var viewOptions = CreateViewOptions(fileInfo);

            var createPagesResult = await _viewerApiConnect.CreatePagesAsync(fileInfo, pagesToCreate, viewOptions);
            if (createPagesResult.IsFailure)
                throw new Exception(createPagesResult.Message);

            foreach (var pageView in createPagesResult.Value.Pages)
            {
                var downloadResult = await _viewerApiConnect.DownloadResourceAsync(pageView,
                    Config.StorageName);

                if (downloadResult.IsFailure)
                    throw new Exception(downloadResult.Message);

                var bytes = downloadResult.Value;
                var page = CreatePage(pageView.Number, bytes);

                if (pageView.Resources != null && pageView.Resources.Any())
                {
                    var resources = await DownloadResourcesAsync(pageView.Resources,
                        Config.StorageName);

                    resources.ForEach(resource => page.AddResource(resource));
                }

                pages.Add(page);
            }

            if (Config.DeleteOutput)
                await _viewerApiConnect.DeleteView(fileInfo, Config.StorageName);

            return pages;
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

        public async Task<DocumentInfo> GetDocumentInfoAsync(string filePath, string password)
        {
            await UploadFileIfNotExists(filePath);

            var fileInfo = CreateFileInfo(filePath, password);
            var viewOptions = CreateViewOptions(fileInfo);
            var result =
                await _viewerApiConnect.GetDocumentInfoAsync(fileInfo, viewOptions);

            if (result.IsFailure)
                throw new Exception(result.Message);

            return result.Value;
        }

        private FileInfo CreateFileInfo(string filePath, string password)
        {
            var fileInfo = new FileInfo
            {
                FilePath = filePath,
                Password = password,
                StorageName = Config.StorageName
            };
            return fileInfo;
        }

        public async Task<byte[]> GetPdfAsync(string filePath, string password)
        {
            await UploadFileIfNotExists(filePath);

            var fileInfo = CreateFileInfo(filePath, password);
            var viewOptions = CreatePdfViewOptions(fileInfo);

            var result = await _viewerApiConnect
                .GetPdfFileAsync(fileInfo, viewOptions);

            if (result.IsFailure)
                throw new Exception(result.Message);

            if (Config.DeleteOutput)
                await _viewerApiConnect.DeleteView(fileInfo, Config.StorageName);

            return result.Value;
        }

        public async Task<byte[]> GetPageResourceAsync(string filePath, string password, int pageNumber,
            string resourceName)
        {
            var page = await GetPageAsync(filePath, password, pageNumber);
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