using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GroupDocs.Viewer.UI.NetFramework.Sample
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static ViewerUI _viewer;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            ViewerUIConfig viewerConfig = new ViewerUIConfig
            {
                UIPath = "/viewer",
                ApiEndpoint = "https://localhost:5001/viewer-api"
            };

            _viewer = ViewerUI.Configure(viewerConfig);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            _viewer.HandleRequest(HttpContext.Current);
        }
    }
}
