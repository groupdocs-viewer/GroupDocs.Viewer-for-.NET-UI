using System.Collections.Generic;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Api.Filters;
using GroupDocs.Viewer.UI.Core.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace GroupDocs.Viewer.UI.Api.Tests.Filters
{
    public class ResponseCacheFilterTests
    {
        [Fact]
        public void OnActionExecuted_WhenCacheDurationSet_ShouldSetCacheControlHeader()
        {
            var config = new Config { ResponseCacheDurationSeconds = 3600 };
            var filter = new ResponseCacheFilter(MSOptions.Create(config));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = "GET";

            var context = CreateActionExecutedContext(httpContext);

            filter.OnActionExecuted(context);

            Assert.Equal("public, max-age=3600", httpContext.Response.Headers["Cache-Control"].ToString());
        }

        [Fact]
        public void OnActionExecuted_WhenCacheDurationZero_ShouldNotSetHeader()
        {
            var config = new Config { ResponseCacheDurationSeconds = 0 };
            var filter = new ResponseCacheFilter(MSOptions.Create(config));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = "GET";

            var context = CreateActionExecutedContext(httpContext);

            filter.OnActionExecuted(context);

            Assert.False(httpContext.Response.Headers.ContainsKey("Cache-Control"));
        }

        [Fact]
        public void OnActionExecuted_WhenPostRequest_ShouldNotSetHeader()
        {
            var config = new Config { ResponseCacheDurationSeconds = 3600 };
            var filter = new ResponseCacheFilter(MSOptions.Create(config));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = "POST";

            var context = CreateActionExecutedContext(httpContext);

            filter.OnActionExecuted(context);

            Assert.False(httpContext.Response.Headers.ContainsKey("Cache-Control"));
        }

        private static ActionExecutedContext CreateActionExecutedContext(HttpContext httpContext)
        {
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            return new ActionExecutedContext(
                actionContext,
                new List<IFilterMetadata>(),
                new object());
        }
    }
}
