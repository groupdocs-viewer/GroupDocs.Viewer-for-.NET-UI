using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Cloud.Api.Common;
using GroupDocs.Viewer.UI.Core.Entities;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts
{
    internal interface IViewerApiConnect
    {
        Task<Result<DocumentInfo>> GetDocumentInfoAsync(string filePath, string password, string storageName);
    }
   
}