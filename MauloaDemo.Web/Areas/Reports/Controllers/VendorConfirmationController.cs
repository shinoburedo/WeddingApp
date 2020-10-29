using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CBAF;
using MauloaDemo.Models;
using MauloaDemo.Repository;
using MauloaDemo.Web.Controllers;
using MauloaDemo.Web.Helpers;
using WatabeWedding.CatWeb.Reports;

namespace MauloaDemo.Web.Areas.Reports.Controllers
{
    [StaffOnlyFilter, AccessLevelFilter(2)]
    public class VendorConfirmationController : BaseController {

        public ActionResult Index() {
            //int user_level = this.CurrentLoginUser.access_level;
            //if (user_level < 1) {
            //    //return new HttpUnauthorizedResult("You do not have enough autorization for this page."); //← これだとログイン画面が表示されてしまう。
            //    return HttpNotFound("You do not have enough autorization for this page.");
            //}

            return View();
        }

        public ActionResult Preview(string vendor_cd, DateTime date_from, DateTime date_to)
        {
            try {
                var repository = new ArrangementRepository();
                var info = repository.GetVendorConfirmationInfo(vendor_cd, date_from, date_to);
                if (info == null) return HttpNotFound();

                var rpt = new MauloaDemo.Reports.VendorConfirmation(info, this.CurrentLoginUser.date_format, this.CurrentLoginUser.time_format);
                return PDFExportHelper.Run(rpt, this.CurrentRegionInfo.CurrentDestination);

            } catch (Exception ex) {
                return Content(ex.Message);
            }
        }

    }
}