using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Threading.Tasks;
using AutoMapper;
using CBAF;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using CBAF.Attributes;

namespace MauloaDemo.Repository {

    public class ScheduleNoteRepository : BaseRepository<ScheduleNote> {

        public ScheduleNoteRepository() {
        }

        public ScheduleNoteRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<ScheduleNote, ScheduleNote>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public IEnumerable<ScheduleNote> GetList(string c_num) {
            var query = Context.ScheduleNotes.AsQueryable();

            if (string.IsNullOrEmpty(c_num)) {
                throw new Exception("Please specify a customer number.");
            }
            query = query.Where(a => a.c_num.Equals(c_num));
            query = query.Where(a => a.deleted == false);

            query = query.OrderBy(o => o.disp_seq).ThenBy(a => a.sch_note_id);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<ScheduleNote>> GetListAsync(string c_num) {
            var list = await Task.Run(() => this.GetList(c_num));
            return list;
        }

        public IEnumerable<ScheduleNote> GetNotesFromPatternId(string c_num, int sch_pattern_id, LoginUser user) {
            var notes = new List<ScheduleNote>();

            var patternRepository = new SchedulePatternRepository(this);
            var pattern = patternRepository.Find(sch_pattern_id, user);
            if (pattern == null) return notes;     //空リストを返す。

            var customerRepository = new CustomerRepository(this);
            var customer = customerRepository.Find(c_num);
            if (customer == null || !customer.wed_date.HasValue || !customer.wed_time.HasValue) {
                return notes;     //空リストを返す。
            }

            var templateRepository = new ScheduleNoteTemplateRepository(this);

            foreach (var i in pattern.Notes) {
                var template = templateRepository.Find(i.template_cd);

                notes.Add(new ScheduleNote() { 
                    sch_note_id = 0,
                    c_num = customer.c_num,
                    template_cd = i.template_cd,
                    title_jpn = template.title_jpn,
                    title_eng = template.title_eng,
                    note_jpn = template.note_jpn,
                    note_eng = template.note_eng,
                    disp_seq = (notes.Count + 1) * 10,
                    deleted = false,
                    last_person = "",
                    update_date = DateTime.UtcNow
                });
            }
            ObjectReflectionHelper.TrimStrings(notes);
            return notes;
        }


        public void Save(ScheduleNote note, LoginUser user, IDbTransaction tr) {

            var need_commit = false;
            if (tr == null) {
                tr = Context.BeginTrans();
                need_commit = true;
            }

            try {
                ScheduleNote oldModel = null;
                ScheduleNote item_db = null;
                if (note.sch_note_id == 0) {
                    item_db = new ScheduleNote() { sch_note_id = note.sch_note_id };
                } else {
                    item_db = this.Find(note.sch_note_id);
                    if (item_db == null) {
                        throw new Exception(string.Format("The key '{0}' was not found.", note.sch_note_id));
                    }
                    oldModel = Mapper.Map<ScheduleNote>(item_db);
                }

                Mapper.Map<ScheduleNote, ScheduleNote>(note, item_db);      //AutoMapperを使って全てのプロパティをコピー
                item_db.last_person = user.login_id;
                item_db.update_date = DateTime.UtcNow;

                HankakuHelper.Apply(item_db);
                UpperCaseHelper.Apply(item_db);
                LowerCaseHelper.Apply(item_db);
                ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                if (note.sch_note_id == 0) {
                    Context.Set<ScheduleNote>().Add(item_db);
                } else {
                    Context.Entry(item_db).State = System.Data.Entity.EntityState.Modified;
                }

                this.SaveChanges();

                //変更ログを保存。
                var log = new LogChangeRepository(this);
                log.InsertLog(user.login_id, note.c_num, oldModel, item_db);

                if (need_commit) tr.Commit();

            } catch (Exception) {
                if (need_commit) tr.Rollback();
                throw;
            }
        }

        public async Task SaveAsync(ScheduleNote phrase, LoginUser user, IDbTransaction tr) {
            await Task.Run(() => this.Save(phrase, user, tr));
        }

        public void SaveList(IEnumerable<ScheduleNote> list, LoginUser user) {
            if (list == null || list.Count() == 0) return;

            using (var tr = this.Context.BeginTrans()) {
                try {
                    list.Each(phrase => this.Save(phrase, user, tr));
                    tr.Commit();

                } catch (Exception) {
                    tr.Rollback();
                    throw;
                }
            }
        }

        public async Task SaveListAsync(IEnumerable<ScheduleNote> list, LoginUser user) {
            await Task.Run(() => this.SaveList(list, user));
        }

        public void Delete(int sch_note_id, LoginUser user) {
            var item = this.Find(sch_note_id);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", sch_note_id));
            }
            Context.Set<ScheduleNote>().Remove(item);
            this.SaveChanges();
        }

        public async Task DeleteAsync(int sch_note_id, LoginUser user) {
            await Task.Run(() => this.Delete(sch_note_id, user));
        }

    }
}
