using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts
{
    internal interface IAuthServerConnect
    {
        Task<string> RequestClientCredentialsTokenAsync();
    }
}