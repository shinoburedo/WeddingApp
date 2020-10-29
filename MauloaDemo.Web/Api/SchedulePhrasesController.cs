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
    public class SchedulePhrasesController : BaseApiController {

        private SchedulePhraseRepository db = new SchedulePhraseRepository();

        [HttpGet]
        [Route("api/SchedulePhrases/Search")]
        [ResponseType(typeof(IEnumerable<SchedulePhrase>))]
        public async Task<IHttpActionResult> Search(string c_num) {
            var list = await db.GetListAsync(c_num);
            return Ok(list);
        }

        [HttpGet]
        [Route("api/SchedulePhrases/GetPhrasesFromPatternId")]
        [ResponseType(typeof(IEnumerable<SchedulePhrase>))]
        public async Task<IHttpActionResult> GetPhrasesFromPatternId(string c_num, int sch_pattern_id) {
            var list = await Task.Run(() => db.GetPhrasesFromPatternId(c_num, sch_pattern_id, _loginUser));
            return Ok(list);
        }

        [HttpGet]
        [Route("api/SchedulePhrases/{id?}")]
        [ResponseType(typeof(SchedulePhrase))]
        public async Task<IHttpActionResult> Get(int? id) {
            SchedulePhrase phrase = null;
            try {
                if (id == null || id == 0) {
                    phrase = new SchedulePhrase();
                } else {
                    phrase = await db.FindAsync(id);
                    if (phrase == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }

            return Ok(phrase);
        }


        [ApiAccessLevelFilter(3)]
        [HttpPost]
        [Route("api/SchedulePhrases/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(SchedulePhrase phrase) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(phrase);
                UpperCaseHelper.Apply(phrase);
                LowerCaseHelper.Apply(phrase);
                phrase.last_person = this._loginUser.login_id;
                phrase.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<SchedulePhrase>(phrase);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                await db.SaveAsync(phrase, this._loginUser, null);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }

        [ApiAccessLevelFilter(3)]
        [HttpPost]
        [Route("api/SchedulePhrases/SaveList")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> SaveList(IEnumerable<SchedulePhrase> list) {
            var status = string.Empty;

            //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
            ModelState.Clear();

            foreach (var model in list) {
                HankakuHelper.Apply(model);
                UpperCaseHelper.Apply(model);
                LowerCaseHelper.Apply(model);
                model.last_person = this._loginUser.login_id;
                model.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<SchedulePhrase>(model);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }
            }

            try {
                await db.SaveListAsync(list, this._loginUser);
                status = "ok";
            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }


        [ApiAccessLevelFilter(3)]
        [HttpPost]
        [Route("api/SchedulePhrases/Delete/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete(int id) {
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
