using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect.Contracts;
using GroupDocs.Viewer.UI.Api.Cloud.Storage.Configuration;
using Microsoft.Extensions.Options;

namespace GroupDocs.Viewer.UI.Api.Cloud.Storage.ApiConnect
{
    internal class AuthServerConnect : IAuthServerConnect
    {
        private readonly HttpClient _httpClient;
        private readonly Config _config;

        public AuthServerConnect(HttpClient httpClient, IOptions<Config> config)
        {
            _httpClient = httpClient;
            _config = config.Value;
        }

        public async Task<string> RequestClientCredentialsTokenAsync()
        {
            var content = new StringContent(
                $"grant_type=client_credentials&client_id={_config.ClientId}&client_secret={_config.ClientSecret}",
                Encoding.UTF8,
                "application/x-www-form-urlencoded");

            var response = await _httpClient.PostAsync("/connect/token", content);
            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<GetAccessTokenResult>(json);
            var token = result.access_token;

            return token;
        }

        private class GetAccessTokenResult
        {
            public string access_token { get; set; }
        }
    }
}