using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MauloaDemo.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //生成される URL を小文字にする
            routes.LowercaseUrls = true;
            //生成される URL の最後にスラッシュを付ける
            routes.AppendTrailingSlash = true;
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Notice", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "MauloaDemo.Web.Controllers" }
            );
        }
    }
}