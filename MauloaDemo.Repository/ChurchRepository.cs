using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CBAF;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using AutoMapper;
using CBAF.Attributes;

namespace MauloaDemo.Repository {
    public class ChurchRepository : BaseRepository<Church> {

        public class SearchParams {
            public string church_cd { get; set; }
            public string church_name { get; set; }
            public string area_cd { get; set; }
        }

        public ChurchRepository() {
        }

        public ChurchRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<Church, Church>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public Church FindByCnum(string c_num) {
            Church church = null;
            var cust = Context.Customers.Find(c_num);
            if (cust != null) {
                church = Context.Churches.Find(cust.church_cd);                
            }
            return church;
        }

        public async Task<Church> FindByCnumAsync(string c_num) {
            Church church = null;
            var cust = await Context.Customers.FindAsync(c_num);
            if (cust != null) {
                church = Context.Churches.Find(cust.church_cd);
            }
            return church;
        }

        public async Task<IEnumerable<Church>> GetListAsync() {
            return await GetListAsync(null, false);
        }

        public async Task<IEnumerable<Church>> GetListAsync(string plan_kind) {
            return await GetListAsync(plan_kind, false);
        }

        public async Task<IEnumerable<Church>> GetListForCalendar() {
            return await GetListAsync("W", true);
        }

        public async Task<IEnumerable<Church>> GetListAsync(string plan_kind, bool includeSpecial) {
            var list = await Task.Run(() => {
                var query = Context.Churches.AsQueryable();

                if (!string.IsNullOrEmpty(plan_kind)) {
                    if (includeSpecial) {
                        query = query.Where(ch => ch.plan_kind == plan_kind || ch.plan_kind == null || ch.plan_kind == "S");
                    } else { 
                        query = query.Where(ch => ch.plan_kind == plan_kind || ch.plan_kind == null);
                    }
                }

                if (!includeSpecial) { 
                    query = query.Where(ch => ch.plan_kind != "S");
                }

                query = query.OrderBy(o => o.disp_seq).ThenBy(o => o.church_cd);

                return query.ToList();
            });
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public string GetPlanKind(string church_cd) {
            var plan_kind = Context.Churches
                                .Where(c => c.church_cd == church_cd)
                                .Select(c => c.plan_kind)
                                .SingleOrDefault();
            return plan_kind;
        }

        public IEnumerable<Church> Search(SearchParams param) {
            if (param == null) {
                param = new SearchParams();
            }

            var query = Context.Churches.AsQueryable();

            if (!string.IsNullOrEmpty(param.church_cd)) {
                query = query.Where(t => t.church_cd.StartsWith(param.church_cd));
            }

            if (!string.IsNullOrEmpty(param.church_name)) {
                query = query.Where(t => t.church_name.Contains(param.church_name)
                                      || t.church_name_jpn.Contains(param.church_name));
            }

            if (!string.IsNullOrEmpty(param.area_cd)) {
                query = query.Where(a => a.area_cd.Equals(param.area_cd));
            }

            query = query.OrderBy(o => o.disp_seq).ThenBy(o => o.church_cd);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<Church>> SearchAsync(SearchParams param) {
            return await Task.Run(() => this.Search(param));
        }

        public void Save(Church church, LoginUser user) {

            if (church.is_new) {
                CheckInvalidCharsForCode(church.church_cd);
            }

            using (var tr = this.Context.BeginTrans()) {
                try {
                    Church oldModel = null;
                    Church item_db = null;
                    if (church.is_new) {
                        if (this.Context.Churches.Any(i => i.church_cd == church.church_cd)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", church.church_cd));
                        }
                        item_db = new Church() { church_cd = church.church_cd };
                    } else {
                        item_db = this.Find(church.church_cd);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", church.church_cd));
                        }
                        oldModel = Mapper.Map(item_db, typeof(Church), typeof(Church)) as Church;
                    }

                    Mapper.Map<Church, Church>(church, item_db);      //AutoMapperを使って全てのプロパティをコピー
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (church.is_new) {
                        Context.Set<Church>().Add(item_db);
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

        public async Task SaveAsync(Church church, LoginUser user) {
            await Task.Run(() => this.Save(church, user));
        }

        public void Delete(string church_cd, LoginUser user) {
            var item = this.Find(church_cd);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", church_cd));
            }
            Context.Set<Church>().Remove(item);
            this.SaveChanges();

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            log.InsertLogForDelete(user.login_id, null, item);
        }

        public async Task DeleteAsync(string church_cd, LoginUser user) {
            await Task.Run(() => this.Delete(church_cd, user));
        }

    }
}
