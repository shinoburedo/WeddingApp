using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MauloaDemo.Models;
using MauloaDemo.Repository;


namespace MauloaDemo.Web.Controllers
{
    public class NoticeController : BaseController
    {
        //
        // GET: /Home/

        public ActionResult Index() {
            return View();
        }

    }
}
