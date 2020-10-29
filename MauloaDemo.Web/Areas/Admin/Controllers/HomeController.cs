using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MauloaDemo.Models;
using MauloaDemo.Repository;
using MauloaDemo.Web.Controllers;


namespace MauloaDemo.Web.Areas.Admin.Controllers
{
    [StaffOnlyFilter, AccessLevelFilter(4)]
    public class HomeController : BaseController
    {
        //
        // GET: /Admin/Home/
        public ActionResult Index()
        {
            return View();
        }
	}
}