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
using MauloaDemo.Models.Combined;

namespace MauloaDemo.Web.Api
{
    public class SchedulePatternsController : BaseApiController {

        private SchedulePatternRepository db = new SchedulePatternRepository();

        [HttpGet]
        [Route("api/SchedulePatterns/Search")]
        [ResponseType(typeof(IEnumerable<SchedulePattern>))]
        public async Task<IHttpActionResult> Search(string item_type = null, string item_cd = null, string description = null) {
            var list = await db.GetListAsync(item_type, item_cd, description, this._loginUser);
            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            foreach (var item in list) {
                item.update_date = item.update_date.AddHours(this._loginUser.time_zone);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/SchedulePatterns/SearchItem")]
        [ResponseType(typeof(IEnumerable<SchedulePatternItemInfo>))]
        public async Task<IHttpActionResult> SearchItem(int id) {
            var list = await db.FindScheduleItemInfoAsync(id, this._loginUser);
            return Ok(list);
        }

        [HttpGet]
        [Route("api/SchedulePatterns/SearchNote")]
        [ResponseType(typeof(IEnumerable<SchedulePatternNoteInfo>))]
        public async Task<IHttpActionResult> SearchNote(int id) {
            var list = await db.FindScheduleNoteInfoAsync(id, this._loginUser);
            return Ok(list);
        }

        [HttpGet]
        [Route("api/SchedulePatterns/{id?}")]
        [ResponseType(typeof(SchedulePattern))]
        public async Task<IHttpActionResult> Get(int? id = null) {
            SchedulePattern pattern = null;
            try {

                if (id == 0 || id == null) {
                    pattern = new SchedulePattern();
                } else {
                    pattern = await db.FindAsync(id, this._loginUser);
                    if (pattern == null) return NotFound();
                }

            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }
            return Ok(pattern);
        }

        [HttpGet]
        [Route("api/SchedulePatterns/Find/{id}")]
        [ResponseType(typeof(SchedulePattern))]
        public async Task<IHttpActionResult> Find(int id) {
            SchedulePattern pattern = null;
            try {

                if (id == 0) {
                    pattern = new SchedulePattern();
                } else {
                    pattern = await db.FindScheduleInfoAsync(id, this._loginUser);
                    if (pattern == null) return NotFound();
                }

            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }
            return Ok(pattern);
        }


        [HttpPost]
        [Route("api/SchedulePatterns/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(SchedulePatternInfo param) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();

                SchedulePattern pattern = new SchedulePattern();
                pattern.description = param.description;
                pattern.Items = param.Items;
                pattern.last_person = param.last_person;
                pattern.Lines = param.Lines;
                pattern.Notes = param.Notes;
                pattern.sch_pattern_id = param.sch_pattern_id;
                pattern.update_date = param.update_date;

                HankakuHelper.Apply(pattern);
                UpperCaseHelper.Apply(pattern);
                LowerCaseHelper.Apply(pattern);
                pattern.last_person = this._loginUser.login_id;
                pattern.update_date = DateTime.UtcNow;

                if (pattern.Lines != null) {
                    foreach (var p in pattern.Lines) {
                        p.sch_pattern_line_id = 0;
                        p.sch_pattern_id = pattern.sch_pattern_id;
                        p.last_person = pattern.last_person;
                    }
                }

                if (pattern.Notes != null) {
                    foreach (var p in pattern.Notes) {
                        p.row_id = 0;
                        p.sch_pattern_id = pattern.sch_pattern_id;
                        p.last_person = pattern.last_person;
                    }
                }

                if (pattern.Items != null) {
                    foreach (var p in pattern.Items) {
                        p.row_id = 0;
                        p.sch_pattern_id = pattern.sch_pattern_id;
                        p.last_person = pattern.last_person;
                    }
                }

                //ここで再度検証を実行。(エラーがあれば例外を発生する。)
                //pattern.Validate();
                if (!ModelState.IsValid) {
                    var errorMsg = "Validation error: \n";
                    foreach (var value in ModelState.Values) {
                        foreach (var err in value.Errors) {
                            errorMsg += string.Format("\n{0}", err.ErrorMessage);
                        }
                    }
                    throw new Exception(errorMsg);
                }

                await Task.Run(() => {
                    db.Save(pattern, this._loginUser);
                });

                status = "ok";
            } catch (DbEntityValidationException ex) {
                var errorMsg = "Validation error: \n";
                foreach (var err in ex.EntityValidationErrors) {
                    foreach (var error in err.ValidationErrors) {
                        errorMsg += string.Format("\n{0}", error.ErrorMessage);
                    }
                }
                status = errorMsg;
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                status = ex.Message;
            }

            return Ok(status);
        }

        [HttpPost]
        [Route("api/SchedulePatterns/Delete/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete(int id) {
            var status = string.Empty;
            try {
                await Task.Run(() => db.Delete(id, this._loginUser));
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
