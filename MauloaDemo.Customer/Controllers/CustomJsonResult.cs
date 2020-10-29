using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using MauloaDemo.Utilities;


namespace MauloaDemo.Customer.Controllers {

    public class CustomJsonResult : JsonResult {

        public override void ExecuteResult(ControllerContext context) {
            if (context == null) {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            if (!string.IsNullOrEmpty(ContentType)) {
                response.ContentType = ContentType;
            }
            else {
                response.ContentType = "application/json";
            }
            if (ContentEncoding != null) {
                response.ContentEncoding = ContentEncoding;
            }
            if (Data != null) {
                //DateTime型をカルチャーに関わらず、かつサーバー間の時差にも影響されない文字列に変換する。
                response.Write(JsonHelper.MySerializeObject(Data));
            }
        }

    }
}