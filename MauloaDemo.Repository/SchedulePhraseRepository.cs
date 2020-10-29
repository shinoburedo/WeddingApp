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

    public class SchedulePhraseRepository : BaseRepository<SchedulePhrase> {

        public SchedulePhraseRepository() {
        }

        public SchedulePhraseRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<SchedulePhrase, SchedulePhrase>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public IEnumerable<SchedulePhrase> GetList(string c_num) {
            var query = Context.SchedulePhrases.AsQueryable();

            if (string.IsNullOrEmpty(c_num)) {
                throw new Exception("Please specify a customer number.");
            }
            query = query.Where(a => a.c_num.Equals(c_num));
            query = query.Where(a => a.deleted == false);

            query = query.OrderBy(o => o.date).ThenBy(o => o.time).ThenBy(o => o.disp_seq);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<SchedulePhrase>> GetListAsync(string c_num) {
            var list = await Task.Run(() => this.GetList(c_num));
            return list;
        }

        public IEnumerable<SchedulePhrase> GetPhrasesFromPatternId(string c_num, int sch_pattern_id, LoginUser user) {
            var phrases = new List<SchedulePhrase>();

            var patternRepository = new SchedulePatternRepository(this);
            var pattern = patternRepository.Find(sch_pattern_id, user);
            if (pattern == null) return phrases;     //空リストを返す。

            var customerRepository = new CustomerRepository(this);
            var customer = customerRepository.Find(c_num);
            if (customer == null || !customer.wed_date.HasValue || !customer.wed_time.HasValue) {
                return phrases;     //空リストを返す。
            }

            var wed_date = customer.wed_date.Value;
            var wed_time = customer.wed_time.Value;
            foreach (var line in pattern.Lines) {
                phrases.Add(new SchedulePhrase() { 
                    sch_phrase_id = 0,
                    c_num = customer.c_num,
                    sch_pattern_line_id = line.sch_pattern_line_id,
                    date = wed_date,
                    time = wed_time.AddMinutes(line.min_offset).ToString("HH:mm"),
                    disp_seq = (phrases.Count+1)*10,
                    title= line.title,
                    title_eng = line.title_eng,
                    description = line.description,
                    description_eng = line.description_eng,
                    item_type = line.item_type,
                    op_seq = 0,
                    deleted = false,
                    last_person = "",
                    update_date = DateTime.UtcNow
                });
            }
            ObjectReflectionHelper.TrimStrings(phrases);
            return phrases;
        }


        public void Save(SchedulePhrase phrase, LoginUser user, IDbTransaction tr) {

            var need_commit = false;
            if (tr == null) {
                tr = Context.BeginTrans();
                need_commit = true;
            }

            try {
                SchedulePhrase oldModel = null;
                SchedulePhrase item_db = null;
                if (phrase.sch_phrase_id == 0) {
                    item_db = new SchedulePhrase() { sch_phrase_id = phrase.sch_phrase_id };
                } else {
                    item_db = this.Find(phrase.sch_phrase_id);
                    if (item_db == null) {
                        throw new Exception(string.Format("The key '{0}' was not found.", phrase.sch_phrase_id));
                    }
                    oldModel = Mapper.Map<SchedulePhrase>(item_db);
                }

                Mapper.Map<SchedulePhrase, SchedulePhrase>(phrase, item_db);      //AutoMapperを使って全てのプロパティをコピー
                item_db.last_person = user.login_id;
                item_db.update_date = DateTime.UtcNow;

                HankakuHelper.Apply(item_db);
                UpperCaseHelper.Apply(item_db);
                LowerCaseHelper.Apply(item_db);
                ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                if (phrase.sch_phrase_id == 0) {
                    Context.Set<SchedulePhrase>().Add(item_db);
                } else {
                    Context.Entry(item_db).State = System.Data.Entity.EntityState.Modified;
                }

                this.SaveChanges();

                //変更ログを保存。
                var log = new LogChangeRepository(this);
                log.InsertLog(user.login_id, phrase.c_num, oldModel, item_db);

                if (need_commit) tr.Commit();

            } catch (Exception) {
                if (need_commit) tr.Rollback();
                throw;
            }
        }

        public async Task SaveAsync(SchedulePhrase phrase, LoginUser user, IDbTransaction tr) {
            await Task.Run(() => this.Save(phrase, user, tr));
        }

        public void SaveList(IEnumerable<SchedulePhrase> list, LoginUser user) {
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

        public async Task SaveListAsync(IEnumerable<SchedulePhrase> list, LoginUser user) {
            await Task.Run(() => this.SaveList(list, user));
        }

        public void Delete(int sch_phrase_id, LoginUser user) {
            var item = this.Find(sch_phrase_id);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", sch_phrase_id));
            }
            Context.Set<SchedulePhrase>().Remove(item);
            this.SaveChanges();
        }

        public async Task DeleteAsync(int sch_phrase_id, LoginUser user) {
            await Task.Run(() => this.Delete(sch_phrase_id, user));
        }


        public async Task<ScheduleSheetInfo> GetScheduleSheetInfo(string c_num, bool english, LoginUser user) {
            //ログインユーザーのカスタマーへの権限をチェック。
            var loginUserRepo = new LoginUserRepository(this);
            if (!loginUserRepo.CanViewCustomer(user, c_num)) {
                throw new InvalidOperationException("Not authorized.");
            }

            var customerRepository = new CustomerRepository(this);
            var info = new ScheduleSheetInfo();
            await Task.Run(() => {
                info.Customer = customerRepository.Find(c_num);
            });
            if (info.Customer == null) return null;

            if (!string.IsNullOrEmpty(info.Customer.hotel_cd)) { 
                var hotelRepository = new HotelRepository(this);
                var hotel = hotelRepository.Find(info.Customer.hotel_cd);
                if (hotel != null) { 
                    info.Customer.hotel_name = (english ? hotel.hotel_name : hotel.hotel_name_jpn);
                }
            }

            var wedInfoRepository = new WedInfoRepository(this);
            var list = await wedInfoRepository.GetList(c_num, "", user);
            info.WedInfo = list.FirstOrDefault(w => w.book_status.Equals(BookingStatus.OK));

            if (!string.IsNullOrEmpty(info.Customer.church_cd)) {
                var churchRepository = new ChurchRepository(this);
                info.Church = churchRepository.Find(info.Customer.church_cd);
            }

            info.Phrases = this.GetList(c_num).ToList();

            var scheduleNoteRepository = new ScheduleNoteRepository(this);
            info.Notes = scheduleNoteRepository.GetList(c_num).ToList();

            return info;
        }

    }
}
