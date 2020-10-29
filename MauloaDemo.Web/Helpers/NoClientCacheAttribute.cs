using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace MauloaDemo.Web {
    public class NoClientCacheAttribute : ActionFilterAttribute {
        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            var response = filterContext.HttpContext.Response;
            if (response.IsRequestBeingRedirected) {
                return;
            }

            response.ClearHeaders();
            response.AppendHeader("Cache-Control", "private, no-cache, no-store, must-revalidate, max-stale=0, post-check=0, pre-check=0");
            response.AppendHeader("Pragma", "no-cache");
            response.AppendHeader("Expires", "-1");

            HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
            cache.SetCacheability(HttpCacheability.NoCache);
        }
    }
}