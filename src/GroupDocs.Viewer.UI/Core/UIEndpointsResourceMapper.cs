using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupDocs.Viewer.UI.Configuration;
using GroupDocs.Viewer.UI.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace GroupDocs.Viewer.UI.Core
{
    internal class UIEndpointsResourceMapper
    {
        private readonly IUIResourcesReader _reader;

        public UIEndpointsResourceMapper(IUIResourcesReader reader)
        {
            _reader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        public IEnumerable<IEndpointConventionBuilder> Map(IEndpointRouteBuilder builder, Options options)
        {
            var endpoints = new List<IEndpointConventionBuilder>();

            var resources = _reader.UIResources.ToList();
            var stylesheets = resources.GetIndexPage()
                .GetCustomStylesheets(options);

            foreach (var resource in resources)
            {
                endpoints.Add(builder.MapGet($"{options.UIPath}/{resource.FileName}", async context =>
                {
                    context.Response.ContentType = resource.ContentType;
                    await context.Response.WriteAsync(resource.Content);
                }));
            }

            endpoints.Add(builder.MapGet(options.UIPath, async context =>
            {
                context.Response.OnStarting(() =>
                {
                    if (!context.Response.Headers.ContainsKey("Cache-Control"))
                    {
                        context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                    }

                    return Task.CompletedTask;
                });

                var indexPage = resources.GetIndexPage();
                var pathBase = context.Request.PathBase;
                var routeValues = context.Request.RouteValues.ToDictionary();
                var content = indexPage.GetIndexPageHtml(options, pathBase, routeValues);

                context.Response.ContentType = indexPage.ContentType;
                await context.Response.WriteAsync(content);
            }));

            foreach (var item in stylesheets)
            {
                endpoints.Add(builder.MapGet(item.ResourcePath, async context =>
                {
                    context.Response.ContentType = "text/css";
                    await context.Response.Body.WriteAsync(item.Content, 0, item.Content.Length);
                }));
            }

            return endpoints;
        }
    }
}