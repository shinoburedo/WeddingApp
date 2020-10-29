using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace MauloaDemo.Web {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiAccessLevelFilterAttribute : System.Web.Http.AuthorizeAttribute {

        private int _required_accessLevel = 0;

        public ApiAccessLevelFilterAttribute(int required_accessLevel) {
            _required_accessLevel = required_accessLevel;
        }

        public override void OnAuthorization(
               System.Web.Http.Controllers.HttpActionContext actionContext) {
            base.OnAuthorization(actionContext);

            if (!HttpContext.Current.User.Identity.IsAuthenticated) {
                throw new HttpException(403, "Forbidden.");
            }

            var repo = new MauloaDemo.Repository.LoginUserRepository();
            var login_user = repo.Find(HttpContext.Current.User.Identity.Name);
            var actual_level = login_user != null ? login_user.access_level : -1;

            if (actual_level < _required_accessLevel) {
                throw new HttpException(403, "Forbidden.");
            }
        }
    }
}