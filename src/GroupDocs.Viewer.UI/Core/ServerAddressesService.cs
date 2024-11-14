using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Collections.Generic;
using System.Linq;

namespace GroupDocs.Viewer.UI.Core
{
    internal class ServerAddressesService
    {
        private readonly IServer _server;

        public ServerAddressesService(IServer server)
        {
            _server = server;
        }

        internal ICollection<string> Addresses => AddressesFeature.Addresses;

        private IServerAddressesFeature AddressesFeature =>
            _server.Features.Get<IServerAddressesFeature>();

        internal string AbsoluteUriFromRelative(string relativeUrl)
        {
            string targetAddress = AddressesFeature.Addresses.First();

            if (targetAddress.EndsWith('/'))
            {
                targetAddress = targetAddress[0..^1];
            }

            if (!relativeUrl.StartsWith('/'))
            {
                relativeUrl = $"/{relativeUrl}";
            }

            return $"{targetAddress}{relativeUrl}";
        }
    }
}
