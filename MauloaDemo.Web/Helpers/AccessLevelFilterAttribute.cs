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
    /// 最低限必要な権限レベルを指定する属性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AccessLevelFilterAttribute : UserSessionFilterAttribute {
        private int _required_accessLevel = 0;

        public AccessLevelFilterAttribute(int required_accessLevel) {
            _required_accessLevel = required_accessLevel;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            if (!base.AuthorizeCore(httpContext)) return false;

            LoginUser login_user = httpContext.Session[Constants.SSKEY_LOGIN_USER] as LoginUser;
            var actual_level = login_user != null ? login_user.access_level : -1;

            if (actual_level < _required_accessLevel) {
                //throw new AccessLevelErrorException(_required_accessLevel, actual_level);
                throw new HttpException(403, "Forbidden.");
            }

            return true;
        }


    }
}