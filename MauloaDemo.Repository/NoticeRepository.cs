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

    public class NoticeRepository : BaseRepository<Notice>{

        public class SearchParams {
            public DateTime? select_date { get; set; }
            public string agent_cd { get; set; }
            public string text { get; set; }
            public bool notice_list { get; set; }
        }

        public NoticeRepository() {
        }

        public NoticeRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<Notice, Notice>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public IEnumerable<Notice> GetList(SearchParams param) {
            if (param == null) {
                param = new SearchParams();
            }

            var query = Context.Notices.AsQueryable();

            if (!string.IsNullOrEmpty(param.agent_cd)) {
                if (param.notice_list) {
                    query = query.Where(a => a.agent_cd.Equals(param.agent_cd) || String.IsNullOrEmpty(a.agent_cd));
                } else {
                    query = query.Where(a => a.agent_cd.Equals(param.agent_cd));
                }
            }

            if (!string.IsNullOrEmpty(param.text)) {
                query = query.Where(a => a.title_eng.Contains(param.text)
                                      || a.title_jpn.Contains(param.text)
                                      || a.notice_eng.Contains(param.text)
                                      || a.notice_jpn.Contains(param.text));
            }

            if (param.select_date != null) {
                query = query.Where(c => c.from_date <= param.select_date
                             && (c.to_date >= param.select_date || c.to_date == null));
            }

            query = query.OrderBy(a => a.disp_seq).ThenBy(a => a.from_date).ThenBy(a => a.to_date).ThenBy(a => a.agent_cd).ThenBy(a => a.notice_id);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<Notice>> GetListAsync(SearchParams param) {
            var list = await Task.Run(() => this.GetList(param));
            return list;
        }


        public void Save(Notice model, LoginUser user) {

            using (var tr = this.Context.BeginTrans()) {
                try {
                    Notice oldModel = null;
                    Notice item_db = null;
                    if (model.is_new) {
                        if (this.Context.Notices.Any(i => i.notice_id == model.notice_id)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", model.notice_id));
                        }
                        item_db = new Notice() { notice_id = model.notice_id };
                    } else {
                        item_db = this.Find(model.notice_id);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", model.notice_id));
                        }
                        oldModel = Mapper.Map(item_db, typeof(Notice), typeof(Notice)) as Notice;
                    }

                    Mapper.Map<Notice, Notice>(model, item_db);      //AutoMapperを使って全てのプロパティをコピー
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (model.is_new) {
                        Context.Set<Notice>().Add(item_db);
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

        public async Task SaveAsync(Notice Notice, LoginUser user) {
            await Task.Run(() => this.Save(Notice, user));
        }

        public void Delete(int notice_id, LoginUser user) {
            var item = this.Find(notice_id);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", notice_id));
            }
            Context.Set<Notice>().Remove(item);
            this.SaveChanges();

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            log.InsertLogForDelete(user.login_id, null, item);
        }

        public async Task DeleteAsync(int notice_id, LoginUser user) {
            await Task.Run(() => this.Delete(notice_id, user));
        }



    }
}