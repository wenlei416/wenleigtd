using System.Web.Mvc;
using System.Web.Routing;

namespace GTD
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "sort",
                url: "task/listtask/{da}/{sortOrder}",
                defaults: new { Controller = "Task", action = "listtask" ,sortOrder=UrlParameter.Optional});

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Task", action = "index", id = UrlParameter.Optional });
        }
    }
}