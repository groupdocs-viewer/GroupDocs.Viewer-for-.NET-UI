using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Cloud.Api.Viewers
{
    internal class HtmlStaticDataViewer : IViewer
    {
        public Task<Pages> RenderPagesAsync(string filePath, string password, int[] pageNumbers)
        {
            var pageTemplate = @"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset=utf-8>
                    <title>Page</title>
                    <link rel=""stylesheet"" media=""all"" href=""/viewer-api/LoadDocumentPageResource?""></head>
                <body>
                    <div style=""height: 100%;display: grid;color:gray"">
                        <div style=""font-size: 10vw;margin: auto;text-align: center;"">
                            Page {0}
                        </div>
                    </div>
                </body>
                </html>
            ";

            var pages = pageNumbers.Select(pageNumber => new Page(pageNumber, string.Format(pageTemplate, pageNumber)));
            var result = new Pages(pages);

            return Task.FromResult(result);
        }

        public Task<DocumentInfo> GetDocumentInfoAsync(string filePath, string password)
        {
            var documentInfo = new DocumentInfo
            {
                PrintAllowed = true,
                Pages = Enumerable.Range(1, 5).Select(pageNumber => new PageInfo
                {
                    Number = pageNumber,
                    Width = 800,
                    Height = 800,
                    Name = $"Page {pageNumber}"
                })
            };

            return Task.FromResult(documentInfo);
        }

        public Task<byte[]> CreatePdfAsync(string filePath, string password)
        {
            var bytes = Encoding.UTF8.GetBytes(@"
%PDF-1.4
1 0 obj
<< /Type /Catalog /Outlines 2 0 R /Pages 3 0 R >>
endobj
2 0 obj
<< /Type Outlines /Count 0 >>
endobj
3 0 obj
<< /Type /Pages /Kids [4 0 R] /Count 1 >>
endobj
4 0 obj
<< /Type /Page /Parent 3 0 R /MediaBox [0 0 612 792] /Contents 5 0 R /Resources << /ProcSet 6 0 R >> >>
endobj
5 0 obj
<< /Length 35 >>
stream
… Page-marking operators …
endstream 
endobj
6 0 obj
[/PDF]
endobj
xref
0 7
0000000000 65535 f 
0000000009 00000 n 
0000000074 00000 n 
0000000119 00000 n 
0000000176 00000 n 
0000000295 00000 n 
0000000376 00000 n 
trailer 
<< /Size 7 /Root 1 0 R >>
startxref
394
%%EOF
");

            return Task.FromResult(bytes);
        }

        public Task<byte[]> GetPageResourceAsync(string filePath, string password, int pageNumber, string resourceName)
        {
            var css = @"
                html {
                    background-color: red;
                }
            ";

            var bytes = Encoding.UTF8.GetBytes(css);

            return Task.FromResult(bytes);
        }
    }
}