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

namespace MauloaDemo.Web.Api
{
    public class AgentsController : BaseApiController {

        private AgentRepository db = new AgentRepository();

        //// GET api/Agents
        //public async Task<IEnumerable<Agent>> GetAgents() {
        //    return await db.GetListAsync(_loginUser);
        //}

        //// GET api/Agents/JTB
        //[ResponseType(typeof(Agent))]
        //public async Task<IHttpActionResult> GetAgent(string id) {
        //    Agent Agent = await db.FindAsync(id);
        //    if (Agent == null) {
        //        return NotFound();
        //    }
        //    return Ok(Agent);
        //}


        [HttpPost]
        [Route("api/Agents/Search")]
        [ResponseType(typeof(IEnumerable<Agent>))]
        public async Task<IHttpActionResult> Search(AgentRepository.SearchParams param) {
            var list = await db.SearchAsync(param);
            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            foreach (var item in list) {
                item.update_date = item.update_date.AddHours(this._loginUser.time_zone);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/Agents/{id?}")]
        [ResponseType(typeof(Agent))]
        public async Task<IHttpActionResult> Get(string id = null) {
            Agent agent = null;
            try {
                if (string.IsNullOrEmpty(id)) {
                    agent = new Agent();
                } else {
                    agent = await db.FindAsync(id);
                    if (agent == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }

            return Ok(agent);
        }


        [HttpPost]
        [Route("api/Agents/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(Agent agent) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(agent);
                UpperCaseHelper.Apply(agent);
                LowerCaseHelper.Apply(agent);
                agent.last_person = this._loginUser.login_id;
                agent.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<Agent>(agent);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                await db.SaveAsync(agent, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }


        [HttpPost]
        [Route("api/Agents/Delete/{id}")]
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
