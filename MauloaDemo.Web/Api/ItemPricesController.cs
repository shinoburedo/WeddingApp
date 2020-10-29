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

    public class ItemPricesController : BaseApiController {

        private ItemPriceRepository db = new ItemPriceRepository();

        [HttpPost]
        [Route("api/itemprices/search")]
        [ResponseType(typeof(IEnumerable<ItemPrice>))]
        public async Task<IHttpActionResult> Search(ItemPriceRepository.SearchParams param) {
            var list = await db.GetListAsync(param);
            return Ok(list);
        }

        // GET api/ItemPrices/5
        [ResponseType(typeof(ItemPrice))]
        public async Task<IHttpActionResult> GetItem(int id) {
            ItemPrice item_price = await db.FindAsync(id);
            if (item_price == null) {
                return NotFound();
            }

            return Ok(item_price);
        }


        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [Route("api/ItemPrices/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(ItemPrice item_price) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(item_price);
                UpperCaseHelper.Apply(item_price);
                LowerCaseHelper.Apply(item_price);
                item_price.last_person = this._loginUser.login_id;
                item_price.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<ItemPrice>(item_price);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                await db.SaveAsync(item_price, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }


        [HttpPost]
        [Route("api/ItemPrices/Delete/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete(int id) {
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

    }

}