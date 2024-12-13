using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.SelfHost.Api.Viewers
{
    public class HtmlStaticDataViewer : IViewer
    {
        const string PAGE_TEMPLATE = @"
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

        const string THUMB_IN_BASE64 =
            "iVBORw0KGgoAAAANSUhEUgAAAdAAAAJYEAAAAACnrTS3AAAACXBIWXMAAAsSAAALEgHS3X78AAAgAElEQVR42u3dZ3Qcx4En8K6ejJwjEYgcCTCAUSQYRMk0tZZEBdKSbIm21z77vd27e7v39d69u/d867fn+7Ber7RaW9Lakm1ZFCVKFMWcwQwQgQCIQGQSOQwmh+66D711NQNgEpIp8P/7YAPUYKa7p/9doauqCaUCADymRBwCAAQUABBQAAQUABBQAEBAARBQAEBAARBQAEBAAQABBUBAAQABBUBAAQABBQAEFAABBQAEFAABBQAEFAAQUAAEFAAQUAAEFAAQUABAQAEQUABAQAEQUABAQAEAAQVAQAEAAQVAQAEAAQUABBQAAQUABBQAAQUABBQAEFAABBQAEFAABBQAEFAAQEABEFAAQEABEFAAQEABAAEFQEABAAEFQEABAAEFAAQUAAEFAAQUAAEFAAQUABBQAAQUABBQAAQUABBQAEBAARBQAEBAARBQAEBAAQABBUBAAQABBUBAAQABBQAEFAABBQAEFAABBQAEFAAQUAAEFAAQUAAEFAAQUABAQAEQUABAQAEQUABAQAEAAQVAQAEAAQVAQAEAAQUABBQAAQUABBQAAQUABBQAEFAABBQAEFAABBQAEFAAQEABEFAAQEABEFAAQEABAAEFQEABAAEFQEABAAEFAAQUAAEFAAQUAAEFAAQUABBQAAQUABBQAAQUABBQAEBAARBQAEBAARBQAEBAAQABBUBAAQABBUBAAQABBQAEFAABBQAEFAABBQAEFAAQUAAEFAAQUAAEFAAQUABAQAEQUABAQAEQUABAQAEAAQVAQAEAAQVAQAEAAQUABBQAAQUABBQAAQUABBQAEFAABBQAEFAABBQAEFAAQEABEFAAQEABEFAAQEABAAEFQEABAAEFQEABAAEFAAQUAAEFAAQUAAEFAAQUABBQAAQUABBQAAQUABBQAEBAARBQAEBAARBQAEBAAQABBUBAAQABBUBAAQABBQAEFAABBQAEFAABBQAEFAAQUAAEFAAQUAAEFAAQUABAQAEQUPimc7nMZhyFx5cah+DJZTZfvjwwYLHEx5eWrltHCI7I44dQutJ2aXj47benpvy/JiwsLo4Qg2H16sLClBRR1GhUqiftqx8be+edtjZJEgRBMBh27Tp4UKNB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";

        public string PageExtension => HtmlPage.DefaultExtension;
        public string ThumbExtension => JpgThumb.DefaultExtension;

        public Page CreatePage(int pageNumber, byte[] data) => 
            new HtmlPage(pageNumber, data);

        public Thumb CreateThumb(int pageNumber, byte[] data) => 
            new JpgThumb(pageNumber, data);

        public Task<Page> GetPageAsync(FileCredentials fileCredentials, int pageNumber)
        {
            var html = string.Format(PAGE_TEMPLATE, pageNumber);
            var bytes = Encoding.UTF8.GetBytes(html);
            var page = new HtmlPage(pageNumber, bytes);

            return Task.FromResult((Page) page);
        }

        public Task<Thumb> GetThumbAsync(FileCredentials fileCredentials, int pageNumber)
        {
            var bytes = Convert.FromBase64String(THUMB_IN_BASE64);
            var thumb = new JpgThumb(pageNumber, bytes);

            return Task.FromResult((Thumb) thumb);
        }

        public Task<Pages> GetPagesAsync(FileCredentials fileCredentials, int[] pageNumbers)
        {
            var pages = pageNumbers.Select(pageNumber =>
            {
                var html = string.Format(PAGE_TEMPLATE, pageNumber);
                var pageBytes = Encoding.UTF8.GetBytes(html);

                var page = new HtmlPage(pageNumber, pageBytes);
                var css = @"
                    html {
                        background-color: red;
                    }
                ";
                var resourceBytes = Encoding.UTF8.GetBytes(css);
                var resource = new PageResource("styles.css", resourceBytes);
                
                page.AddResource(resource);

                return page;
            });

            var result = new Pages(pages);

            return Task.FromResult(result);
        }

        public Task<Thumbs> GetThumbsAsync(FileCredentials fileCredentials, int[] pageNumbers)
        {
            var thumbs = pageNumbers.Select(pageNumber =>
            {
                var bytes = Convert.FromBase64String(THUMB_IN_BASE64);
                var thumb = new JpgThumb(pageNumber, bytes);

                return thumb;
            });

            var result = new Thumbs(thumbs);

            return Task.FromResult(result);
        }

        public Task<DocumentInfo> GetDocumentInfoAsync(FileCredentials fileCredentials)
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

        public Task<byte[]> GetPdfAsync(FileCredentials fileCredentials)
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

        public Task<byte[]> GetPageResourceAsync(FileCredentials fileCredentials,int pageNumber, string resourceName)
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