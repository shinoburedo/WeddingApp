using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;

namespace MauloaDemo.Customer.Controllers {

    public class MyRequireHttpsAttribute : RequireHttpsAttribute 
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null) {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext == null) {
                return;
            }

            if (string.Equals(filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"],
                        "https",
                        StringComparison.InvariantCultureIgnoreCase)) {
                return;     //X-Forwarded-Protoヘッダーの値がhttpsである場合はリダイレクトしない。（ループを防ぐため。）
            }

            var port = filterContext.HttpContext.Request.Url.Port;
            if (port != 80) {
                return;     //80番ポート以外の場合はリダイレクトしない。（ループを防ぐため。）
            }

            if (filterContext.HttpContext.Request.IsLocal){
                return;
            }

            base.OnAuthorization(filterContext);
        }
    }
}