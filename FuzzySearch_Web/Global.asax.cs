using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using FuzzySearchDemo.Models;

namespace FuzzySearchDemo
{
    public class MvcApplication : System.Web.HttpApplication
    {

        // WarAndPeace
        public string[] WarAndPeace;
        public int WarAndPeaceNumberOfPages;

        protected void Application_Start()
        {
            // WarAndPeace
            WarAndPeace = Models.SearchInBook_WarAndPeace.СreatePages();
            WarAndPeaceNumberOfPages = WarAndPeace.Length;

            Application["WarAndPeace"] = WarAndPeace;
            Application["WarAndPeaceNumberOfPages"] = WarAndPeaceNumberOfPages;

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }
    }
}
