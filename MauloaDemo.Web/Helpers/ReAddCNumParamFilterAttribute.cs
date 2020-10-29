using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using ProjectM.Repository;
using ProjectM.Models;


namespace ProjectM.Web {

    /// <summary>
    /// クエリー文字列に"c_num"が含まれていない場合に、Sessionに保存されたc_numがあればそれを付加したURLにリダイレクトする。
    /// 
    /// 基本的にはクエリー文字列にc_numを付加してページ間でc_numを受け渡すが、それが不可能な場合もあるので一応Sessionも併用出来る様に。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ReAddCNumParamFilterAttribute : ActionFilterAttribute {

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (filterContext.IsChildAction) return;

            //HttpContextBase context = filterContext.HttpContext;

            //string c_num = TypeHelper.GetStrTrim(context.Request.QueryString["c_num"]);
            //if (String.IsNullOrWhiteSpace(c_num)) {

            //    //クエリー文字列に"c_num"が含まれていない場合に、Sessionに保存されたc_numがあればそれを付加したURLにリダイレクトする。
            //    c_num = context.Session[Constants.SSKEY_CURRENT_CNUM] as string;

            //    if (!String.IsNullOrWhiteSpace(c_num)) {
            //        string s = context.Request.Url.ToString();
            //        s = s.Replace("&c_num=", "").Replace("?c_num=", "");
            //        s += s.Contains("?") ? "&" : "?";
            //        s += "c_num=" + c_num;
            //        filterContext.Result = new RedirectResult(s);
            //    }
            //}
            //else {
            //    context.Session[Constants.SSKEY_CURRENT_CNUM] = c_num.ToUpper();
            //}
        }
    }
}