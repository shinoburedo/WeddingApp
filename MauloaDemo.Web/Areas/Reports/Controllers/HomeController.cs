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
    [StaffOnlyFilter, AccessLevelFilter(3)]
    public class HomeController : BaseController {

        public ActionResult Index() {
            //int user_level = this.CurrentLoginUser.access_level;
            //if (user_level < 1) {
            //    //return new HttpUnauthorizedResult("You do not have enough autorization for this page."); //← これだとログイン画面が表示されてしまう。
            //    return HttpNotFound("You do not have enough autorization for this page.");
            //}

            IEnumerable<MgmReport> list = null;
            try {
                var repository = new MgmReportRepository();
                list = repository.GetList("Management", string.Empty, string.Empty, false, this.CurrentLoginUser);
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                ViewBag.Error = ex.Message;
            }
            return View(list);
        }

        [AccessLevelFilter(6)]
        public ActionResult ManageReports() {
            //int user_level = this.CurrentLoginUser.access_level;
            //if (user_level < 6) {
            //    //return new HttpUnauthorizedResult("You do not have enough autorization for this page."); //← これだとログイン画面が表示されてしまう。
            //    return HttpNotFound("You do not have enough autorization for this page.");
            //}

            return View();
        }

        public ActionResult Report(string cd) {
            ViewBag.RepCd = cd;
            return View();
        }

        public ActionResult ExecuteReport(string _rep_cd) {
            try {
                var repository = new MgmReportRepository();
                var report = repository.Find(_rep_cd, this.CurrentLoginUser);
                if (report == null) {
                    throw new Exception(string.Format("Report '{0}' not found.", _rep_cd));
                }

                foreach (string key in Request.QueryString.Keys) {
                    if (key != null) {                              //URLの最後に「&」が付いているとkeyがnullになるのでその場合は無視。
                        //パラメータ名は常に小文字で比較する。
                        var param_name = key.ToLower();
                        var value = Request.QueryString.Get(key);

                        //param_nameが一致するパラメータを探す。
                        var p = report.Params.FirstOrDefault(m => m.param_name == param_name);
                        if (p != null) {
                            p.input_value = value;
                        }
                    }
                }

                DataTable dt = repository.ExecuteReport(report, this.CurrentLoginUser);
                if (dt.Rows.Count == 0) {
                    throw new Exception("No records found.");
                }

                var filename = string.IsNullOrEmpty(report.output_name) ? report.rep_cd : report.output_name;

                if ("csv".Equals(TypeHelper.GetStrTrim(report.output_type).ToLower())) {
                    //Outputファイル名
                    filename = filename + (filename.EndsWith(".csv") ? "" : ".csv");

                    //CSVファイルを出力。
                    return new CSVResult(dt, report.write_header, filename);

                } else {
                    //Outputファイル名
                    filename = filename + (filename.EndsWith(".xls") ? "" : ".xls");

                    //Templateパス           
                    var sPath = TypeHelper.GetStrTrim(ManagementReportGeneric.GetTemplatePath(this.CurrentRegionInfo.CurrentDestination));
                    sPath += report.excel_name;

                    //Excelファイルを出力。
                    return new EXCELResult(ManagementReportGeneric.CreateExcel(sPath, report.sheet_num, report.start_pos, report.write_header, dt), filename);
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                ViewBag.Error = ex.Message;
            }
            return View();
        }

        public ActionResult PreviewDailyMovement(
            DateTime? wed_date,
            string church_cd,
            string agent_cd) {
            try {
                var repository = new CustomerRepository();
                var info = repository.GetDailyMovementReportList(wed_date, church_cd, agent_cd).ToList();
                if (info == null) return HttpNotFound();

                var rpt = new MauloaDemo.Reports.DailyMovement(info, this.CurrentLoginUser.date_format, this.CurrentLoginUser.time_format, true, this.CurrentLoginUser);
                return PDFExportHelper.Run(rpt, this.CurrentRegionInfo.CurrentDestination);

            } catch (Exception ex) {
                return Content(ex.Message);
            }
        }


    }
}