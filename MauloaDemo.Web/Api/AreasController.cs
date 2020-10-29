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

    public class AreasController : BaseApiController {

        private AreaRepository db = new AreaRepository();

        [HttpGet]
        [Route("api/Areas")]
        [ResponseType(typeof(IEnumerable<Area>))]
        public async Task<IHttpActionResult> GetList(string area_cd = null, string description = null) {
            var list = await db.GetListAsync(area_cd, description);
            return Ok(list);
        }

        [HttpPost]
        [Route("api/Areas/Search")]
        [ResponseType(typeof(IEnumerable<Area>))]
        public async Task<IHttpActionResult> Search(string area_cd = null, string description = null) {
            var list = await db.GetListAsync(area_cd, description);
            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            foreach (var item in list) {
                item.update_date = item.update_date.AddHours(this._loginUser.time_zone);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/Areas/{id?}")]
        [ResponseType(typeof(Area))]
        public async Task<IHttpActionResult> Get(string id = null){
            Area area = null;
            try {
                if (string.IsNullOrEmpty(id)) {
                    area = new Area();
                } else {
                    area = await db.FindAsync(id);
                    if (area == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }

            return Ok(area);
        }


        [HttpPost]
        [Route("api/Areas/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(Area area) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(area);
                UpperCaseHelper.Apply(area);
                LowerCaseHelper.Apply(area);
                area.last_person = this._loginUser.login_id;
                area.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<Area>(area);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                await db.SaveAsync(area, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }


        [HttpPost]
        [Route("api/Areas/Delete/{id}")]
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