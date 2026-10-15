using Microsoft.AspNetCore.Mvc.Routing;

namespace GroupDocs.Viewer.UI.Api.Filters
{
    /// <summary>
    /// Adds a route value that only the routes mapped by <c>MapGroupDocsViewerApi</c> provide.
    /// Conventional routes of the app, e.g. <c>{controller=Home}/{action=Index}/{id?}</c>, don't provide it
    /// and can't reach the controller, so the endpoint conventions such as <c>RequireAuthorization()</c>
    /// can't be bypassed by requesting <c>/viewer/GetPdf</c> instead of <c>/viewer-api/get-pdf</c>.
    /// </summary>
    internal class GroupDocsViewerApiRouteAttribute : RouteValueAttribute
    {
        public GroupDocsViewerApiRouteAttribute()
            : base(ApiNames.ROUTE_VALUE_KEY, ApiNames.ROUTE_VALUE)
        {
        }
    }
}
