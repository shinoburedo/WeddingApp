using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
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
using CBAF.Attributes;

namespace MauloaDemo.Web.Api {

    public class VendorsController : BaseApiController  {

        private VendorRepository db = new VendorRepository();

        [HttpPost]
        [Route("api/Vendors/Search")]
        [ResponseType(typeof(IEnumerable<Vendor>))]
        public async Task<IHttpActionResult> Search(VendorRepository.SearchParams param) {
            var list = await db.GetListAsync(param);
            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            foreach (var item in list) {
                item.update_date = item.update_date.AddHours(this._loginUser.time_zone);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/Vendors/{id?}")]
        [ResponseType(typeof(Vendor))]
        public async Task<IHttpActionResult> Get(string id = null) {
            Vendor vendor = null;
            try {
                if (string.IsNullOrEmpty(id)) {
                    vendor = new Vendor();
                } else {
                    vendor = await db.FindAsync(id);
                    if (vendor == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }

            return Ok(vendor);
        }


        [HttpPost]
        [Route("api/Vendors/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(Vendor vendor) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(vendor);
                UpperCaseHelper.Apply(vendor);
                LowerCaseHelper.Apply(vendor);
                vendor.last_person = this._loginUser.login_id;
                vendor.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<Vendor>(vendor);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                await db.SaveAsync(vendor, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }


        [HttpPost]
        [Route("api/Vendors/Delete/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete(string id) {
            var status = string.Empty;
            try {
                await db.DeleteAsync(id, this._loginUser);
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