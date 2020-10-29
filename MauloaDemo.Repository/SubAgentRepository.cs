using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using CBAF;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using AutoMapper;
using CBAF.Attributes;


namespace MauloaDemo.Repository {

    public class SubAgentRepository : BaseRepository<AgentParent>{

        public class SearchParams {
            public string child_cd { get; set; }
            public string agent_cd { get; set; }
        }

        public SubAgentRepository() {
        }

        public SubAgentRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<AgentParent, AgentParent>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public AgentParent FindByCnum(string c_num) {
            AgentParent ap = null;
            var cust = Context.Customers.Find(c_num);
            if (cust != null) {
                ap = Context.AgentParents.Find(cust.sub_agent_cd);                
            }
            return ap;
        }

        public async Task<AgentParent> FindByCnumAsync(string c_num) {
            return await Task.Run(() => FindByCnum(c_num));
        }

        public IEnumerable<AgentParent> GetChildList(string sub_agent_cd) {

            var query = Context.AgentParents.AsQueryable();

            //エージェントの場合のみ制限を掛ける。(PROMの場合は全件を返す。)
            var ownSubAgentCd = AgentParent.GetOwnSubAgentCd();
            if (sub_agent_cd != ownSubAgentCd) {
                var subAgent = Context.AgentParents.Find(sub_agent_cd);
                if (subAgent != null && subAgent.indep_flg && !string.IsNullOrEmpty(subAgent.parent_cd)) {
                    //〇〇センターなどの場合は自分自身と、親が同じでかつagent_area_cdが同じ全てのエージェントを返す。
                    // 例：「HIS関東手配課」、「HIS関西手配課」など
                    query = query.Where(ap => ap.child_cd == sub_agent_cd
                                        || (ap.parent_cd == subAgent.parent_cd && ap.agent_area_cd == subAgent.agent_area_cd));
                } else {
                    //親エージェントの場合は自分自身と配下の全てのエージェント、末端のエージェントの場合は自分自身のみを返す。
                    // 例：「HISJ」、「JTBO」、「JTBW」、「HAVGIZ」
                    query = query.Where(ap => ap.child_cd == sub_agent_cd
                                        || ap.parent_cd == sub_agent_cd);
                }
            }

            //ORDER BY indep_flg DESC, inv_agent, parent_cd, child_cd
            query = query.OrderByDescending(a => a.indep_flg)
                         .ThenBy(a => a.parent_cd)
                         .ThenBy(a => a.agent_area_cd)
                         .ThenBy(a => a.inv_agent)
                         .ThenBy(a => a.child_cd);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }
        public async Task<IEnumerable<AgentParent>> GetChildListAsync(string sub_agent_cd) {
            return await Task.Run(() => this.GetChildList(sub_agent_cd));
        }

        public bool HasAccessTo(string my_sub_agent_cd, string target_sub_agent_cd) {
            if (my_sub_agent_cd == AgentParent.GetOwnSubAgentCd()) {
                return true;        //PROMユーザーは常にtrueを返す。
            }

            var query = Context.AgentParents.AsQueryable();
            var subAgent = Context.AgentParents.Find(my_sub_agent_cd);
            if (subAgent != null && subAgent.indep_flg && !string.IsNullOrEmpty(subAgent.parent_cd)) {
                //ログインが〇〇センターなどの場合は自分自身と、親が同じでかつagent_area_cdが同じ全てのエージェントを返す。
                // 例：「HIS関東手配課」、「HIS関西手配課」など
                query = query.Where(ap => ap.child_cd == my_sub_agent_cd
                                    || (ap.parent_cd == subAgent.parent_cd && ap.agent_area_cd == subAgent.agent_area_cd));
            } else {
                //ログインが親エージェントの場合は自分自身とその配下の全てのエージェントを返す。
                // 例：「HIS」、「JTBO」、「JTBW」
                query = query.Where(ap => ap.child_cd == my_sub_agent_cd
                                    || ap.parent_cd == my_sub_agent_cd);
            }
            return query.Any(a => a.child_cd == target_sub_agent_cd);
        }



        public async Task<IEnumerable<AgentParent>> GetInvAgentList(LoginUser user) {
            var list = await Task.Run(() => {
                var query = Context.AgentParents.AsQueryable();

                //query = query.Where(ap => ap.invoice_type != null);
                query = query.Where(ap => ap.parent_cd == null);

                var ownSubAgentCd = AgentParent.GetOwnSubAgentCd();
                query = query.Where(ap => ap.child_cd != ownSubAgentCd);

                //エージェントユーザーの場合は親Agentが同じものに絞る。
                if (user != null && user.IsAgent()) {
                    if (string.IsNullOrEmpty(user.agent_cd)){
                        var subAgentRepository = new SubAgentRepository(this);
                        user.agent_cd = subAgentRepository.GetAgentCd(user.sub_agent_cd);
                    }
                    query = query.Where(ap => ap.parent_cd == user.agent_cd || ap.child_cd == user.agent_cd);
                }
                query = query.OrderBy(o => o.child_cd);
                return query.ToList();
            });
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }


        public string GetAgentCd(string sub_agent_cd) {
            string agent_cd = null;

            if (!Context.AgentParents.Any(a => a.child_cd == sub_agent_cd)) {
                return null;
            }

            var parent_cd = Context.AgentParents
                                .Where(a => a.child_cd == sub_agent_cd)
                                .Select(a => a.parent_cd)
                                .SingleOrDefault();
            agent_cd = string.IsNullOrEmpty(parent_cd) ? sub_agent_cd: parent_cd;
            return TypeHelper.GetStrTrim(agent_cd);
        }
        public async Task<string> GetAgentCdAsync(string sub_agent_cd) {
            return await Task.Run(() => GetAgentCd(sub_agent_cd));
        }

        public string GetDefaultInvAgent(string sub_agent_cd, string invoice_type) {
            var def_inv_agent = Sales.INV_AGENT_CUST;
            var ownSubAgentCd = AgentParent.GetOwnSubAgentCd();
            if (sub_agent_cd == ownSubAgentCd) {
                return def_inv_agent;
            }

            var ap = this.Find(sub_agent_cd);
            if (ap == null) return def_inv_agent;

            def_inv_agent = TypeHelper.GetStrTrim(ap.inv_agent);
            if (string.IsNullOrEmpty(def_inv_agent)) {
                def_inv_agent = Sales.INV_AGENT_CUST;
            }

            return def_inv_agent;
        }
        public async Task<string> GetDefaultInvAgentAsync(string sub_agent_cd, string invoice_type) {
            return await Task.Run(() => GetDefaultInvAgent(sub_agent_cd, invoice_type));
        }

        public IEnumerable<AgentParent> Search(SearchParams param) {
            if (param == null) {
                param = new SearchParams();
            }

            var query = Context.AgentParents.AsQueryable();

            if (!string.IsNullOrEmpty(param.child_cd)) {
                query = query.Where(t => t.child_cd.StartsWith(param.child_cd));
            }

            if (!string.IsNullOrEmpty(param.agent_cd)) {
                query = query.Where(t => t.parent_cd.StartsWith(param.agent_cd));
            }

            query = query.OrderBy(o => o.child_cd);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<AgentParent>> SearchAsync(SearchParams param) {
            return await Task.Run(() => this.Search(param));
        }

        public void Save(AgentParent sub_agent, LoginUser user) {

            if (sub_agent.is_new) {
                CheckInvalidCharsForCode(sub_agent.child_cd);
            }

            using (var tr = this.Context.BeginTrans()) {
                try {
                    AgentParent oldModel = null;
                    AgentParent item_db = null;
                    if (sub_agent.is_new) {
                        if (this.Context.AgentParents.Any(i => i.child_cd == sub_agent.child_cd)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", sub_agent.child_cd));
                        }
                        item_db = new AgentParent() { child_cd = sub_agent.child_cd };
                    } else {
                        item_db = this.Find(sub_agent.child_cd);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", sub_agent.child_cd));
                        }
                        oldModel = Mapper.Map(item_db, typeof(AgentParent), typeof(AgentParent)) as AgentParent;
                    }

                    Mapper.Map<AgentParent, AgentParent>(sub_agent, item_db);
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (sub_agent.is_new) {
                        Context.Set<AgentParent>().Add(item_db);
                    } else {
                        Context.Entry(item_db).State = System.Data.Entity.EntityState.Modified;
                    }

                    this.SaveChanges();

                    //変更ログを保存。
                    var log = new LogChangeRepository(this);
                    log.InsertLog(user.login_id, null, oldModel, item_db);

                    tr.Commit();

                } catch (Exception) {
                    tr.Rollback();
                    throw;
                }
            }
        }

        public async Task SaveAsync(AgentParent agent, LoginUser user) {
            await Task.Run(() => this.Save(agent, user));
        }

        public void Delete(string child_cd, LoginUser user) {
            var item = this.Find(child_cd);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", child_cd));
            }
            Context.Set<AgentParent>().Remove(item);
            this.SaveChanges();

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            log.InsertLogForDelete(user.login_id, null, item);
        }

        public async Task DeleteAsync(string child_cd, LoginUser user) {
            await Task.Run(() => this.Delete(child_cd, user));
        }
    }
}