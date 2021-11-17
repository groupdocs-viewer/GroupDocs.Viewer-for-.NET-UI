using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Cloud.Api.ApiConnect.Contracts
{
    /// <summary>
    /// Auth API
    /// </summary>
    internal interface IAuthServerConnect
    {
        /// <summary>
        /// Requests client credentials Bearer token 
        /// </summary>
        /// <returns>Bearer token</returns>
        Task<string> RequestClientCredentialsTokenAsync();
    }
}