using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebShop2018
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Ukljucimo rutiranje preko atributa
            routes.MapMvcAttributeRoutes();

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "WebProgramiranje",
                url: "route/test/{year}/{category}",
                defaults: new { controller = "Home",
                    action = "RouteTest",
                    year = UrlParameter.Optional,
                    category = UrlParameter.Optional
                }
            );
        }
    }
}
