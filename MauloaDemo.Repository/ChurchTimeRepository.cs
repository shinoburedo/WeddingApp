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

    public class ChurchTimeRepository : BaseRepository<ChurchTime> {

        public class SearchParams {
            public string church_cd { get; set; }   
        }

        public class SaveParams {
            public List<ChurchTime> list { get; set; }
        }


        public ChurchTimeRepository() {
        }

        public ChurchTimeRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<ChurchTime, ChurchTime>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public IEnumerable<ChurchTime> Search(SearchParams param) {
            if (param == null) {
                param = new SearchParams();
            }

            var query = Context.ChurchTimes.AsQueryable();

            if (!string.IsNullOrEmpty(param.church_cd)) {
                query = query.Where(t => t.church_cd.StartsWith(param.church_cd));
            }

            query = query.OrderBy(o => o.church_cd).ThenBy(o => o.start_time);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<ChurchTime>> SearchAsync(SearchParams param) {
            return await Task.Run(() => this.Search(param));
        }

        public async Task SaveListAsync(List<ChurchTime> list, LoginUser user) {

            using (var tr = this.Context.BeginTrans()) {
                try {
                    foreach (var church_time in list) {
                        //ItemMasterにitem_cdが存在しなければエラー
                        if (!this.Context.Churches.Any(i => i.church_cd == church_time.church_cd)) {
                            throw new Exception(string.Format("The key value '{0}' was not found in Church Master.", church_time.church_cd));
                        }
                        church_time.start_time = new DateTime(1900, 1, 1, church_time.start_time.Hour, church_time.start_time.Minute, church_time.start_time.Second);

                        //startTimeが重複した場合は何もしない
                        if (this.Context.ChurchTimes.Any(i => i.church_cd == church_time.church_cd && i.start_time == church_time.start_time)) {
                        } else {
                            ChurchTime oldModel = null;
                            ChurchTime item_db = null;
                            var isNew = true;
                            if (this.Context.ChurchTimes.Any(i => i.church_time_id == church_time.church_time_id)) {
                                item_db = this.Find(church_time.church_time_id);
                                oldModel = Mapper.Map(item_db, typeof(ChurchTime), typeof(ChurchTime)) as ChurchTime;
                                isNew = false;
                            } else {
                                item_db = new ChurchTime();
                            }
                            Mapper.Map<ChurchTime, ChurchTime>(church_time, item_db);      //AutoMapperを使って全てのプロパティをコピー
                            HankakuHelper.Apply(item_db);
                            UpperCaseHelper.Apply(item_db);
                            LowerCaseHelper.Apply(item_db);
                            item_db.last_person = user.login_id;
                            item_db.update_date = DateTime.UtcNow;

                            if (!isNew) {
                                //更新
                                Context.Entry(item_db).State = System.Data.Entity.EntityState.Modified;
                            } else {
                                //新規登録
                                Context.Set<ChurchTime>().Add(item_db);
                            }
                            await Task.Run(() => this.SaveChanges());

                            //変更ログを保存。
                            var log = new LogChangeRepository(this);
                            log.InsertLog(user.login_id, null, oldModel, item_db);
                        }
                    }
                    tr.Commit();

                } catch (Exception) {
                    tr.Rollback();
                    throw;
                }
            }
        }

        public void Delete(int church_time_id, LoginUser user) {
            var item = this.Find(church_time_id);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", church_time_id));
            }
            Context.Set<ChurchTime>().Remove(item);
            this.SaveChanges();

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            log.InsertLogForDelete(user.login_id, null, item);
        }

        public async Task DeleteAsync(int church_time_id, LoginUser user) {
            await Task.Run(() => this.Delete(church_time_id, user));
        }

    }
}
