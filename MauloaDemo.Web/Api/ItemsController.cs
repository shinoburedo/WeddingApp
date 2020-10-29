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

    public class ItemsController : BaseApiController {

        private ItemRepository db = new ItemRepository();

        [HttpGet]
        [Route("api/Items")]
        [ResponseType(typeof(IEnumerable<Item>))]
        public async Task<IHttpActionResult> GetList(string item_cd = null, string description = null) {
            var list = await db.GetListAsync(item_cd, description);
            return Ok(list);
        }

        // POST api/items/searchWithPrice
        /// <summary>
        /// 価格付きのオプション一覧を検索して返す。
        /// </summary>
        /// <param name="prms"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/items/searchWithPrice")]
        public async Task<IEnumerable<ItemWithPrice>> SearchWithPrice(ItemRepository.SearchParams prms) {

            //エージェントユーザーの場合はsub_agent_cdで制限を掛ける。
            if (_loginUser.IsAgent()) {
                var subAgentRepo = new SubAgentRepository(db);
                if (!subAgentRepo.HasAccessTo(_loginUser.sub_agent_cd, prms.sub_agent_cd)) {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }

                prms.open_to_japan_only = true;
            }

            if (string.IsNullOrEmpty(prms.sub_agent_cd)) {
                prms.sub_agent_cd = _loginUser.sub_agent_cd;
            }

            return await db.GetListWithPrice(prms);
        }

        /// <summary>
        /// パッケージプランの一覧を検索して返す。
        /// </summary>
        /// <param name="prms"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/items/search")]
        public async Task<IEnumerable<Item>> Search(ItemRepository.SearchParams prms) {
            //エージェントユーザーの場合はsub_agent_cdで制限を掛ける。
            if (_loginUser.IsAgent()) {
                if (!string.IsNullOrEmpty(prms.sub_agent_cd)) {
                    var subAgentRepo = new SubAgentRepository(db);
                    if (!subAgentRepo.HasAccessTo(_loginUser.sub_agent_cd, prms.sub_agent_cd)) {
                        throw new HttpResponseException(HttpStatusCode.NotFound);
                    }
                } else { 
                    prms.sub_agent_cd = _loginUser.sub_agent_cd;
                }
                prms.open_to_japan_only = true;
            }
            var list = await db.GetList(prms);
            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            foreach (var item in list) {
                item.update_date = item.update_date.AddHours(this._loginUser.time_zone);
            }
            return list;
        }

        [Route("api/items/getchildren")]
        public async Task<IEnumerable<Item>> GetChildren(string item_cd) {
            return await db.GetChildren(item_cd);
        }



        // GET api/Items/5
        [ResponseType(typeof(Item))]
        public async Task<IHttpActionResult> GetItem(string id) {
            Item area = await db.FindAsync(id);
            if (area == null) {
                return NotFound();
            }

            return Ok(area);
        }


        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        [Route("api/Items/{id?}")]
        [ResponseType(typeof(Item))]
        public async Task<IHttpActionResult> Get(string id = null) {
            Item item = null;
            try {
                if (string.IsNullOrEmpty(id)) {
                    item = new Item();
                } else {
                    item = await db.FindAsync(id);
                    if (item == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }

            return Ok(item);
        }

        [HttpPost]
        [Route("api/Items/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(Item item) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(item);
                UpperCaseHelper.Apply(item);
                LowerCaseHelper.Apply(item);
                item.last_person = this._loginUser.login_id;
                item.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<Item>(item);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                await db.SaveAsync(item, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }

        [HttpPost]
        [Route("api/Items/Delete/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete(string id) {
            var status = string.Empty;
            try {
                await db.DeleteAsync(id, this._loginUser);
                await new ItemPriceRepository().DeleteByItemCdAsync(id, this._loginUser);
                await new ItemVendorRepository().DeleteByItemCdAsync(id, this._loginUser);
                await new ItemCostRepository().DeleteByItemCdAsync(id, this._loginUser);
                status = "ok";
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                status = "error: " + ex.Message;
            }
            return Ok(status);
        }

    }

}