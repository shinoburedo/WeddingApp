using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CBAF;
using CBAF.Attributes;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;


namespace MauloaDemo.Repository {

    public class AgentRepository : BaseRepository<Agent> {

        public class SearchParams {
            public string agent_cd { get; set; }   
            public string agent_name { get; set; }   
            public string agent_fit { get; set; }
            public string area_cd { get; set; }
        }

        public AgentRepository() {
        }

        public AgentRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<Agent, Agent>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public Agent FindByCnum(string c_num) {
            Agent ap = null;
            var cust = Context.Customers.Find(c_num);
            if (cust != null) {
                ap = Context.Agents.Find(cust.sub_agent_cd);
            }
            return ap;
        }

        public async Task<Agent> FindByCnumAsync(string c_num) {
            Agent ap = null;
            var cust = await Context.Customers.FindAsync(c_num);
            if (cust != null) {
                ap = Context.Agents.Find(cust.sub_agent_cd);
            }
            return ap;
        }

        public IEnumerable<Agent> GetList(LoginUser user) {
            var query = Context.Agents.AsQueryable();

            //エージェントユーザーの場合は自エージェントのみに絞る。
            if (user != null && user.IsAgent()) {
                var subAgentRepository = new SubAgentRepository(this);
                var agent_cd = subAgentRepository.GetAgentCd(user.sub_agent_cd);
                query = query.Where(a => a.agent_cd == agent_cd);
            }

            query = query.OrderBy(o => o.agent_cd);
            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<Agent>> GetListAsync(LoginUser user) {
            var list = await Task.Run(() => GetList(user) );
            return list;
        }


        public IEnumerable<Agent> Search(SearchParams param) {
            if (param == null) {
                param = new SearchParams();
            }

            var query = Context.Agents.AsQueryable();

            if (!string.IsNullOrEmpty(param.agent_cd)) {
                query = query.Where(t => t.agent_cd.StartsWith(param.agent_cd));
            }

            if (!string.IsNullOrEmpty(param.agent_name)) {
                query = query.Where(t => t.agent_name.Contains(param.agent_name) 
                                      || t.agent_name_jpn.Contains(param.agent_name));
            }

            if (!string.IsNullOrEmpty(param.agent_fit)) {
                query = query.Where(a => a.agent_fit.Equals(param.agent_fit));
            }

            if (!string.IsNullOrEmpty(param.area_cd)) {
                query = query.Where(a => a.area_cd.Equals(param.area_cd));
            }

            query = query.OrderBy(o => o.agent_cd);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<Agent>> SearchAsync(SearchParams param) {
            return await Task.Run(() => this.Search(param));
        }

        public void Save(Agent agent, LoginUser user) {

            using (var tr = this.Context.BeginTrans()) {
                try {
                    Agent oldModel = null;
                    Agent item_db = null;
                    if (agent.is_new) {
                        if (this.Context.Agents.Any(i => i.agent_cd == agent.agent_cd)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", agent.agent_cd));
                        }
                        item_db = new Agent() { agent_cd = agent.agent_cd };
                    } else {
                        item_db = this.Find(agent.agent_cd);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", agent.agent_cd));
                        }
                        oldModel = Mapper.Map(item_db, typeof(Agent), typeof(Agent)) as Agent;
                    }

                    Mapper.Map<Agent, Agent>(agent, item_db);      //AutoMapperを使って全てのプロパティをコピー
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (agent.is_new) {
                        Context.Set<Agent>().Add(item_db);
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

        public async Task SaveAsync(Agent agent, LoginUser user) {
            await Task.Run(() => this.Save(agent, user));
        }

        public void Delete(string agent_cd, LoginUser user) {
            var item = this.Find(agent_cd);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", agent_cd));
            }
            Context.Set<Agent>().Remove(item);
            this.SaveChanges();

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            log.InsertLogForDelete(user.login_id, null, item);
        }

        public async Task DeleteAsync(string agent_cd, LoginUser user) {
            await Task.Run(() => this.Delete(agent_cd, user));
        }

    }
}
