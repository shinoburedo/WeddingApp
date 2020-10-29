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

    public class ItemTypesController : BaseApiController {

        private ItemTypeRepository db = new ItemTypeRepository();

        // GET api/ItemTypes
        public async Task<IEnumerable<ItemType>> GetItemTypes() {
            return await db.GetListAsync();
        }

        [HttpPost]
        [Route("api/itemtypes/search")]
        [ResponseType(typeof(IEnumerable<ItemType>))]
        public async Task<IEnumerable<ItemType>> Search(ItemTypeRepository.SearchParams param) {
            var list = await db.GetListAsync(param);
            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            foreach (var item in list) {
                item.update_date = item.update_date.AddHours(this._loginUser.time_zone);
            }

            return list;
        }


        //[HttpGet]
        //[Route("api/ItemTypes/Search")]
        //[ResponseType(typeof(IEnumerable<ItemType>))]
        //public async Task<IHttpActionResult> Search(ItemTypeRepository.SearchParams param) {
        //    var list = await db.GetListAsync(param);
        //    return Ok(list);
        //}

        //// GET api/ItemTypes/XXX
        //[ResponseType(typeof(ItemType))]
        //public async Task<IHttpActionResult> GetItemType(string id) {
        //    ItemType itemType = await db.FindAsync(id);
        //    if (itemType == null) return NotFound();
        //    return Ok(itemType);
        //}

        [HttpGet]
        [Route("api/ItemTypes/{id?}")]
        [ResponseType(typeof(ItemType))]
        public async Task<IHttpActionResult> Get(string id = null) {
            ItemType itemType = null;
            try {
                if (string.IsNullOrEmpty(id)) {
                    itemType = new ItemType();
                } else {
                    itemType = await db.FindAsync(id);
                    if (itemType == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }

            return Ok(itemType);
        }


        [HttpPost]
        [Route("api/ItemTypes/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(ItemType itemType) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(itemType);
                UpperCaseHelper.Apply(itemType);
                LowerCaseHelper.Apply(itemType);
                itemType.last_person = this._loginUser.login_id;
                itemType.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<ItemType>(itemType);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                await db.SaveAsync(itemType, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }


        [HttpPost]
        [Route("api/ItemTypes/Delete/{id}")]
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