using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Endpoints
{
    /// <summary>
    /// Conventions added to the builder returned by MapGroupDocsViewerApi must reach every API endpoint (issue #116),
    /// and conventional routes of the app must not create other endpoints for the API actions (issue #117).
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
        public async Task MapGroupDocsViewerApi_WithDefaultControllerRoute_ShouldNotMapViewerActionsToDefaultRoute()
        {
            await using var app = CreateApp();

            app.MapGroupDocsViewerApi().RequireAuthorization("ViewerPolicy");
            app.MapDefaultControllerRoute();

            var endpoints = GetViewerEndpoints(app);
            Assert.Equal(ApiEndpointsCount, endpoints.Count);
            Assert.All(endpoints, endpoint =>
            {
                Assert.StartsWith("viewer-api/", endpoint.RoutePattern.RawText);
                Assert.Equal("ViewerPolicy", endpoint.Metadata.GetMetadata<IAuthorizeData>()?.Policy);
            });
        }

        [Fact]
        public async Task CustomConventionalRoute_WithoutApiRouteValue_ShouldNotReachViewerController()
        {
            await using var app = CreateApp();

            app.MapControllerRoute("my-viewer-api", "my-viewer-api/{action}", new { controller = ApiNames.CONTROLLER_NAME });

            Assert.Empty(GetViewerEndpoints(app));
        }

        // NOTE: the upgrade notes in README.md recommend this route for apps with their own route to the controller
        [Fact]
        public async Task CustomConventionalRoute_WithApiRouteValue_ShouldReachViewerController()
        {
            await using var app = CreateApp();

            app.MapControllerRoute("my-viewer-api", "my-viewer-api/{action}", new RouteValueDictionary
            {
                ["controller"] = ApiNames.CONTROLLER_NAME,
                [ApiNames.ROUTE_VALUE_KEY] = ApiNames.ROUTE_VALUE,
            });

            var endpoints = GetViewerEndpoints(app);
            Assert.Equal(ApiEndpointsCount, endpoints.Count);
            Assert.All(endpoints, endpoint => Assert.StartsWith("my-viewer-api/", endpoint.RoutePattern.RawText));
        }

        // NOTE: the upgrade notes in README.md recommend route names for links to the API methods
        [Fact]
        public async Task MapGroupDocsViewerApi_WithDefaultControllerRoute_ShouldGenerateLinksByRouteName()
        {
            await using var app = CreateApp();

            app.MapGroupDocsViewerApi();
            app.MapDefaultControllerRoute();

            // NOTE: the link generator sees the endpoints of the app once the app is started
            app.Urls.Add("http://127.0.0.1:0");
            await app.StartAsync();

            var linkGenerator = app.Services.GetRequiredService<LinkGenerator>();
            Assert.Equal("/viewer-api/get-pdf?file=sample.docx",
                linkGenerator.GetPathByRouteValues(ApiNames.API_METHOD_GET_PDF, new { file = "sample.docx" }));

            await app.StopAsync();
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

        private static List<RouteEndpoint> GetViewerEndpoints(IEndpointRouteBuilder app) =>
            GetEndpoints(app)
                .Where(endpoint => endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()?.ControllerTypeInfo == typeof(ViewerController))
                .ToList();
    }
}
