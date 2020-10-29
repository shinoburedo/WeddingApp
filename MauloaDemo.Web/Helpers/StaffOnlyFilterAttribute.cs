using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

using MauloaDemo.Web.ViewModels;
using MauloaDemo.Repository;
using MauloaDemo.Models;


namespace MauloaDemo.Web {

    /// <summary>
    /// スタッフのみアクセス可能な事を示すフィルター
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class StaffOnlyFilterAttribute : UserSessionFilterAttribute {

        public StaffOnlyFilterAttribute() {
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            if (!base.AuthorizeCore(httpContext)) return false;

            LoginUser login_user = httpContext.Session[Constants.SSKEY_LOGIN_USER] as LoginUser;
            if (login_user == null || !login_user.IsStaff()) {
                throw new HttpException(403, "Forbidden.");
            }

            return true;
        }


    }
}