using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Extensions;

namespace GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Handlers
{
    internal class ProtectedApiBearerTokenHandler : DelegatingHandler
    {
        private readonly IAuthServerConnect _authServerConnect;

        public ProtectedApiBearerTokenHandler(IAuthServerConnect authServerConnect)
        {
            _authServerConnect = authServerConnect;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // request the access token
            var accessToken = await _authServerConnect.RequestClientCredentialsTokenAsync();

            // set the bearer token to the outgoing request as Authentication Header
            request.SetBearerToken(accessToken);

            // Proceed calling the inner handler, that will actually send the requestto our protected api
            return await base.SendAsync(request, cancellationToken);
        }
    }
}