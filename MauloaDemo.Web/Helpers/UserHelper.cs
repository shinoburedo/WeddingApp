using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;
using MauloaDemo.Web.Controllers;
using MauloaDemo.Models;

namespace MauloaDemo.Web {
    public static class UserHelper {

        public static LoginUser LoginUser
        {
            get {
                if (HttpContext.Current == null) return null;
                if (HttpContext.Current.Session == null) return null;
                return HttpContext.Current.Session[Constants.SSKEY_LOGIN_USER] as LoginUser;
            }
            set {
                if (HttpContext.Current == null) return;
                if (HttpContext.Current.Session == null) return;
                HttpContext.Current.Session[Constants.SSKEY_LOGIN_USER] = value;
            }
        }
        

    }
}