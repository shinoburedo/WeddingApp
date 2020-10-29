using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using MauloaDemo.Repository;
using MauloaDemo.Models;


namespace MauloaDemo.Web {

    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UserCultureFilterAttribute : ActionFilterAttribute {

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (filterContext.IsChildAction) return;
            if (filterContext.HttpContext == null) return;
            if (filterContext.HttpContext.Session == null) return;

            //セッションからログインユーザー情報を取得。
            LoginUser user = UserHelper.LoginUser;
            if (user != null) {
                SetCultureToThread(user.culture_name);
            }

            base.OnActionExecuting(filterContext);
        }

        //ユーザーの言語・カルチャー設定をスレッド全体に反映する。(これによってActiveReportsにも反映される。)
        public static void SetCultureToThread(string culture_name) {
            System.Threading.Thread.CurrentThread.CurrentCulture
                = System.Globalization.CultureInfo.CreateSpecificCulture(culture_name);

            System.Threading.Thread.CurrentThread.CurrentUICulture
                = System.Globalization.CultureInfo.CreateSpecificCulture(culture_name);
        }
    }
}