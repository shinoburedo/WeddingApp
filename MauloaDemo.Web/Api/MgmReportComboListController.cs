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

namespace MauloaDemo.Web.Api {

    public class MgmReportComboListController : BaseApiController {

        private MgmReportRepository db = new MgmReportRepository();

        [HttpGet]
        [Route("api/MgmReportComboList/SearchComboLists")]
        [ResponseType(typeof(IEnumerable<MgmReportComboList>))]
        public async Task<IHttpActionResult> SearchComboLists(string list_cd, string description) {
            var list = await db.SearchComboListsAsync(list_cd, description);
            return Ok(list);
        }

        [HttpGet]
        [Route("api/MgmReportComboList/GetComboList/{list_cd?}")]
        [ResponseType(typeof(MgmReportComboList))]
        public async Task<IHttpActionResult> GetComboList(string list_cd = null) {
            MgmReportComboList item = null;
            try {
                if (string.IsNullOrEmpty(list_cd)) {
                    item = new MgmReportComboList();
                } else {
                    item = await db.FindComboListAsync(list_cd);
                    if (item == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }
            return Ok(item);
        }


        [HttpPost]
        [Route("api/MgmReportComboList/SaveComboList")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> SaveComboList(MgmReportComboList comboList) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(comboList);
                UpperCaseHelper.Apply(comboList);
                LowerCaseHelper.Apply(comboList);
                comboList.last_person = this._loginUser.login_id;
                comboList.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあれば例外を発生する。)
                this.Validate<MgmReportComboList>(comboList);
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
                    db.SaveComboList(comboList, this._loginUser);
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
        [Route("api/MgmReportComboList/DeleteComboList/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> DeleteComboList(string id) {
            var status = string.Empty;
            try {
                await db.DeleteComboListAsync(id, this._loginUser);
                status ="ok";
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                status = "error: " + ex.Message;
            }
            return Ok(status);
        }



        [HttpGet]
        [Route("api/MgmReportComboList/GetComboListData/{id}")]
        [ResponseType(typeof(List<MgmReportComboList.ValueTextPair>))]
        public async Task<IHttpActionResult> GetComboListData(string id) {

            List<MgmReportComboList.ValueTextPair> list = null;
            try {
                if (string.IsNullOrEmpty(id)) return NotFound();

                if (id.StartsWith("[")) {
                    list = MgmReportComboList.ParseString(id);
                } else {
                    var comboList = await db.FindComboListAsync(id);
                    if (comboList == null) return NotFound();
                    list = await db.ExecuteComboListAsync(id);
                }

            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }
            return Ok(list);
        }


        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
