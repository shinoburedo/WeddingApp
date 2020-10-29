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

    public class HolidaysController : BaseApiController {

        private HolidayRepository db = new HolidayRepository();

        [HttpGet]
        [Route("api/Holidays")]
        [ResponseType(typeof(IEnumerable<Holiday>))]
        public async Task<IHttpActionResult> GetList(DateTime? holiday = null, string description = null) {
            var list = await db.GetListAsync(holiday, description);
            return Ok(list);
        }

        [HttpPost]
        [Route("api/Holidays/Search")]
        [ResponseType(typeof(IEnumerable<Holiday>))]
        public async Task<IHttpActionResult> Search(DateTime? holiday = null, string description = null) {
            var list = await db.GetListAsync(holiday, description);
            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            foreach (var item in list) {
                item.update_date = item.update_date.AddHours(this._loginUser.time_zone);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/Holidays/{id?}")]
        [ResponseType(typeof(Holiday))]
        public async Task<IHttpActionResult> Get(string id = null) {
            Holiday holiday = null;
            try {
                if (string.IsNullOrEmpty(id)) {
                    holiday = new Holiday();
                } else {
                    DateTime dt = System.DateTime.ParseExact(id,
                                "yyyyMMddHHmmss",
                                System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                System.Globalization.DateTimeStyles.None);
                    holiday = await db.FindAsync(dt);
                    if (holiday == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }

            return Ok(holiday);
        }


        [HttpPost]
        [Route("api/Holidays/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(Holiday holiday) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(holiday);
                UpperCaseHelper.Apply(holiday);
                LowerCaseHelper.Apply(holiday);
                holiday.last_person = this._loginUser.login_id;
                holiday.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<Holiday>(holiday);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                await db.SaveAsync(holiday, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }


        [HttpPost]
        [Route("api/Holidays/Delete/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete(string id) {
            var status = string.Empty;
            try {
                if (!string.IsNullOrEmpty(id)) {
                    DateTime dt = System.DateTime.ParseExact(id,
                                "yyyyMMddHHmmss",
                                System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                System.Globalization.DateTimeStyles.None);
                    await db.DeleteAsync(dt, this._loginUser);
                    status = "ok";
                }

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