using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CBAF;
using MauloaDemo.Web.Controllers;
using MauloaDemo.Models;
using MauloaDemo.Repository;
using System.Data.Entity.Validation;
using CBAF.Attributes;

namespace MauloaDemo.Web.Api
{
    public class MgmReportController : BaseApiController {

        private MgmReportRepository db = new MgmReportRepository();

        [HttpGet]
        [Route("api/MgmReport/SearchReports")]
        [ResponseType(typeof(IEnumerable<MgmReport>))]
        public async Task<IHttpActionResult> SearchReports(string menu_cd, string rep_cd, string rep_name) {
            if (this._loginUser.access_level < 1) {
                return null;
            }
            var list = await db.GetListAsync(menu_cd, rep_cd, rep_name, true, this._loginUser);
            return Ok(list);
        }

        [HttpGet]
        [Route("api/MgmReport/{rep_cd?}")]
        [ResponseType(typeof(MgmReport))]
        public async Task<IHttpActionResult> GetReport(string rep_cd = null) {
            MgmReport report = null;
            try {
                if (this._loginUser.access_level < 1) {
                    throw new Exception("Authorization error");
                }

                if (string.IsNullOrEmpty(rep_cd)) {
                    report = new MgmReport();
                } else {
                    report = await db.FindAsync(rep_cd, this._loginUser);
                    if (report == null) return NotFound();
                }

                //ListViewの不具合なのか最後のアイテムだけKendoのDatePickerなどが正しく適用されないので、ダミーのアイテムを最後に追加する。(このアイテムは画面上は非表示)
                report.Params.Add(new MgmReportParam() { param_name = "_dummy_", required = false, param_type = "date" });

            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }
            return Ok(report);
        }

        [HttpGet]
        [Route("api/MgmReport/GetComboLists")]
        [ResponseType(typeof(IEnumerable<string>))]
        public async Task<IHttpActionResult> GetComboLists() {
            var list = await db.SearchComboListsAsync(string.Empty, string.Empty);
            var code_list = list.Select(i => i.list_cd);
            return Ok(code_list);
        }

        [HttpGet]
        [Route("api/MgmReport/GetReportStoredProcs")]
        [ResponseType(typeof(IEnumerable<string>))]
        public async Task<IHttpActionResult> GetReportStoredProcs() {
            if (this._loginUser.access_level < 1) return NotFound();

            var startswith = "usp_mgm_rpt_";
            var filter = Request.GetQueryNameValuePairs().ElementAt(1); 
            var str_filter = filter.Value;  //Request.QueryString["filter[filters][0][value]"];
            if (!string.IsNullOrEmpty(str_filter) && str_filter.Length >= 4) {
                startswith = str_filter;
            }
            var list = await db.GetStoreProcsAsync(startswith);
            return Ok(list);
        }

        [HttpGet]
        [Route("api/MgmReport/GetStoredProcParams")]
        [ResponseType(typeof(IEnumerable<MgmReportDao.StoredProcParam>))]
        public async Task<IHttpActionResult> GetStoredProcParams(string proc_name) {
            IEnumerable<MgmReportDao.StoredProcParam> list = null;
            try {
                if (this._loginUser.access_level < 6) return NotFound();
                if (string.IsNullOrEmpty(proc_name)) return NotFound();

                list = await db.GetStoredProcParamsAsync(proc_name);

            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }
            return Ok(list);
        }

        [HttpPost]
        [Route("api/MgmReport/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(MgmReport mgmReport) {
            var status = string.Empty;
            try {
                if (this._loginUser.access_level < 6) {
                    throw new Exception("Authorization error");
                }

                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(mgmReport);
                UpperCaseHelper.Apply(mgmReport);
                LowerCaseHelper.Apply(mgmReport);
                mgmReport.last_person = this._loginUser.login_id;
                mgmReport.update_date = DateTime.UtcNow;

                if (mgmReport.Params != null) {
                    foreach (var p in mgmReport.Params) {
                        p.param_id = 0;
                        p.rep_cd = mgmReport.rep_cd;
                        p.last_person = mgmReport.last_person;
                    }
                }

                //ここで再度検証を実行。(エラーがあれば例外を発生する。)
                this.Validate<MgmReport>(mgmReport);
                if (!ModelState.IsValid) {
                    var errorMsg = "Validation error: \n";
                    foreach (var value in ModelState.Values) {
                        foreach (var err in value.Errors) {
                            errorMsg += string.Format("\n{0}", err.ErrorMessage);
                        }
                    }
                    throw new Exception(errorMsg);
                }

                await Task.Run(() => {
                    db.Save(mgmReport, this._loginUser);
                });

                status = "ok";
            } catch (DbEntityValidationException ex) {
                var errorMsg = "Validation error: \n";
                foreach (var err in ex.EntityValidationErrors) {
                    foreach (var error in err.ValidationErrors) {
                        errorMsg += string.Format("\n{0}", error.ErrorMessage);
                    }
                }
                status = errorMsg;
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                status = ex.Message;
            }

            return Ok(status);
        }

        [HttpPost]
        [Route("api/MgmReport/Delete/{rep_cd}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete(string rep_cd) {
            var status = string.Empty;
            try {
                if (this._loginUser.access_level < 6) {
                    throw new Exception("Authorization error");
                }

                await Task.Run(() => db.Delete(rep_cd, this._loginUser));
                status = "ok";
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                status = "error: " + ex.Message;
            }

            return Ok(status);
        }



        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
