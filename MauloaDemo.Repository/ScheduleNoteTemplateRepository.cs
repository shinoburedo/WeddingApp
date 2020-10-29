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

    public class ScheduleNoteTemplateRepository : BaseRepository<ScheduleNoteTemplate>{

        public class SearchParams {
            public string template_cd { get; set; }
            public string title { get; set; }
        }

        public ScheduleNoteTemplateRepository() {
        }

        public ScheduleNoteTemplateRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<ScheduleNoteTemplate, ScheduleNoteTemplate>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public IEnumerable<ScheduleNoteTemplate> GetList(string template_cd, string title) {
            var query = Context.ScheduleNoteTemplates.AsQueryable();

            if (!string.IsNullOrEmpty(template_cd)) {
                query = query.Where(a => a.template_cd.StartsWith(template_cd));
            }

            if (!string.IsNullOrEmpty(title)) {
                query = query.Where(a => a.title_eng.Contains(title) 
                                      || a.title_jpn.Contains(title));
            }

            query = query.OrderBy(a =>a.template_cd);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<ScheduleNoteTemplate>> GetListAsync(string template_cd, string title) {
            var list = await Task.Run(() => this.GetList(template_cd, title));
            return list;
        }


        public void Save(ScheduleNoteTemplate model, LoginUser user) {

            if (model.is_new) {
                CheckInvalidCharsForCode(model.template_cd);
            }

            using (var tr = this.Context.BeginTrans()) {
                try {
                    ScheduleNoteTemplate oldModel = null;
                    ScheduleNoteTemplate item_db = null;
                    if (model.is_new) {
                        if (this.Context.ScheduleNoteTemplates.Any(i => i.template_cd == model.template_cd)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", model.template_cd));
                        }
                        item_db = new ScheduleNoteTemplate() { template_cd = model.template_cd };
                    } else {
                        item_db = this.Find(model.template_cd);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", model.template_cd));
                        }
                        oldModel = Mapper.Map(item_db, typeof(ScheduleNoteTemplate), typeof(ScheduleNoteTemplate)) as ScheduleNoteTemplate;
                    }

                    Mapper.Map<ScheduleNoteTemplate, ScheduleNoteTemplate>(model, item_db);      //AutoMapperを使って全てのプロパティをコピー
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (model.is_new) {
                        Context.Set<ScheduleNoteTemplate>().Add(item_db);
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

        public async Task SaveAsync(ScheduleNoteTemplate ScheduleNoteTemplate, LoginUser user) {
            await Task.Run(() => this.Save(ScheduleNoteTemplate, user));
        }

        public void Delete(string template_cd, LoginUser user) {
            var item = this.Find(template_cd);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", template_cd));
            }
            Context.Set<ScheduleNoteTemplate>().Remove(item);
            this.SaveChanges();

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            log.InsertLogForDelete(user.login_id, null, item);
        }

        public async Task DeleteAsync(string template_cd, LoginUser user) {
            await Task.Run(() => this.Delete(template_cd, user));
        }



    }
}