using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using CBAF;
using MauloaDemo.Models;


namespace MauloaDemo.Customer {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LoggingFilterAttribute : ActionFilterAttribute {

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (filterContext.IsChildAction) return;
            if (filterContext.RequestContext == null) return;
            if (filterContext.RequestContext.HttpContext == null) return;

            bool isEnabled = TypeHelper.GetBool(ConfigurationManager.AppSettings["LoggingEnabled"]);
            if (isEnabled) {
                var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                var actionName = filterContext.ActionDescriptor.ActionName;
                var msg = String.Format("Action Executing: {0}/{1}", controllerName, actionName);
                filterContext.HttpContext.Trace.Write(msg);
            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext) {
            if (filterContext.Exception != null) {
                bool isEnabled = TypeHelper.GetBool(ConfigurationManager.AppSettings["LoggingEnabled"]);
                if (isEnabled) {
                    var controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                    var actionName = filterContext.ActionDescriptor.ActionName;
                    var exceptionMsg = filterContext.Exception.ToString();
                    var msg = String.Format("Exception in {0}/{1}: {2}", controllerName, actionName, exceptionMsg);
                    filterContext.HttpContext.Trace.Write(msg);
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}