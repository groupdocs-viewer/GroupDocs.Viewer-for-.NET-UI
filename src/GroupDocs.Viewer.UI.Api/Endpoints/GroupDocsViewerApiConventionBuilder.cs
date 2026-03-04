using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;

namespace GroupDocs.Viewer.UI.Api
{
    class GroupDocsViewerApiConventionBuilder : IEndpointConventionBuilder
    {
        private readonly IEnumerable<IEndpointConventionBuilder> _endpoints;

        public GroupDocsViewerApiConventionBuilder(IEnumerable<IEndpointConventionBuilder> endpoints)
        {
            _endpoints = endpoints ?? throw new ArgumentNullException(nameof(endpoints));
        }

        public void Add(Action<EndpointBuilder> convention)
        {
            foreach (var endpoint in _endpoints)
            {
                endpoint.Add(convention);
            }
        }
    }
}
