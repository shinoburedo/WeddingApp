using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using MauloaDemo.Models;
using MauloaDemo.Customer.ViewModels;


namespace MauloaDemo.Customer {

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class AjaxUserSessionFilterAttribute : ActionFilterAttribute {

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (filterContext.IsChildAction) return;
            if (filterContext.RequestContext == null) return;
            if (filterContext.RequestContext.HttpContext == null) return;

            var httpContext = filterContext.RequestContext.HttpContext;
            if (httpContext.Session == null) return;

            // Ajaxリクエストではない場合は何もしない。
            if (!filterContext.RequestContext.HttpContext.Request.IsAjaxRequest()) return;

            if (UserHelper.LoginUser == null) {
                var url = System.Web.Security.FormsAuthentication.LoginUrl
                            + "?ReturnUrl=" + httpContext.Server.UrlEncode(httpContext.Request.UrlReferrer.ToString());

                // Statusコード：403(Forbidden)を返す。
                // (401:UnauthorizedだとASP.NETによって自動的に302のログイン画面へのRedirectに書き換えられてしまうので。)
                // JavaScript側でAjaxのエラーハンドラでStatus=403の場合にe.xhr.statusTextにurlが入っているのでそこに移動させる必要がある。
                filterContext.Result =new HttpStatusCodeResult(403, url);
            }
        }

    
    }
}