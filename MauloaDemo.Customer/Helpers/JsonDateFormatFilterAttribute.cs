using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;

using MauloaDemo.Models;


namespace MauloaDemo.Customer {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JsonDateFormatFilterAttribute : ActionFilterAttribute {

        /// <summary>
        /// JsonResultのDateTime型が含まれる場合、カスタムのフォーマットを適用する。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext) {
            if (filterContext.Exception == null) {
                if (filterContext.Result is JsonResult) {
                    JsonResult jsonResult = (JsonResult)filterContext.Result;

                    var myJson = new CustomJsonResult();
                    myJson.Data = jsonResult.Data;
                    filterContext.Result = myJson;
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }

}