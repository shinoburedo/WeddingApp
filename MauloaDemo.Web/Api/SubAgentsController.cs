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
    public class SubAgentsController : BaseApiController {

        private SubAgentRepository db = new SubAgentRepository();

        // GET api/SubAgents
        [Route("api/SubAgents/GetChildList")]
        public async Task<IEnumerable<AgentParent>> GetChildList() {
            return await db.GetChildListAsync(_loginUser.sub_agent_cd);
        }

        [Route("api/SubAgents/GetInvAgentList")]
        public async Task<IEnumerable<AgentParent>> GetInvAgentList() {
            var result = await db.GetInvAgentList(_loginUser);

            var list = result.ToList();
            list.Add(new AgentParent() {
                child_cd = Sales.INV_AGENT_CUST,
                parent_cd = null,
                company_name = "カスタマー",
                company_name_eng = "Customer"
            });

            return list;
        }


        // GET api/SubAgents/5
        [ResponseType(typeof(AgentParent))]
        public async Task<IHttpActionResult> GetSubAgent(string id) {

            //エージェントユーザーの場合はsub_agent_cdで制限を掛ける。
            if (_loginUser.IsAgent()) {
                var subAgentRepo = new SubAgentRepository(db);
                if (!subAgentRepo.HasAccessTo(_loginUser.sub_agent_cd, id)) {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
            }
            
            AgentParent ap = await db.FindAsync(id);
            if (ap == null) {
                return NotFound();
            }

            return Ok(ap);
        }

        [Route("api/SubAgents/GetDefaultInvAgent")]
        public async Task<string> GetDefaultInvAgent(string sub_agent_cd, string invoice_type) {
            return await db.GetDefaultInvAgentAsync(sub_agent_cd, invoice_type);
        }


        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [Route("api/SubAgents/Search")]
        [ResponseType(typeof(IEnumerable<AgentParent>))]
        public async Task<IHttpActionResult> Search(SubAgentRepository.SearchParams param) {
            var list = await db.SearchAsync(param);
            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            foreach (var item in list) {
                item.update_date = item.update_date.AddHours(this._loginUser.time_zone);
            }
            return Ok(list);
        }

        [HttpGet]
        [Route("api/SubAgents/{id?}")]
        [ResponseType(typeof(AgentParent))]
        public async Task<IHttpActionResult> Get(string id = null) {
            AgentParent sub_agent = null;
            try {
                if (string.IsNullOrEmpty(id)) {
                    sub_agent = new AgentParent();
                } else {
                    sub_agent = await db.FindAsync(id);
                    if (sub_agent == null) return NotFound();
                }
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }

            return Ok(sub_agent);
        }


        [HttpPost]
        [Route("api/SubAgents/Save")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Save(AgentParent sub_agent) {
            var status = string.Empty;
            try {
                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(sub_agent);
                UpperCaseHelper.Apply(sub_agent);
                LowerCaseHelper.Apply(sub_agent);
                sub_agent.last_person = this._loginUser.login_id;
                sub_agent.update_date = DateTime.UtcNow;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<AgentParent>(sub_agent);
                if (!ModelState.IsValid) {
                    return Ok(buildModelStateErrorMsg());
                }

                await db.SaveAsync(sub_agent, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }


        [HttpPost]
        [Route("api/SubAgents/Delete/{id}")]
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