using System.Diagnostics;
using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.UI.Api.Configuration;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;
using GroupDocs.Viewer.UI.SelfHost.Api.Licensing;
using Microsoft.Extensions.Options;
using UIConfig = GroupDocs.Viewer.UI.Core.Configuration.Config;

namespace GroupDocs.Viewer.UI.Sample.Performance;

/// <summary>
/// Sets the license and opens one document of each type when the app starts, the way the viewer
/// opens a document: the view info, the first page as HTML and its thumbnail as a JPG image.
/// </summary>
/// <remarks>
/// The first render in a process is much slower than the ones after it: the engine loads and
/// compiles its code and reads the installed fonts, once for every document type, and the first
/// use of a setting, such as sheets split into pages, costs time too. This service pays for that
/// before the first user comes, with the viewer's own settings. It renders outside the viewer's
/// cache, so it warms the engine up on every start, also when the cache already holds the pages.
/// </remarks>
public sealed class WarmUpService(
    Action<HtmlViewOptions> configureHtmlPages,
    IOptions<UIConfig> uiConfig,
    IFileStorage fileStorage,
    IViewerLicenseManager licenseManager,
    ILogger<WarmUpService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Let the app start listening; the warm-up runs in the background
        await Task.Yield();

        try
        {
            licenseManager.SetLicense();

            var entries = await fileStorage.ListDirsAndFilesAsync(string.Empty, stoppingToken);
            var oneFilePerType = entries
                .Where(entry => !entry.IsDirectory)
                .GroupBy(entry => Path.GetExtension(entry.FilePath).ToLowerInvariant())
                .Select(files => files.First());

            foreach (var file in oneFilePerType)
                await OpenAsync(file, stoppingToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // A failed warm-up must not stop the app
            logger.LogWarning(ex, "The warm-up stopped");
        }
    }

    private async Task OpenAsync(FileSystemEntry file, CancellationToken stoppingToken)
    {
        var started = Stopwatch.GetTimestamp();
        try
        {
            var bytes = await fileStorage.ReadFileAsync(file.FilePath, stoppingToken);
            var loadOptions = new LoadOptions(FileType.FromExtension(Path.GetExtension(file.FilePath)));
            using var viewer = new Viewer(new MemoryStream(bytes), loadOptions);

            var pageOptions = HtmlViewOptions.ForEmbeddedResources(_ => new MemoryStream(), (_, page) => page.Dispose());
            configureHtmlPages(pageOptions);
            viewer.GetViewInfo(ViewInfoOptions.FromHtmlViewOptions(pageOptions));
            viewer.View(pageOptions, stoppingToken, 1);

            if (uiConfig.Value.EnableThumbnails)
            {
                var thumbOptions = new JpgViewOptions(_ => new MemoryStream(), (_, thumb) => thumb.Dispose())
                {
                    DefaultFontName = pageOptions.DefaultFontName,
                    SpreadsheetOptions = pageOptions.SpreadsheetOptions,
                    ExtractText = false,
                    Quality = ThumbSettings.ThumbQuality,
                    MaxWidth = ThumbSettings.MaxThumbWidth,
                    MaxHeight = ThumbSettings.MaxThumbHeight,
                };
                viewer.View(thumbOptions, stoppingToken, 1);
            }

            logger.LogInformation("Warmed up with {File} in {Elapsed:0} ms",
                file.FileName, Stopwatch.GetElapsedTime(started).TotalMilliseconds);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // For example, a password-protected document
            logger.LogInformation("Skipped {File} in the warm-up: {Reason}", file.FileName, ex.Message);
        }
    }
}
