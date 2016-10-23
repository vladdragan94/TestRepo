using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MovieDictionary
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
        }  

        protected void Application_EndRequest()
        {
            if (Context.Response.StatusCode >= 400)
            {
                Response.Clear();

                RouteData routeData = new RouteData();
                routeData.Values.Add("controller", "Home");
                routeData.Values.Add("action", "Info");

                if (Context.Response.StatusCode == 404)
                {
                    routeData.Values.Add("message", Entities.Constants.ErrorMessages.PageNotFound);
                }
                else 
                {
                    routeData.Values.Add("message", Entities.Constants.ErrorMessages.DefaultError);
                }

                Server.ClearError();
                Response.TrySkipIisCustomErrors = true;

                IController controller = new Controllers.HomeController();
                controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            }
        }
    }
}
