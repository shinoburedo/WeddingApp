using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MauloaDemo.Models;
using MauloaDemo.Repository;


namespace MauloaDemo.Web.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index() {
            return Redirect(Url.Action("Index", "Notice"));
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult HorizontalMenu() {
            return PartialView("_HorizontalMenu");
        }

        [AllowAnonymous]
        [ChildActionOnly]
        public ActionResult LoginUser() {
            return PartialView("_LoginUser");
        }



    }
}
