using GroupDocs.Viewer.Options;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Sample.Performance;
using Microsoft.AspNetCore.ResponseCompression;

var builder = WebApplication.CreateBuilder(args);

var viewerType = ViewerType.HtmlWithEmbeddedResources;

// A Linux server usually doesn't have the fonts your documents use, so the engine searches the
// installed fonts for a substitute, again and again as it lays out the text. A default font ends
// that search. Where the fonts are installed, as on Windows, a default font has little to replace
// and can cost time instead, so this sample sets it on Linux only. Measure it with your documents,
// and pick an installed font that covers their languages. DejaVu Sans comes with fontconfig.
string? defaultFontName = OperatingSystem.IsLinux() ? "DejaVu Sans" : null;

builder.Services
    .AddGroupDocsViewerUI(config =>
    {
        config.RenderingMode = viewerType.ToRenderingMode();

        // 1. The first response waits for one rendered page instead of three.
        //    The viewer requests the other pages as they scroll into view.
        config.PreloadPages = 1;

        // Every thumbnail is one more render of its page, as a JPG image, and it often takes
        // longer than the page itself. The viewer renders it when the browser asks for it, so
        // the pages don't wait for their thumbnails. If your users don't need them, hide them:
        // config.EnableThumbnails = false;
    });

builder.Services
    .AddControllers()
    .AddGroupDocsViewerSelfHostApi(config =>
    {
        config.SetViewerType(viewerType);

        //Trial limitations https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/
        //Temporary license can be requested at https://purchase.groupdocs.com/temporary-license
        //config.SetLicensePath("GroupDocs.Viewer.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'

        // 2-4. The HTML pages, see ConfigureHtmlPages at the end of this file
        config.ConfigureHtmlViewOptions(ConfigureHtmlPages);

        config.ConfigurePdfViewOptions(options =>
        {
            // 5. The PDF for download and printing: pages as the sheets would print instead of one
            //    page per sheet, and lighter page content. A large sheet's PDF gets smaller and
            //    quicker to create; a small sheet's PDF can take a little longer.
            if (defaultFontName is not null)
                options.DefaultFontName = defaultFontName;
            options.SpreadsheetOptions = SpreadsheetOptions.ForRenderingByPageBreaks();
            options.PdfOptimizationOptions = new PdfOptimizationOptions { OptimizeSpreadsheets = true };
        });
    })
    .AddLocalStorage("./Files")
    // 6. Every page is rendered once and comes from the cache after that. The cache doesn't
    //    know the settings: clear the Cache folder after you change them.
    .AddLocalCache("./Cache");

// 7. Pages, view data and the viewer's scripts are text that compresses well. Compress them
//    here or in the proxy in front of the app.
builder.Services.AddResponseCompression(options =>
{
    // HTTPS responses aren't compressed by default because of attacks such as BREACH, which
    // need a secret and text an attacker controls in the same response. Check your own pages.
    options.EnableForHttps = true;
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["text/javascript", "image/svg+xml"]);
});

// 8. The first render in a process loads the engine and reads the installed fonts, and the first
//    use of a setting costs time too. WarmUpService opens one document of each type when the app
//    starts, the way the viewer does and with the same settings, so the first user doesn't wait.
builder.Services.AddHostedService(services =>
    ActivatorUtilities.CreateInstance<WarmUpService>(services, (Action<HtmlViewOptions>)ConfigureHtmlPages));

var app = builder.Build();

app.UseResponseCompression();

// Logs how long each API request took and sends it in the Server-Timing header,
// which the browser's developer tools show under Network > Timing.
app.UseServerTiming("/viewer-api");

app
    .UseRouting()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", async context =>
        {
            await context.Response.SendFileAsync("index.html");
        });
        endpoints.MapGroupDocsViewerUI(options =>
        {
            options.UIPath = "/viewer";
            options.ApiEndpoint = "/viewer-api";
        });
        endpoints.MapGroupDocsViewerApi(options =>
        {
            options.ApiPath = "/viewer-api";
        });
    });

await app.RunAsync();

// The settings of the HTML pages, used by the viewer and by WarmUpService
void ConfigureHtmlPages(HtmlViewOptions options)
{
    // 2. Fonts the server doesn't have go straight to the default font, see the top of this file.
    if (defaultFontName is not null)
        options.DefaultFontName = defaultFontName;

    // 3. Every page is a separate HTML document, so embedded fonts would be written into every
    //    page again. Without them, the browser draws the text with its own fonts.
    options.ExcludeFonts = true;

    // 4. Sheets are split into pages of 100 rows. By default a whole sheet is one page, so the
    //    first page of a large sheet takes as long as the whole sheet.
    options.SpreadsheetOptions = SpreadsheetOptions.ForSplitSheetIntoPages(100);
    options.SpreadsheetOptions.RenderGridLines = true;
    options.SpreadsheetOptions.RenderHeadings = true;
}
