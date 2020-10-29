using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using MauloaDemo.Repository;
using MauloaDemo.Web.Controllers;

namespace MauloaDemo.Web.Areas.Customers.Controllers
{
    [AccessLevelFilter(2)]
    public class HomeController : BaseController
    {
        //
        // GET: /customers/home/
        public ActionResult Index() {
            var prms = new CustomerRepository.SearchParams() {
                sub_agent_cd = this.CurrentLoginUser.HasChildAgents ? "" : this.CurrentLoginUser.sub_agent_cd
            };
            return View(prms); 
        }

        //
        // GET: /customers/home/create
        [AccessLevelFilter(3)]
        public ActionResult Create() {
            var customerInfo = new CustomerInfo();
            customerInfo.Customer.sub_agent_cd = this.CurrentLoginUser.HasChildAgents ? "" : this.CurrentLoginUser.sub_agent_cd;

            ViewBag.Mode = "create";
            return View("Edit", customerInfo);
        }

        //
        // GET: /customers/home/edit
        public ActionResult Edit(string id) {
            //ログインユーザーのカスタマーへの権限をチェック。
            var loginUserRepo = new LoginUserRepository();
            if (!loginUserRepo.CanViewCustomer(this.CurrentLoginUser, id)) {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden, "Not authorized.");
            }

            var customerInfo = new CustomerInfo();
            customerInfo.Customer.c_num = id;
            customerInfo.Customer.sub_agent_cd = this.CurrentLoginUser.IsAgent() ? this.CurrentLoginUser.sub_agent_cd : "";

            ViewBag.Mode = "edit";
            return View("Edit", customerInfo);
        }


        //public ActionResult PlanSearchDialog() {
        //    return View();
        //}

        [AccessLevelFilter(3)]
        public ActionResult ScheduleDialog() {
            return View();
        }


        public async Task<ActionResult> PreviewChurchOrderSheet(string c_num, int id) {
            try {
                //ログインユーザーのカスタマーへの権限をチェック。
                var loginUserRepo = new LoginUserRepository();
                if (!loginUserRepo.CanViewCustomer(this.CurrentLoginUser, c_num)) {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden, "Not authorized.");
                }

                var wedInfoRepository = new WedInfoRepository();
                var info = await Task<ChurchOrderSheetInfo>.Run(() => wedInfoRepository.GetChurchOrderSheetInfo(c_num, id, this.CurrentLoginUser));
                if (info == null) return HttpNotFound();

                var rpt = new MauloaDemo.Reports.ChurchOrderSheet(info);
                return PDFExportHelper.Run(rpt, this.CurrentRegionInfo.CurrentDestination);

            } catch (Exception ex) {
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> PreviewVendorOrderSheet(string c_num, int id) {
            try {
                //ログインユーザーのカスタマーへの権限をチェック。
                var loginUserRepo = new LoginUserRepository();
                if (!loginUserRepo.CanViewCustomer(this.CurrentLoginUser, c_num)) {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden, "Not authorized.");
                }

                var repository = new ArrangementRepository();
                var info = await repository.GetVendorOrderSheetInfo(c_num, id, this.CurrentLoginUser);
                if (info == null) return HttpNotFound();

                var rpt = new MauloaDemo.Reports.VendorOrderSheet(info, this.CurrentLoginUser.date_format, this.CurrentLoginUser.time_format);
                return PDFExportHelper.Run(rpt, this.CurrentRegionInfo.CurrentDestination);

            } catch (Exception ex) {
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> PreviewFolderSheet(string id, bool english = false) {
            english = this.CurrentLoginUser.Language == "en";
            try {
                if (string.IsNullOrEmpty(id)) {
                    throw new Exception("Invalid parameter.");
                }
                //ログインユーザーのカスタマーへの権限をチェック。
                var loginUserRepo = new LoginUserRepository();
                if (!loginUserRepo.CanViewCustomer(this.CurrentLoginUser, id)) {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden, "Not authorized.");
                }

                var repository = new CustomerRepository();
                var info = await repository.GetFolderSheetInfo(id, english, this.CurrentLoginUser);
                if (info == null) return HttpNotFound();

                var rpt = new MauloaDemo.Reports.FinalInfoSheet(info, this.CurrentLoginUser.date_format, this.CurrentLoginUser.time_format, english, this.CurrentLoginUser);
                return PDFExportHelper.Run(rpt, this.CurrentRegionInfo.CurrentDestination);

            } catch (Exception ex) {
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> PreviewWeddingInfo(string id, bool english = false) {
            english = this.CurrentLoginUser.Language == "en";
            try {
                if (string.IsNullOrEmpty(id)) {
                    throw new Exception("Invalid parameter.");
                }
                //ログインユーザーのカスタマーへの権限をチェック。
                var loginUserRepo = new LoginUserRepository();
                if (!loginUserRepo.CanViewCustomer(this.CurrentLoginUser, id)) {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden, "Not authorized.");
                }

                var repository = new CustomerRepository();
                var info = await repository.GetFolderSheetInfo(id, english, this.CurrentLoginUser);
                if (info == null) return HttpNotFound();

                var rpt = new MauloaDemo.Reports.WeddingInfoSheet(info, this.CurrentLoginUser.date_format, this.CurrentLoginUser.time_format, english, this.CurrentLoginUser);
                return PDFExportHelper.Run(rpt, this.CurrentRegionInfo.CurrentDestination);

            } catch (Exception ex) {
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> PreviewScheduleSheet(string id, bool english = false) {
            try {
                if (string.IsNullOrEmpty(id)) {
                    throw new Exception("Invalid parameter.");
                }
                //ログインユーザーのカスタマーへの権限をチェック。
                var loginUserRepo = new LoginUserRepository();
                if (!loginUserRepo.CanViewCustomer(this.CurrentLoginUser, id)) {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden, "Not authorized.");
                }

                var repository = new SchedulePhraseRepository();
                var info = await Task<ScheduleSheetInfo>.Run(() => repository.GetScheduleSheetInfo(id, english, this.CurrentLoginUser));
                if (info == null) return HttpNotFound();

                var rpt = new MauloaDemo.Reports.ScheduleSheet(info);
                return PDFExportHelper.Run(rpt, this.CurrentRegionInfo.CurrentDestination);

            } catch (Exception ex) {
                return Content(ex.Message);
            }
        }

        public async Task<ActionResult> PreviewCostumeOrderSheet(string c_num)
        {
            try {
                //ログインユーザーのカスタマーへの権限をチェック。
                var loginUserRepo = new LoginUserRepository();
                if (!loginUserRepo.CanViewCustomer(this.CurrentLoginUser, c_num))
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.Forbidden, "Not authorized.");
                }
                //CosInfo
                var repository = new CosInfoRepository();
                var info = await Task<CostumeOrderSheetInfo>.Run(() => repository.GetCosInfo(c_num, this.CurrentLoginUser));
                if (info == null) return HttpNotFound();
                var cos_info = Task.Run(() => repository.GetList(c_num)).Result.ToList();
                //ScheduleInfo
                var schedule_repository = new SchedulePhraseRepository();
                var schedule_info = await Task<ScheduleSheetInfo>.Run(() => schedule_repository.GetScheduleSheetInfo(c_num, true, this.CurrentLoginUser));
                //WedPlanInfo
                var order_info = await Task<IEnumerable<SalesListItem>>.Run(() => repository.GetWedInfoChildrenByCNum(c_num, this.CurrentLoginUser));

                var rpt = new MauloaDemo.Reports.CostumeOrderSheet(info, this.CurrentLoginUser.date_format, this.CurrentLoginUser.time_format, schedule_info, order_info.ToList(), cos_info);
                return PDFExportHelper.Run(rpt, this.CurrentRegionInfo.CurrentDestination);

            } catch (Exception ex) {
                return Content(ex.Message);
            }
        }

    }
}