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

namespace MauloaDemo.Web.Api
{
    public class ScheduleNoteTemplatesController : BaseApiController {

        private ScheduleNoteTemplateRepository db = new ScheduleNoteTemplateRepository();

        [HttpGet]
        [Route("api/ScheduleNoteTemplates/Search")]
        [ResponseType(typeof(IEnumerable<ScheduleNoteTemplate>))]
        public async Task<IHttpActionResult> Search(string template_cd, string title) {
            var list = await db.GetListAsync(template_cd, title);
            return Ok(list);
        }

        [HttpPost]
        [Route("api/ScheduleNoteTemplates/SearchPost")]
        [ResponseType(typeof(IEnumerable<ScheduleNoteTemplate>))]
        public async Task<IHttpActionResult> SearchPost(ScheduleNoteTemplateRepository.SearchParams prms) {
            if (prms == null) {
                prms = new ScheduleNoteTemplateRepository.SearchParams();
            }
            var list = await db.GetListAsync(prms.template_cd, prms.title);
            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            foreach (var item in list) {
                item.update_date = item.update_date.AddHours(this._loginUser.time_zone);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/ScheduleNoteTemplates/{id?}")]
        [ResponseType(typeof(ScheduleNoteTemplate))]
        public async Task<IHttpActionResult> Get(string id = null) {
            ScheduleNoteTemplate model = null;
            try {
                if (string.IsNullOrEmpty(id)) {
                    model = new ScheduleNoteTemplate();
                } else {
                    model = await db.FindAsync(id);
                    if (model == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }

            return Ok(model);
        }


        [HttpPost]
        [Route("api/ScheduleNoteTemplates/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(ScheduleNoteTemplate model) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(model);
                UpperCaseHelper.Apply(model);
                LowerCaseHelper.Apply(model);
                model.last_person = this._loginUser.login_id;
                model.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<ScheduleNoteTemplate>(model);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                await db.SaveAsync(model, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }

        [HttpPost]
        [Route("api/ScheduleNoteTemplates/Delete/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete(string id) {
            var status = string.Empty;
            try {
                await db.DeleteAsync(id, this._loginUser);
                status = "ok";
            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
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
