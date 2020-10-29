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

    public class ItemOptionsController : BaseApiController {

        private ItemOptionRepository db = new ItemOptionRepository();

        [HttpPost]
        [Route("api/itemoptions/search")]
        [ResponseType(typeof(IEnumerable<ItemGrouping>))]
        public async Task<IEnumerable<ItemGrouping>> Search(ItemOptionRepository.SearchParams param) {
            return await db.GetListAsync(param);
        }

        [HttpPost]
        [Route("api/itemoptions/searchoptions")]
        [ResponseType(typeof(IEnumerable<ItemGrouping>))]
        public async Task<IEnumerable<ItemGrouping>> SearchOptions(ItemOptionRepository.SearchParams param) {
            return await db.GetOptionListAsync(param);
        }

        //[HttpGet]
        //[Route("api/ItemOptions")]
        //[ResponseType(typeof(IEnumerable<ItemOption>))]
        //public async Task<IHttpActionResult> GetList(string item_cd = null, string description = null) {
        //    var list = await db.GetListAsync(item_cd, description);
        //    return Ok(list);
        //}


        ///// <summary>
        ///// パッケージプランの一覧を検索して返す。
        ///// </summary>
        ///// <param name="prms"></param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("api/Itemoptions/search")]
        //public async Task<IEnumerable<ItemOption>> Search(ItemOptionRepository.SearchParams prms) {
        //    return await db.GetList(prms);
        //}

        //[Route("api/Itemoptions/getchildren")]
        //public async Task<IEnumerable<ItemOption>> GetChildren(string item_cd) {
        //    return await db.GetChildren(item_cd);
        //}



        // GET api/ItemOptions/5
        [ResponseType(typeof(ItemOption))]
        public async Task<IHttpActionResult> GetItemOption(string item_cd, string child_cd) {
            ItemOption item = await db.FindAsync(item_cd, child_cd);
            if (item == null) {
                return NotFound();
            }

            return Ok(item);
        }


        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        [Route("api/ItemOptions/{item_cd}/{child_cd}")]
        [ResponseType(typeof(ItemOption))]
        public async Task<IHttpActionResult> Get(string item_cd = null, string child_cd = null) {
            ItemOption item = null;
            try {
                if (string.IsNullOrEmpty(item_cd) && string.IsNullOrEmpty(child_cd)) {
                    item = new ItemOption();
                } else {
                    item = await db.FindAsync(item_cd, child_cd);
                    if (item == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }

            return Ok(item);
        }

        [HttpPost]
        [Route("api/ItemOptions/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(ItemOption item) {
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
                this.Validate<ItemOption>(item);
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
        [Route("api/itemoptions/savelist")]
        [ResponseType(typeof(string))]
        //public async Task<IHttpActionResult> SaveList(string item_cd, string list) {
        public async Task<IHttpActionResult> SaveList(ItemGroupingListParam param) {
            var status = string.Empty;
            try {
                await db.SaveListAsync(param.item_cd, param.list, param.is_new, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }

        [HttpPost]
        [Route("api/ItemOptions/Delete/{item_cd}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete(string item_cd = null) {
            var status = string.Empty;
            try {
                await db.DeleteAsync(item_cd, this._loginUser);
                status = "ok";
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                status = "error: " + ex.Message;
            }
            return Ok(status);
        }

        [HttpPost]
        [Route("api/ItemOptions/DeleteOption/{item_cd}/{child_cd}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> DeleteOption(string item_cd = null, string child_cd = null) {
            var status = string.Empty;
            try {
                await db.DeleteOptionAsync(item_cd, child_cd, this._loginUser);
                status = "ok";
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                status = "error: " + ex.Message;
            }
            return Ok(status);
        }

    }

}