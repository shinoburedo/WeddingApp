using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MauloaDemo.Models;
using MauloaDemo.Repository;
using MauloaDemo.Web.Controllers;

namespace MauloaDemo.Web.Areas.Masters.Controllers
{
    public class HomeController : BaseMasterController
    {
        //
        // GET: /Masters/Masters/
        public ActionResult Index() {
            return View();
        }
	}
}