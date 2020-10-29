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
using CBAF.Attributes;

namespace MauloaDemo.Web.Api {

    public class PickupPlacesController : BaseApiController {

        private PickupPlaceRepository db = new PickupPlaceRepository();

        // GET api/PickupPlaces
        public async Task<IEnumerable<PickupPlace>> GetPickupPlaces() {
            return await db.GetList();
        }

        // POST api/PickupPlaces/search
        [HttpPost]
        [Route("api/PickupPlaces/search")]
        public async Task<IEnumerable<PickupPlace>> search(PickupPlaceRepository.SearchParams prms) {
            var list = await db.GetList(prms);
            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            foreach (var item in list) {
                item.update_date = item.update_date.AddHours(this._loginUser.time_zone);
            }
            return list;
        }

        // GET api/PickupPlaces/5
        [ResponseType(typeof(PickupPlace))]
        public async Task<IHttpActionResult> GetPickupPlace(string id)
        {
            PickupPlace PickupPlace = await db.FindAsync(id);
            if (PickupPlace == null) {
                return NotFound();
            }

            return Ok(PickupPlace);
        }

        [HttpGet]
        [Route("api/PickupPlaces/{id?}")]
        [ResponseType(typeof(PickupPlace))]
        public async Task<IHttpActionResult> Get(int? id = null) {
            PickupPlace pickupPlace = null;
            try {
                if (id == null || id == 0) {
                    pickupPlace = new PickupPlace();
                } else {
                    pickupPlace = await db.FindAsync(id);
                    if (pickupPlace == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }

            return Ok(pickupPlace);
        }

        [HttpPost]
        [Route("api/PickupPlaces/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(PickupPlace pickupPlace) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(pickupPlace);
                UpperCaseHelper.Apply(pickupPlace);
                LowerCaseHelper.Apply(pickupPlace);
                pickupPlace.last_person = this._loginUser.login_id;
                pickupPlace.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<PickupPlace>(pickupPlace);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                await db.SaveAsync(pickupPlace, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }


        [HttpPost]
        [Route("api/PickupPlaces/Delete/{id}")]
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}