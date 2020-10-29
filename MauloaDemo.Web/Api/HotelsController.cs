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

    public class HotelsController : BaseApiController  {

        private HotelRepository db = new HotelRepository();

        [HttpPost]
        [Route("api/Hotels/Search")]
        [ResponseType(typeof(IEnumerable<Hotel>))]
        public async Task<IHttpActionResult> Search(HotelRepository.SearchParams param) {
            var list = await db.GetListAsync(param);
            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            foreach (var item in list) {
                item.update_date = item.update_date.AddHours(this._loginUser.time_zone);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/Hotels/{id?}")]
        [ResponseType(typeof(Hotel))]
        public async Task<IHttpActionResult> Get(string id = null) {
            Hotel hotel = null;
            try {
                if (string.IsNullOrEmpty(id)) {
                    hotel = new Hotel();
                } else {
                    hotel = await db.FindAsync(id);
                    if (hotel == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }

            return Ok(hotel);
        }


        [HttpPost]
        [Route("api/Hotels/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(Hotel hotel) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(hotel);
                UpperCaseHelper.Apply(hotel);
                LowerCaseHelper.Apply(hotel);
                hotel.last_person = this._loginUser.login_id;
                hotel.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<Hotel>(hotel);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                await db.SaveAsync(hotel, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }


        [HttpPost]
        [Route("api/Hotels/Delete/{id}")]
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