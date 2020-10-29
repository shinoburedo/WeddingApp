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
using MauloaDemo.Models.Combined;

namespace MauloaDemo.Web.Areas.Reports.Controllers
{
    [StaffOnlyFilter, AccessLevelFilter(2)]
    public class BookingReportController : BaseController {

        public ActionResult Index() {
            //int user_level = this.CurrentLoginUser.access_level;
            //if (user_level < 1) {
            //    //return new HttpUnauthorizedResult("You do not have enough autorization for this page."); //← これだとログイン画面が表示されてしまう。
            //    return HttpNotFound("You do not have enough autorization for this page.");
            //}

            return View();
        }

        public ActionResult Preview(DateTime? date_from, DateTime? date_to, string agent_cd,
            string church_cd, string area_cd, string item_type, string vendor_cd, string item_cd,
            bool include_cust_cxl, bool include_sales_cxl, bool not_finalized_only, BookingReport.SortBy? sort_by) {
            var param = new BookingReport.SearchParam() {
                date_from = date_from,
                date_to = date_to,
                agent_cd = agent_cd,
                church_cd = church_cd,
                area_cd = area_cd,
                item_cd = item_cd,
                item_type = item_type,
                vendor_cd = vendor_cd,
                include_cust_cxl = include_cust_cxl,
                include_sales_cxl = include_sales_cxl,
                not_finalized_only = not_finalized_only,
                sort_by = sort_by
            };
            try {
                var repository = new SalesRepository();
                var info = repository.GetBookingReport(param).ToList();
                if (info == null) return HttpNotFound();

                var rpt = new MauloaDemo.Reports.BookingReport(info, param, this.CurrentLoginUser.date_format, this.CurrentLoginUser.time_format);
                return PDFExportHelper.Run(rpt, this.CurrentRegionInfo.CurrentDestination);

            } catch (Exception ex) {
                return Content(ex.Message);
            }
        }

    }
}