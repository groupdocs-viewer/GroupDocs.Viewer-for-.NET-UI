using System.Text.Json;
using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<TContent> As<TContent>(this HttpResponseMessage response)
        {
            if (response == null)
            {
                throw new InvalidOperationException(
                    $"Response is null or message can't be deserialized as {typeof(TContent).FullName}.");
            }
            string body = await response.Content
                .ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                throw new InvalidOperationException(
                    $"Response is null or message can't be deserialized as {typeof(TContent).FullName}.");
            }
            TContent content = JsonSerializer.Deserialize<TContent>(body);

            if (content is not null)
            {
                return content;
            }

            throw new InvalidOperationException($"Response is null or message can't be deserialized as {typeof(TContent).FullName}.");
        }
    }
}
