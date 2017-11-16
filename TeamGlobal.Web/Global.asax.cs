using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace TeamGlobal.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            log.Debug(exception.Message);
            log.Debug(exception.StackTrace);
            Server.ClearError();
            Response.Redirect("/Home/Error");
        }

        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            log.Debug("Application Started.");
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
