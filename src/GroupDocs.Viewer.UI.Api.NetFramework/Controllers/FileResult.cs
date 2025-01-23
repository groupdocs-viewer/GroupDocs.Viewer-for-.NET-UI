using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace GroupDocs.Viewer.UI.Api.NetFramework.Controllers;

public class FileResult : IHttpActionResult
{
    private readonly byte[] _fileContents;
    private readonly string _contentType;
    private readonly string _fileName;

    public FileResult(byte[] fileContents, string contentType, string fileName)
    {
        _fileContents = fileContents;
        _contentType = contentType;
        _fileName = fileName;
    }

    public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
    {
        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(_fileContents)
        };

        response.Content.Headers.ContentType = new MediaTypeHeaderValue(_contentType);
        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
            FileName = _fileName
        };

        return Task.FromResult(response);
    }
}