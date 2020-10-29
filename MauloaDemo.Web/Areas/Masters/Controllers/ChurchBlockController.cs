using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MauloaDemo.Web.Controllers;

namespace MauloaDemo.Web.Areas.Masters.Controllers
{
    public class ChurchBlockController : BaseMasterController
    {
        // GET: Masters/ChurchBlock
        public ActionResult Index() {
            return View();
        }
    }
}