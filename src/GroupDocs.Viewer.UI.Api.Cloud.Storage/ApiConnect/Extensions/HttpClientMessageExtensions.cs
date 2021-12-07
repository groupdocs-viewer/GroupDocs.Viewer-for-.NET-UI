using System.Net.Http;
using System.Net.Http.Headers;

namespace GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Extensions
{
    internal static class HttpClientMessageExtensions
    {
        /// <summary>
        /// Sets an authorization header with a given scheme and value.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="scheme">The scheme.</param>
        /// <param name="token">The token.</param>
        public static void SetToken(this HttpRequestMessage request, string scheme, string token)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue(scheme, token);
        }

        /// <summary>
        /// Sets an authorization header with a bearer token.
        /// </summary>
        /// <param name="request">The HTTP request message.</param>
        /// <param name="token">The token.</param>
        public static void SetBearerToken(this HttpRequestMessage request, string token)
        {
            request.SetToken("Bearer", token);
        }
    }
}