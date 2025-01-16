using GroupDocs.Viewer.UI.Api.NetFramework.Extensions;
using GroupDocs.Viewer.UI.Core;
using GroupDocs.Viewer.UI.NetFramework;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace GroupDocs.Viewer.UI.SelfHosted.App.NetFramework.Sample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static ViewerUI _viewer;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            var viewerType = ViewerType.HtmlWithEmbeddedResources;
            GlobalConfiguration.Configure(config =>
            {
                var serviceCollection = MvcBuilderExtensions.AddGroupDocsViewerSelfHostApi(null, config =>
                {
                    config.SetViewerType(viewerType);
                    //Trial limitations https://docs.groupdocs.com/viewer/net/evaluation-limitations-and-licensing-of-groupdocs-viewer/
                    //Temporary license can be requested at https://purchase.groupdocs.com/temporary-license
                    config.SetLicensePath("c:\\licenses\\GroupDocs.Viewer.lic"); // or set environment variable 'GROUPDOCS_LIC_PATH'
                })
                .AddLocalStorage(HttpContext.Current.Server.MapPath("~/App_Data/Files"))
                .AddLocalCache(HttpContext.Current.Server.MapPath("~/App_Data/Cache"));

                var serviceProvider = serviceCollection.Services.BuildServiceProvider();
                config.DependencyResolver = new ViewerDependencyResolver(serviceProvider);
                ViewerApiEndpointRouteBuilder.RegisterViewerApi(config, options =>
                {
                    options.ApiPath = "/viewer-api";
                });
            });
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ViewerUIConfig viewerConfig = new ViewerUIConfig
            {
                UIPath = "/viewer",
                ApiEndpoint = "/viewer-api"
            };
            _viewer = ViewerUI.Configure(viewerConfig);


        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.RequestType == "GET")
            {
                _viewer.HandleRequest(HttpContext.Current);
            }

        }
    }
}
