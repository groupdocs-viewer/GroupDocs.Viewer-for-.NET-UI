# Performance Tuning Sample

This sample shows settings that make the viewer faster and the pages it sends smaller. [Program.cs](Program.cs) explains every setting, and every API request logs its time, so you can try the settings with your own documents.

## What This Sample Shows

| Setting | What it saves |
|---|---|
| `PreloadPages = 1` | The first response waits for one rendered page instead of three |
| `ExcludeFonts = true` | Fonts embedded again in every page: the viewer renders each page as a separate HTML document |
| `SpreadsheetOptions.ForSplitSheetIntoPages(100)` | Rendering a whole sheet before its first page shows: by default a sheet is one page |
| `DefaultFontName`, on Linux only | The search for a substitute of every font the server doesn't have |
| PDF: `ForRenderingByPageBreaks()` and `OptimizeSpreadsheets` | Time and size of a large sheet's PDF for download and printing |
| `AddLocalCache()` | Rendering a page again |
| `AddResponseCompression()` | Bytes sent to the browser |
| `WarmUpService` | Loading the engine, and the first use of the settings, during the first request |

`OptimizeSpreadsheets` needs GroupDocs.Viewer.UI 26.11.0 or later. Earlier versions ignore the `PdfOptimizationOptions` set with `ConfigurePdfViewOptions`.

## Prerequisites

- .NET 10 SDK or later

## Running

```bash
cd samples/GroupDocs.Viewer.UI.Sample.Performance
dotnet run
```

Open your browser at `https://localhost:5001`.

## See the Difference

- The console logs every API request with its time. The browser's developer tools show the same time as **Server Timing** under **Network** > **Timing**.
- Pages in the cache come back in a few milliseconds. Delete the `Cache` folder to measure the rendering again, and after you change a setting: the cache doesn't know the settings.
- Run `GroupDocs.Viewer.UI.Sample`, which uses the default settings, to compare. Copy `sales-10000-rows.csv` to its `Files` folder to see a whole sheet rendered as one page.

Compared with the default settings, with this sample's documents on one Windows machine. Times depend on the machine, so the table shows how much smaller and faster each request got:

| Request | Size | Time |
|---|---|---|
| `sales-10000-rows.csv`, the first page: 100 rows instead of the whole sheet | −99% | −59% |
| `basic-presentation.pptx`, all 13 slides without embedded fonts | −97% | −35% |
| `annual-review.docx`, all 6 pages without embedded fonts | −5% | −5% |
| `sales-10000-rows.csv` as PDF, with page breaks and lighter output | −53% | −21% |
| The next pages, with thumbnails rendered on request (new in 26.11.0) | | −38% to −86% |
| The first response, with thumbnails rendered on request (new in 26.11.0) | | −2% to −40% |
| A page on the wire, compressed | −91% | |
| A document viewed again, from the cache | | −97% to −99% |
| The first view after the app starts, with the warm-up | | −96% |

`WarmUpService` opens the documents with the viewer's own settings, because the first use of a setting costs time too: after this warm-up, the first view of `sales-10000-rows.csv` took 58% less time than after a warm-up with the default settings.

## Trade-offs

- **Fonts left out of the pages:** the browser draws the text with its own fonts. Where it doesn't have a document's font, the text looks different and can take a little more or less room. Keep the fonts when the exact look matters more.
- **Default font:** it replaces every font the server doesn't have, symbol fonts included, so their symbols can turn into letters. Install the symbol fonts your documents use. On Windows, where the fonts are installed, a default font made `annual-review.docx` about a quarter slower and `basic-presentation.pptx` about a tenth faster, so this sample sets it on Linux only.
- **Sheets split into pages:** a page holds 100 rows and all the columns, so a wide sheet gives wide pages. Rendering every page takes longer in total than one large page, but the first page shows at once.
- **The lighter PDF:** standard fonts such as Arial are referenced instead of embedded. For a small sheet, the PDF can take a little longer to create.
- **Thumbnails:** every thumbnail is one more render of its page, as a JPG image, and it often takes longer than the page itself: for a 100-row page of `sales-10000-rows.csv`, about 20 times as long. Since 26.11.0 the viewer renders a thumbnail when the browser asks for it, so the pages don't wait for their thumbnails. The browser still asks for the thumbnail of every page it shows, also while the thumbnails pane is closed, so set `EnableThumbnails = false` if your users don't need them.
- **Compression over HTTPS:** ASP.NET Core turns it off by default because of attacks such as BREACH, which need a secret and text an attacker controls in the same response. Check your pages before you turn it on, or compress in the proxy in front of the app.

## On Linux Servers

- Install the fonts your documents use, or fonts with the same metrics: Carlito for Calibri, Caladea for Cambria, and Liberation for Arial, Times New Roman and Courier New (`fonts-crosextra-carlito`, `fonts-crosextra-caladea` and `fonts-liberation` on Debian and Ubuntu). Text in a script that no installed font covers is drawn as empty boxes.
- Keep the font set small. The more fonts are installed, the longer the search for a substitute takes.
- One render uses about one CPU core. Plan the CPU and memory of your instances for the number of renders at the same time.
