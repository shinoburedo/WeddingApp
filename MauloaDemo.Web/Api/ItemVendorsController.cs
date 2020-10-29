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
using MauloaDemo.Models.Combined;
using MauloaDemo.Repository;
using CBAF.Attributes;

namespace MauloaDemo.Web.Api {

    public class ItemVendorsController : BaseApiController {

        private ItemVendorRepository db = new ItemVendorRepository();

        [HttpPost]
        [Route("api/itemvendors/search")]
        [ResponseType(typeof(IEnumerable<ItemVendor>))]
        public async Task<IHttpActionResult> Search(ItemVendorRepository.SearchParams param) {
            var list = await db.GetListAsync(param);
            return Ok(list);
        }

        // GET api/ItemVendors/5
        [ResponseType(typeof(ItemVendor))]
        public async Task<IHttpActionResult> GetItem(int id) {
            ItemVendor item_vendor = await db.FindAsync(id);
            if (item_vendor == null) {
                return NotFound();
            }

            return Ok(item_vendor);
        }


        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [Route("api/ItemVendors/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(ItemVendor item_vendor) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(item_vendor);
                UpperCaseHelper.Apply(item_vendor);
                LowerCaseHelper.Apply(item_vendor);
                item_vendor.last_person = this._loginUser.login_id;
                item_vendor.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<ItemVendor>(item_vendor);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                await db.SaveAsync(item_vendor, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }


        [HttpPost]
        [Route("api/ItemVendors/Delete/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete(int id) {
            var status = string.Empty;
            try {
                var item_vendor = db.Find(id);
                await db.DeleteAsync(id, this._loginUser);
                await new ItemCostRepository().DeleteByItemVendorAsync(item_vendor.item_cd, item_vendor.vendor_cd, this._loginUser);
                status = "ok";
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                status = "error: " + ex.Message;
            }
            return Ok(status);
        }

    }

}