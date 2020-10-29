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

    public class HolidayRepository : BaseRepository<Holiday>{

        public HolidayRepository() {
        }

        public HolidayRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<Holiday, Holiday>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        //public Holiday FindByCnum(string c_num) {
        //    Holiday holiday = null;
        //    var cust = Context.Customers.Find(c_num);
        //    if (cust != null) {
        //        holiday = Context.Holidays.Find(cust.holiday_cd);                
        //    }
        //    return holiday;
        //}

        //public async Task<Holiday> FindByCnumAsync(string c_num) {
        //    Holiday holiday = null;
        //    var cust = await Context.Customers.FindAsync(c_num);
        //    if (cust != null) {
        //        holiday = Context.Holidays.Find(cust.holiday_cd);
        //    }
        //    return holiday;
        //}

        public IEnumerable<Holiday> GetList(DateTime? holiday, string description) {
            var query = Context.Holidays.AsQueryable();

            if (holiday.HasValue) {
                query = query.Where(a => a.holiday.Equals(holiday));
            }

            if (!string.IsNullOrEmpty(description)) {
                query = query.Where(a => a.description.Contains(description)
                                      || a.description.Contains(description));
            }

            query = query.OrderBy(a =>a.holiday);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<Holiday>> GetListAsync(DateTime? holiday, string description) {
            var list = await Task.Run(() => this.GetList(holiday, description));
            return list;
        }


        public void Save(Holiday holiday, LoginUser user) {

            using (var tr = this.Context.BeginTrans()) {
                try {
                    Holiday oldModel = null;
                    Holiday item_db = null;
                    if (holiday.is_new) {
                        if (this.Context.Holidays.Any(i => i.holiday == holiday.holiday)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", holiday.holiday));
                        }
                        item_db = new Holiday() { holiday = holiday.holiday };
                    } else {
                        item_db = this.Find(holiday.holiday);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", holiday.holiday));
                        }
                        oldModel = Mapper.Map(item_db, typeof(Holiday), typeof(Holiday)) as Holiday;
                    }

                    Mapper.Map<Holiday, Holiday>(holiday, item_db);      //AutoMapperを使って全てのプロパティをコピー
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (holiday.is_new) {
                        Context.Set<Holiday>().Add(item_db);
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

        public async Task SaveAsync(Holiday holiday, LoginUser user) {
            await Task.Run(() => this.Save(holiday, user));
        }

        public void Delete(DateTime holiday, LoginUser user) {
            var item = this.Find(holiday);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", holiday));
            }
            Context.Set<Holiday>().Remove(item);
            this.SaveChanges();

            ////変更ログを保存。
            //var log = new LogChangeRepository(this);
            //log.InsertLogForDelete(user.login_id, null, item);
        }

        public async Task DeleteAsync(DateTime holiday, LoginUser user) {
            await Task.Run(() => this.Delete(holiday, user));
        }



    }
}