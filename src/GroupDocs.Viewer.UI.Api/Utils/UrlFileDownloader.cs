using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GroupDocs.Viewer.UI.Api.Utils
{
    /// <summary>
    /// Downloads files for "upload by URL". Only public internet addresses can be reached,
    /// so a client can't use the server to access internal services (server-side request forgery).
    /// </summary>
    internal static class UrlFileDownloader
    {
        private static readonly HttpClient HttpClient = CreateHttpClient();

        /// <summary>
        /// Downloads a file from an absolute HTTP or HTTPS URL.
        /// </summary>
        /// <param name="url">The URL to download the file from.</param>
        /// <param name="maxFileSize">The maximum file size in bytes, <c>null</c> for no limit.</param>
        /// <param name="cancellationToken">A token to cancel the download.</param>
        public static async Task<(string FileName, byte[] Bytes)> DownloadAsync(
            string url, long? maxFileSize, CancellationToken cancellationToken)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                throw new ArgumentException("Only absolute HTTP and HTTPS URLs can be uploaded.");

            using var response = await HttpClient.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            if (response.Content.Headers.ContentLength > maxFileSize)
                throw CreateFileTooLargeException();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var bytes = await ReadAllBytesAsync(stream, maxFileSize, cancellationToken);

            return (Path.GetFileName(uri.LocalPath), bytes);
        }

        internal static async Task<byte[]> ReadAllBytesAsync(Stream stream, long? maxFileSize, CancellationToken cancellationToken)
        {
            using var memoryStream = new MemoryStream();
            var buffer = new byte[81920];

            int bytesRead;
            while ((bytesRead = await stream.ReadAsync(buffer, cancellationToken)) > 0)
            {
                // NOTE: Content-Length is optional, so the limit is enforced while reading
                if (memoryStream.Length + bytesRead > maxFileSize)
                    throw CreateFileTooLargeException();

                memoryStream.Write(buffer, 0, bytesRead);
            }

            return memoryStream.ToArray();
        }

        /// <summary>
        /// Returns <c>true</c> for public unicast addresses, <c>false</c> for loopback, private, link-local
        /// (e.g. cloud metadata at 169.254.169.254), multicast, documentation and other special-purpose ranges.
        /// </summary>
        internal static bool IsPublicAddress(IPAddress address)
        {
            if (address.IsIPv4MappedToIPv6)
                return IsPublicAddress(address.MapToIPv4());

            var bytes = address.GetAddressBytes();

            if (address.AddressFamily == AddressFamily.InterNetwork)
                return IsPublicIPv4Address(bytes);

            // NAT64 64:ff9b::/96 embeds an IPv4 address
            if (bytes.Take(12).SequenceEqual(new byte[] { 0x00, 0x64, 0xff, 0x9b, 0, 0, 0, 0, 0, 0, 0, 0 }))
                return IsPublicIPv4Address(bytes.Skip(12).ToArray());

            // 6to4 2002::/16 embeds an IPv4 address
            if (bytes[0] == 0x20 && bytes[1] == 0x02)
                return IsPublicIPv4Address(bytes.Skip(2).Take(4).ToArray());

            // Documentation 2001:db8::/32
            if (bytes[0] == 0x20 && bytes[1] == 0x01 && bytes[2] == 0x0d && bytes[3] == 0xb8)
                return false;

            // Global unicast 2000::/3, this excludes ::1, ::, fc00::/7, fe80::/10, ff00::/8 and others
            return (bytes[0] & 0xe0) == 0x20;
        }

        private static bool IsPublicIPv4Address(byte[] bytes) =>
            !(bytes[0] == 0                                             // 0.0.0.0/8 "this network"
              || bytes[0] == 10                                         // 10.0.0.0/8 private
              || (bytes[0] == 100 && (bytes[1] & 0xc0) == 64)           // 100.64.0.0/10 shared address space
              || bytes[0] == 127                                        // 127.0.0.0/8 loopback
              || (bytes[0] == 169 && bytes[1] == 254)                   // 169.254.0.0/16 link-local
              || (bytes[0] == 172 && (bytes[1] & 0xf0) == 16)           // 172.16.0.0/12 private
              || (bytes[0] == 192 && bytes[1] == 0 && bytes[2] == 0)    // 192.0.0.0/24 protocol assignments
              || (bytes[0] == 192 && bytes[1] == 0 && bytes[2] == 2)    // 192.0.2.0/24 documentation
              || (bytes[0] == 192 && bytes[1] == 88 && bytes[2] == 99)  // 192.88.99.0/24 6to4 relay
              || (bytes[0] == 192 && bytes[1] == 168)                   // 192.168.0.0/16 private
              || (bytes[0] == 198 && (bytes[1] & 0xfe) == 18)           // 198.18.0.0/15 benchmarking
              || (bytes[0] == 198 && bytes[1] == 51 && bytes[2] == 100) // 198.51.100.0/24 documentation
              || (bytes[0] == 203 && bytes[1] == 0 && bytes[2] == 113)  // 203.0.113.0/24 documentation
              || bytes[0] >= 224);                                      // 224.0.0.0/4 multicast, 240.0.0.0/4 reserved

        private static HttpClient CreateHttpClient()
        {
            var handler = new SocketsHttpHandler
            {
                // NOTE: through a proxy the connection goes to the proxy, so the target address couldn't be checked
                UseProxy = false,
                MaxAutomaticRedirections = 5,
                PooledConnectionLifetime = TimeSpan.FromMinutes(1),
                // NOTE: every connection, including the ones made for redirects, goes through this check
                ConnectCallback = ConnectToPublicAddressAsync
            };

            var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Other");

            return httpClient;
        }

        // Resolves the host and connects to the addresses that passed the check,
        // so DNS can't return a different, internal, address for the actual connection
        private static async ValueTask<Stream> ConnectToPublicAddressAsync(
            SocketsHttpConnectionContext context, CancellationToken cancellationToken)
        {
            var host = context.DnsEndPoint.Host;
            var addresses = IPAddress.TryParse(host, out var address)
                ? new[] { address }
                : await Dns.GetHostAddressesAsync(host, cancellationToken);
            var publicAddresses = addresses.Where(IsPublicAddress).ToArray();

            if (publicAddresses.Length == 0)
                throw new HttpRequestException("Uploading files from internal network addresses is not allowed.");

            var socket = new Socket(SocketType.Stream, ProtocolType.Tcp) { NoDelay = true };
            try
            {
                await socket.ConnectAsync(publicAddresses, context.DnsEndPoint.Port, cancellationToken);
                return new NetworkStream(socket, ownsSocket: true);
            }
            catch
            {
                socket.Dispose();
                throw;
            }
        }

        private static InvalidOperationException CreateFileTooLargeException() =>
            new InvalidOperationException("The file is larger than the maximum upload size.");
    }
}
