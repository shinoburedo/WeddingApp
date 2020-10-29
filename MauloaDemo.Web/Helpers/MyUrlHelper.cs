using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MauloaDemo.Web {

    public static class MyUrlHelper {

        //public static string CurrentAction(this UrlHelper urlHelper) {
        //    var routeValueDictionary = urlHelper.RequestContext.RouteData.Values;

        //    // In case using virtual dirctory
        //    var rootUrl = urlHelper.Content("~/");
        //    return string.Format("{0}{1}/{2}/", rootUrl, routeValueDictionary["controller"], routeValueDictionary["action"]);
        //}

        public static string CurrentActionPath(this ViewContext viewContext) {
            var rootUrl = viewContext.RootUrl();
            var areaName = viewContext.AreaName();
            if (!String.IsNullOrEmpty(areaName)) areaName += "/";

            var controllerName = viewContext.ControllerName();
            var actionName = viewContext.ActionName();

            return string.Format("{0}{1}{2}/{3}/",
                            rootUrl,
                            areaName,
                            controllerName,
                            actionName);
        }

        public static string RootUrl(this ViewContext viewContext) {
            var routeValueDictionary = viewContext.RequestContext.RouteData.Values;
            UrlHelper urlHelper = new UrlHelper(viewContext.RequestContext);
            var applicationName = urlHelper.Content("~/");

            return applicationName;
        }

        public static string ControllerName(this ViewContext viewContext) {
            string controllerName = viewContext.RequestContext.RouteData.GetRequiredString("controller");
            return controllerName;
        }

        public static string ActionName(this ViewContext viewContext) {
            string actionName = viewContext.RequestContext.RouteData.GetRequiredString("action");
            return actionName;
        }

        public static string AreaName(this ViewContext viewContext) {
            string areaName = viewContext.RequestContext.RouteData.DataTokens["area"] as string;
            return areaName;
        }

    }
}