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
    public class ItemRepository : BaseRepository<Item> {

        public class SearchParams {
            public string plan_type { get; set; }           //W=Wedding Plan, P=Photo Plan, O=Options
            public string item_type { get; set; }
            public string church_cd { get; set; }
            public string item_cd { get; set; }
            public string item_name { get; set; }

            public DateTime? wed_date { get; set; }         //discon_dateとの比較のために必要。
            public string c_num { get; set; }               //オプション検索の場合の価格取得のために必要。(c_numからwed_date, plan_kindを判断する)
            public string sub_agent_cd { get; set; }        //プラン検索の場合の価格取得のために必要。
            public bool open_to_japan_only { get; set; }
        }


        public ItemRepository() {
        }

        public ItemRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<Item, Item>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public async Task<IEnumerable<Item>> GetListAsync(string item_cd, string description) {
            SearchParams prms = new SearchParams() {
                                    item_cd = item_cd,
                                    item_name = description
                                };
            var list = await Task.Run(() => this.GetList(prms));
            return list;
        }

        public async Task<IEnumerable<Item>> GetList(SearchParams prms) {
            if (prms == null) prms = new SearchParams { item_cd = "-" };

            var agent_cd = "";

            //SubAgentCdが指定されていればその親のコードを取得。
            if (!string.IsNullOrEmpty(prms.sub_agent_cd)) {
                if (prms.sub_agent_cd != AgentParent.GetOwnSubAgentCd()) { 
                    var subAgentRepository = new SubAgentRepository(this);
                    agent_cd = await subAgentRepository.GetAgentCdAsync(prms.sub_agent_cd);
                }
            }

            if (!string.IsNullOrEmpty(agent_cd) && !prms.wed_date.HasValue) {
                prms.wed_date = RegionConfig.GetRegionToday(this.RegionCd);
            }


            var sql = new StringBuilder(
                      @"SELECT i.*
                        FROM item i ");
            var criteria = new StringBuilder();
            var prmset = new SqlParamSet();

            if (!string.IsNullOrEmpty(prms.church_cd)) {
                Context.AddSqlParam(criteria, "i.church_cd = @church_cd");
                prmset.AddChar("@church_cd", 5, prms.church_cd);
            }

            if ("W".Equals(prms.plan_type)) {
                Context.AddSqlParam(criteria, "i.item_type = 'PKG'");
            } else if ("P".Equals(prms.plan_type)) {
                Context.AddSqlParam(criteria, "i.item_type = 'PHP'");
            } else if ("O".Equals(prms.plan_type)) {
                Context.AddSqlParam(criteria, "i.item_type NOT IN ('PKG', 'PHP')");
            } else if (!string.IsNullOrEmpty(prms.plan_type)) {
                Context.AddSqlParam(criteria, "i.item_type = ''");        //plan_typeがW/P/O以外で空文字列でもない場合は０件の結果セットを返す。
            }

            if (!string.IsNullOrEmpty(prms.item_type)) {
                if (prms.item_type == "*PK") {
                    Context.AddSqlParam(criteria, "i.item_type IN ('PKG', 'PHP') ");
                } else if (prms.item_type == "*OP") {
                    Context.AddSqlParam(criteria, "i.item_type NOT IN ('PKG', 'PHP')");
                } else { 
                    Context.AddSqlParam(criteria, "i.item_type = @item_type");
                    prmset.AddChar("@item_type", 3, prms.item_type);
                }
            }

            if (!string.IsNullOrEmpty(prms.item_cd)) {
                Context.AddSqlParam(criteria, "i.item_cd LIKE @item_cd + '%'");
                prmset.AddVarChar("@item_cd", 15, prms.item_cd);        //AddCharではなくAddVarCharにしないとLIKE検索が上手く行かないので注意。
            }

            if (!string.IsNullOrEmpty(prms.item_name)) {
                Context.AddSqlParam(criteria, "i.item_name LIKE '%' + @item_name + '%' OR i.item_name_jpn LIKE '%' + @item_name + '%'");
                prmset.AddVarChar("@item_name", 200, prms.item_name);
            }

            if (prms.wed_date.HasValue) {
                Context.AddSqlParam(criteria, "i.discon_date IS NULL OR i.discon_date > @wed_date");
                prmset.AddDateTime("@wed_date", prms.wed_date.Value);

                if (!string.IsNullOrEmpty(agent_cd)) {
                    Context.AddSqlParam(criteria, "EXISTS(select * from item_price ip " + 
                                                        " where (ip.item_cd = i.item_cd ) " + 
                                                        "   and (ip.agent_cd = @agent_cd) " + 
                                                        "   and (ip.eff_from <= @wed_date) " + 
                                                        "   and (ip.eff_to >= @wed_date OR ip.eff_to is null) )");
                    prmset.AddChar("@agent_cd", 6, agent_cd);
                }
            }

            //open_to_japanで絞る
            Context.AddSqlParamIfTrue(prms.open_to_japan_only, criteria, "i.open_to_japan = 1");

            Context.MergeCriteriaToSql(sql, criteria);
            sql.Append(" ORDER BY i.item_cd ");
            
            var list = await Task.Run(() => Context.Database.SqlQuery<Item>(sql.ToString(), prmset.ToArray()).ToList());
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<ItemWithPrice>> GetListWithPrice(SearchParams prms) {
            if (prms == null) prms = new SearchParams{ item_cd = "-"};

            DateTime? wed_date = prms.wed_date;
            string agent_cd = null;
            string plan_kind = null;

            //カスタマーの挙式日とPlan Kind('W'=Wed/'P'=Photo)を取得。
            var customerRepository = new CustomerRepository(this);
            if (!string.IsNullOrEmpty(prms.c_num)) {
                wed_date = await customerRepository.GetCustomerWedDateAsync(prms.c_num);
                plan_kind = await customerRepository.GetPlanKindAsync(prms.c_num);
            } 
            
            //SubAgentCdが指定されていればその親のコードを取得。指定されていなければカスタマーのAgentCdを取得。
            if (!string.IsNullOrEmpty(prms.sub_agent_cd)) {
                var subAgentRepository = new SubAgentRepository(this);
                agent_cd = await subAgentRepository.GetAgentCdAsync(prms.sub_agent_cd);
            } else if (!string.IsNullOrEmpty(prms.c_num)) {
                agent_cd = await customerRepository.GetCustomerAgentCdAsync(prms.c_num);
            }

            var sql = @"EXEC usp_get_item_list 
                                @plan_type, 
                                @church_cd, 
                                @item_type, 
                                @item_cd, 
                                @item_name, 
                                @wed_date, 
                                @agent_cd, 
                                @plan_kind,
                                @open_to_japan_only ";
            var prmSet = new SqlParamSet();
            prmSet.AddChar("@plan_type", 1, prms.plan_type);
            prmSet.AddChar("@church_cd", 5, prms.church_cd);
            prmSet.AddChar("@item_type", 3, prms.item_type);
            prmSet.AddVarChar("@item_cd", 15, prms.item_cd);
            prmSet.AddNVarChar("@item_name", 200, prms.item_name);
            prmSet.AddDateTime("@wed_date", wed_date);
            prmSet.AddChar("@agent_cd", 6, agent_cd);
            prmSet.AddChar("@plan_kind", 1, plan_kind);
            prmSet.AddBit("@open_to_japan_only", prms.open_to_japan_only);

            var list = await Task.Run(() => Context.Database.SqlQuery<ItemWithPrice>(sql, prmSet.ToArray()).ToList());
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<Item>> GetChildren(string item_cd) {
            var sql = @"SELECT ch.*
                        FROM item p 
	                        INNER JOIN item_option op ON (p.item_cd = op.item_cd)
	                        INNER JOIN item ch ON (ch.item_cd = op.child_cd)
                        WHERE (p.item_cd = @item_cd)
                        ORDER BY ch.item_cd
                       ";
            var prms = new SqlParamSet();
            prms.AddChar("@item_cd", 15, item_cd);

            var list = await Task.Run(() => Context.Database.SqlQuery<Item>(sql, prms.ToArray()).ToList());
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public string GetItemTypeByItemCd(string item_cd) {
            string result = Context.Items
                                .Where(i => i.item_cd == item_cd)
                                .Select(i => i.item_type)
                                .SingleOrDefault();
            return result;
        }
        public async Task<string> GetItemTypeByItemCdAsync(string item_cd) {
            return await Task.Run(() => GetItemTypeByItemCd(item_cd));
        }

        public void Save(Item item, LoginUser user) {

            if (item.is_new) {
                CheckInvalidCharsForCode(item.item_cd);
            }

            using (var tr = this.Context.BeginTrans()) {
                try {
                    Item oldModel = null;
                    Item item_db = null;
                    if (item.is_new) {
                        if (this.Context.Items.Any(i => i.item_cd == item.item_cd)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", item.item_cd));
                        }
                        item_db = new Item() { item_cd = item.item_cd };
                    } else {
                        item_db = this.Find(item.item_cd);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", item.item_cd));
                        }
                        oldModel = Mapper.Map(item_db, typeof(Item), typeof(Item)) as Item;
                    }

                    Mapper.Map<Item, Item>(item, item_db);      //AutoMapperを使って全てのプロパティをコピー
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (item.is_new) {
                        Context.Set<Item>().Add(item_db);
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

        public async Task SaveAsync(Item item, LoginUser user) {
            await Task.Run(() => this.Save(item, user));
        }

        public void Delete(string item_cd, LoginUser user) {
            var item = this.Find(item_cd);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", item_cd));
            }
            Context.Set<Item>().Remove(item);
            this.SaveChanges();
        }

        public async Task DeleteAsync(string item_cd, LoginUser user) {
            await Task.Run(() => this.Delete(item_cd, user));
        }

    }
}
