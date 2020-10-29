using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity.Validation;
using System.Configuration;
using CBAF;
using MauloaDemo.Web;
using MauloaDemo.Web.Controllers;
using MauloaDemo.Models;
using MauloaDemo.Repository;
using MauloaDemo.Reports;

namespace MauloaDemo.Web.Areas.Reports.Controllers
{
    [NoClientCache]                                 //IEだとAjaxでGETするとブラウザのキャッシュが使われてしまう問題の対処。
    [StaffOnlyFilter, AccessLevelFilter(2)]
    public class ComboListController : BaseController
    {
        //
        // GET: /Reports/ComboList/

        public ActionResult Index() {
            return View();
        }

    }
}
