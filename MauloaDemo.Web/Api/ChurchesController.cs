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
    public class ChurchesController : BaseApiController {

        private ChurchRepository db = new ChurchRepository();

        [HttpGet]
        [Route("api/Churches/GetList")]
        public async Task<IEnumerable<Church>> GetList() {
            return await db.GetListAsync();
        }

        [Route("api/churches/forphotoplan")]
        public async Task<IEnumerable<Church>> GetForPhotoPlan() {
            return await db.GetListAsync("P", false);
        }

        [Route("api/churches/forweddingplan")]
        public async Task<IEnumerable<Church>> GetForWeddingPlan() {
            return await db.GetListAsync("W", false);
        }

        // GET api/Churches/ForCalendar
        [Route("api/churches/forcalendar")]
        public async Task<IEnumerable<Church>> GetForCalendar() {
            return await db.GetListForCalendar();
        }

        // GET api/Churches?plan_kind=W&includeSpecial=false
        public async Task<IEnumerable<Church>> GetChurches(string plan_kind, bool includeSpecial = false) {
            return await db.GetListAsync(plan_kind, includeSpecial);
        }

        // GET api/Churches/5
        [ResponseType(typeof(Church))]
        public async Task<IHttpActionResult> GetChurch(string id) {
            Church church = await db.FindAsync(id);
            if (church == null) {
                return NotFound();
            }

            return Ok(church);
        }


        
        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [Route("api/Churches/Search")]
        [ResponseType(typeof(IEnumerable<Church>))]
        public async Task<IHttpActionResult> Search(ChurchRepository.SearchParams param) {
            var list = await db.SearchAsync(param);
            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            foreach (var item in list) {
                item.update_date = item.update_date.AddHours(this._loginUser.time_zone);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/Churches/{id?}")]
        [ResponseType(typeof(Church))]
        public async Task<IHttpActionResult> Get(string id = null) {
            Church church = null;
            try {
                if (string.IsNullOrEmpty(id)) {
                    church = new Church();
                } else {
                    church = await db.FindAsync(id);
                    if (church == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }

            return Ok(church);
        }


        [HttpPost]
        [Route("api/Churches/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(Church church) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(church);
                UpperCaseHelper.Apply(church);
                LowerCaseHelper.Apply(church);
                church.last_person = this._loginUser.login_id;
                church.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<Church>(church);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                await db.SaveAsync(church, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }


        [HttpPost]
        [Route("api/Churches/Delete/{id}")]
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
    }
}