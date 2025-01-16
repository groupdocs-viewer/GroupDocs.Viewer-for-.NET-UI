using GroupDocs.Viewer.UI.Api.Models;
using GroupDocs.Viewer.UI.Api.Utils;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Configuration;
using GroupDocs.Viewer.UI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api.Shared.Controllers;

public class ViewerService : IViewerService
{
    private readonly IViewer _viewer;
    private readonly IFileStorage _fileStorage;
    private readonly IApiUrlBuilder _apiUrlBuilder;
    private readonly Config _config;
    private readonly ISearchTermResolver _searchTermResolver;
    public ViewerService(
        IViewer viewer,
        IFileStorage fileStorage,
        IApiUrlBuilder apiUrlBuilder,
        Config config,
        ISearchTermResolver searchTermResolver)
    {
        _viewer = viewer;
        _fileStorage = fileStorage;
        _apiUrlBuilder = apiUrlBuilder;
        _config = config;
        _searchTermResolver = searchTermResolver;
    }

    public async Task<List<FileSystemItem>> ListDirAsync(string path)
    {
        var files = await _fileStorage.ListDirsAndFilesAsync(path);
        return files.Select(f => new FileSystemItem(f.FilePath, f.FilePath, f.IsDirectory, f.Size)).ToList();
    }

    public async Task<UploadFileResponse> UploadFileAsync(string fileName, byte[] fileData, bool rewrite)
    {
        var filePath = await _fileStorage.WriteFileAsync(fileName, fileData, rewrite);
        return new UploadFileResponse(filePath);
    }

    public async Task<ViewDataResponse> GetViewerDataAsync(ViewDataRequest request)
    {
        var file = new FileCredentials(request.File, request.FileType, request.Password);
        var docInfo = await _viewer.GetDocumentInfoAsync(file);
        var pagesToCreate = GetPagesToCreate(docInfo.TotalPagesCount, _config.PreloadPages);
        var pages = await CreateViewDataPages(file, docInfo, pagesToCreate);
        var searchTerm = await _searchTermResolver.ResolveSearchTermAsync(request.File);
        return new ViewDataResponse
        {
            File = request.File,
            FileType = docInfo.FileType,
            CanPrint = docInfo.PrintAllowed,
            SearchTerm = searchTerm,
            Pages = pages
        };
    }

    public async Task<List<PageData>> CreatePagesAsync(CreatePagesRequest request)
    {
        var file = new FileCredentials(request.File, request.FileType, request.Password);
        var docInfo = await _viewer.GetDocumentInfoAsync(file);
        return await CreatePagesAndThumbs(file, docInfo, request.Pages);
    }

    public async Task<CreatePdfResponse> CreatePdfAsync(CreatePdfRequest request)
    {
        var file = new FileCredentials(request.File, request.FileType, request.Password);
        await _viewer.GetPdfAsync(file);
        return new CreatePdfResponse { PdfUrl = _apiUrlBuilder.BuildPdfUrl(request.File) };
    }

    public async Task<byte[]> GetPageAsync(GetPageRequest request)
    {
        var file = new FileCredentials(request.File);
        var page = await _viewer.GetPageAsync(file, request.Page);
        return page.PageData;
    }

    public async Task<byte[]> GetThumbAsync(GetThumbRequest request)
    {
        var file = new FileCredentials(request.File);
        var thumb = await _viewer.GetThumbAsync(file, request.Page);
        return thumb.ThumbData;
    }

    public async Task<byte[]> GetPdfAsync(GetPdfRequest request)
    {
        var file = new FileCredentials(request.File);
        return await _viewer.GetPdfAsync(file);
    }

    public async Task<byte[]> GetResourceAsync(GetResourceRequest request)
    {
        var file = new FileCredentials(request.File);
        return await _viewer.GetPageResourceAsync(file, request.Page, request.Resource);
    }

    private static int[] GetPagesToCreate(int totalPageCount, int preloadPageCount)
    {
        if (preloadPageCount == 0)
            return Enumerable.Range(1, totalPageCount).ToArray();

        return Enumerable.Range(1, Math.Min(totalPageCount, preloadPageCount)).ToArray();
    }

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
            var pageUrl = _apiUrlBuilder.BuildPageUrl(file.FilePath, page.Number, _viewer.PageExtension);
            var thumbUrl = _apiUrlBuilder.BuildThumbUrl(file.FilePath, page.Number, _viewer.ThumbExtension);

            var pageData = _config.EnableThumbnails
                ? new PageData(page.Number, page.Width, page.Height, pageUrl, thumbUrl)
                : new PageData(page.Number, page.Width, page.Height, pageUrl);

            pages.Add(pageData);
        }

        return pages;
    }

    private async Task<List<PageData>> CreateViewDataPages(FileCredentials file, DocumentInfo docInfo, int[] pagesToCreate)
    {
        await _viewer.GetPagesAsync(file, pagesToCreate.ToArray());

        if (_config.EnableThumbnails)
        {
            await _viewer.GetThumbsAsync(file, pagesToCreate.ToArray());
        }

        var pages = new List<PageData>();
        foreach (var page in docInfo.Pages)
        {
            var isPageCreated = pagesToCreate.Contains(page.Number);
            if (isPageCreated)
            {
                var pageUrl = _apiUrlBuilder.BuildPageUrl(file.FilePath, page.Number, _viewer.PageExtension);
                var thumbUrl = _apiUrlBuilder.BuildThumbUrl(file.FilePath, page.Number, _viewer.PageExtension);

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
}