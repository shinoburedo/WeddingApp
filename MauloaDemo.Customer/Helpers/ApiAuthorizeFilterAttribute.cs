using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace MauloaDemo.Customer {

    public class ApiAuthorizeFilterAttribute : System.Web.Http.AuthorizeAttribute {

        public override void OnAuthorization(
               System.Web.Http.Controllers.HttpActionContext actionContext) {
            base.OnAuthorization(actionContext);

            if (!HttpContext.Current.User.Identity.IsAuthenticated) {
                throw new HttpException(403, "Forbidden.");
            }

            //if (actionContext.Request.Headers.GetValues("authenticationToken") != null) {
            //    // get value from header
            //    string authenticationToken = Convert.ToString(
            //      actionContext.Request.Headers.GetValues("authenticationToken").FirstOrDefault());

            //    //authenticationTokenPersistant
            //    // it is saved in some data store
            //    // i will compare the authenticationToken sent by client with
            //    // authenticationToken persist in database against specific user, and act accordingly
            //    if (authenticationTokenPersistant != authenticationToken) {
            //        HttpContext.Current.Response.AddHeader("authenticationToken", authenticationToken);
            //        HttpContext.Current.Response.AddHeader("AuthenticationStatus", "NotAuthorized");
            //        actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Forbidden);
            //        return;
            //    }

            //    HttpContext.Current.Response.AddHeader("authenticationToken", authenticationToken);
            //    HttpContext.Current.Response.AddHeader("AuthenticationStatus", "Authorized");
            //    return;
            //}
            //actionContext.Response =
            //  actionContext.Request.CreateResponse(HttpStatusCode.ExpectationFailed);
            //actionContext.Response.ReasonPhrase = "Please provide valid inputs";
        }
    }
}