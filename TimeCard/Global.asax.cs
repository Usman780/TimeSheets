using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
namespace TimeCard
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //private const string jobAction = "http://localhost:50119/Home/timezone";
        //private const string jobAction = "http://timecard-1.apphb.com/Home/timezone";
        private const string jobAction = "http://timesheets.strattechnologies.com/Home/timezone";
        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 3600000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var client = new WebClient();
            client.DownloadData(jobAction);
        }
    }
}