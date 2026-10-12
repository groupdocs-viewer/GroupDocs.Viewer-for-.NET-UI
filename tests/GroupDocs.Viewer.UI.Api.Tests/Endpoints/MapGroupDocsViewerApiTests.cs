using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Endpoints
{
    /// <summary>
    /// Conventions added to the builder returned by MapGroupDocsViewerApi must reach every API endpoint (issue #116).
    /// </summary>
    public class MapGroupDocsViewerApiTests
    {
        private const int ApiEndpointsCount = 9;

        [Fact]
        public async Task MapGroupDocsViewerApi_WithRequireAuthorization_ShouldRequireAuthorizationOnAllEndpoints()
        {
            await using var app = CreateApp();

            app.MapGroupDocsViewerApi().RequireAuthorization("ViewerPolicy");

            var endpoints = GetEndpoints(app);
            Assert.Equal(ApiEndpointsCount, endpoints.Count);
            Assert.All(endpoints, endpoint =>
                Assert.Equal("ViewerPolicy", endpoint.Metadata.GetMetadata<IAuthorizeData>()?.Policy));
        }

        [Fact]
        public async Task MapGroupDocsViewerApi_WithCustomConvention_ShouldApplyConventionToAllEndpoints()
        {
            await using var app = CreateApp();
            var metadata = new object();

            app.MapGroupDocsViewerApi(options => options.ApiPath = "/custom-api").WithMetadata(metadata);

            var endpoints = GetEndpoints(app);
            Assert.Equal(ApiEndpointsCount, endpoints.Count);
            Assert.All(endpoints, endpoint =>
            {
                Assert.StartsWith("custom-api/", endpoint.RoutePattern.RawText);
                Assert.Contains(metadata, endpoint.Metadata);
            });
        }

        [Fact]
        public async Task MapGroupDocsViewerApi_WithoutConventions_ShouldNotRequireAuthorization()
        {
            await using var app = CreateApp();

            app.MapGroupDocsViewerApi();

            var endpoints = GetEndpoints(app);
            Assert.Equal(ApiEndpointsCount, endpoints.Count);
            Assert.All(endpoints, endpoint => Assert.Null(endpoint.Metadata.GetMetadata<IAuthorizeData>()));
        }

        private static WebApplication CreateApp()
        {
            var builder = WebApplication.CreateBuilder();
            builder.Services
                .AddControllers()
                .AddApplicationPart(typeof(ViewerController).Assembly);

            return builder.Build();
        }

        // NOTE: conventional routes also add link generation endpoints, they never match requests
        private static List<RouteEndpoint> GetEndpoints(IEndpointRouteBuilder app) =>
            app.DataSources
                .SelectMany(dataSource => dataSource.Endpoints)
                .OfType<RouteEndpoint>()
                .Where(endpoint => endpoint.Metadata.GetMetadata<ISuppressMatchingMetadata>()?.SuppressMatching != true)
                .ToList();
    }
}
