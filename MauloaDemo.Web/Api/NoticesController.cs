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

    public class NoticesController : BaseApiController {

        private NoticeRepository db = new NoticeRepository();

        // POST api/Notices/search
        [HttpPost]
        [Route("api/Notices/search")]
        public async Task<IEnumerable<Notice>> search(NoticeRepository.SearchParams prms) {
            var list = await db.GetListAsync(prms);
            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            foreach (var item in list) {
                item.update_date = item.update_date.AddHours(this._loginUser.time_zone);
            }
            return list;
        }

        // POST api/Notices/search
        [HttpPost]
        [Route("api/Notices/searchForNoticeList")]
        public async Task<IEnumerable<Notice>> searchForNoticeList() {
            NoticeRepository.SearchParams prms = new NoticeRepository.SearchParams {
                agent_cd = this._loginUser.agent_cd,
                select_date =  DateTime.Now.Date,
                notice_list = true
            };
            var list = await db.GetListAsync(prms);
            foreach (var item in list) {
                item.notice_jpn = String.IsNullOrEmpty(item.notice_jpn) ? item.notice_jpn : item.notice_jpn.Replace("\n", "<br>");
                item.notice_eng = String.IsNullOrEmpty(item.notice_eng) ? item.notice_eng : item.notice_eng.Replace("\n", "<br>");
            }
            return list;
        }

        [HttpGet]
        [Route("api/Notices/{id?}")]
        [ResponseType(typeof(Notice))]
        public async Task<IHttpActionResult> Get(int? id = null) {
            Notice pickupPlace = null;
            try {
                if (id == null || id == 0) {
                    pickupPlace = new Notice();
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
        [Route("api/Notices/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(Notice pickupPlace) {
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
                this.Validate<Notice>(pickupPlace);
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
        [Route("api/Notices/Delete/{id}")]
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