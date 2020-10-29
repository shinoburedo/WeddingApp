using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using CBAF;
using MauloaDemo.Customer.ViewModels;
using MauloaDemo.Repository;
using MauloaDemo.Models;


namespace MauloaDemo.Customer {

    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UserSessionFilterAttribute : AuthorizeAttribute {

        /// <summary>
        /// Forms認証クッキーがあり、かつセッションのLoginUserとlogin_idが一致する場合は true を返す。
        /// 
        /// クエリー文字列に認証トークンが含まれているかチェックして、認証トークンが検証出来ればログイン済みとしてtrueを返す。
        /// 
        /// AuthorizeAttributeの認証チェックの部分のみをカスタマイズ。
        ///
        /// 参考： ASP.NET MVC4のソースコードは以下で確認出来る。
        /// http://aspnetwebstack.codeplex.com/
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            if (httpContext == null) return false;

            if (httpContext.User.Identity.IsAuthenticated) {
                LoginUser session_user = httpContext.Session[Constants.SSKEY_LOGIN_USER] as LoginUser;

                ////Forms認証クッキーがあり、かつセッションのLoginUserとstaff_cdが一致する場合は true を返す。
                //string auth_name = httpContext.User.Identity.Name.ToUpper();
                //if (session_user != null && session_user.staff_cd.ToUpper().Equals(auth_name)) {
                //    return true;
                //}

                //Forms認証クッキーがあり、かつセッションにLoginUserがセットされていればtrueを返す。
                if (session_user != null) {

                    //セッションにLoginUserがあるがパスワードの有効期限が切れている場合はProfile画面のパスワード変更タブにリダイレクト。
                    if (session_user.eff_to_pass.HasValue && Common.GetJapanDate().CompareTo(session_user.eff_to_pass) > 0) {
                        string url = System.Web.Security.FormsAuthentication.LoginUrl;
                        url = TypeHelper.GetStrTrim(url).ToLower();
                        url = url.Replace("login", "profile");
                        url += "?tab=1";
                        httpContext.Response.Redirect(url);
                    }

                    return true;
                }
            }

            httpContext.Session.Clear();
            httpContext.Session.Abandon();
            return false;
        }

        public override void OnAuthorization(AuthorizationContext filterContext) {
            if (filterContext.IsChildAction) return;

            if (filterContext == null) {
                throw new ArgumentNullException("filterContext");
            }

            //AjaxUserSessionFilterが付いている場合は、ここでの認証チェックはスキップして、代わりに[AjaxUserSessionFilter]からStatus 403(Forbidden)を返す。
            //bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AjaxUserSessionFilterAttribute), inherit: true);

            //Ajaxリクエストの場合はここでの認証チェックはスキップして、代わりに[AjaxUserSessionFilter]からStatus 403(Forbidden)を返す。
            bool skipAuthorization = filterContext.HttpContext.Request.IsAjaxRequest();
            if (skipAuthorization) return;

            base.OnAuthorization(filterContext);
        }



    }
}