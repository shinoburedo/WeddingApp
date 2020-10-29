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

    public class HotelRepository : BaseRepository<Hotel> {

        public class SearchParams {
            public string hotel_cd { get; set; }
            public string hotel_name { get; set; }
            public string area_cd { get; set; }
        }

        public HotelRepository() {
        }

        public HotelRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<Hotel, Hotel>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public Hotel FindByCnum(string c_num) {
            Hotel hotel = null;
            var cust = Context.Customers.Find(c_num);
            if (cust != null) {
                hotel = Context.Hotels.Find(cust.hotel_cd);                
            }
            return hotel;
        }

        public async Task<Hotel> FindByCnumAsync(string c_num) {
            Hotel hotel = null;
            var cust = await Context.Customers.FindAsync(c_num);
            if (cust != null) {
                hotel = Context.Hotels.Find(cust.hotel_cd);
            }
            return hotel;
        }

        public IEnumerable<Hotel> GetList(SearchParams param) {
            if (param == null) {
                param = new SearchParams();
            }

            var query = Context.Hotels.AsQueryable();

            if (!string.IsNullOrEmpty(param.hotel_cd)) {
                query = query.Where(a => a.hotel_cd.StartsWith(param.hotel_cd));
            }

            if (!string.IsNullOrEmpty(param.hotel_name)) {
                query = query.Where(a => a.hotel_name.Contains(param.hotel_name)
                                      || a.hotel_name_jpn.Contains(param.hotel_name));
            }

            if (!string.IsNullOrEmpty(param.area_cd)) {
                query = query.Where(a => a.area_cd.Equals(param.area_cd));
            }

            query = query.OrderBy(o => o.sort_order).ThenBy(o => o.hotel_cd);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<Hotel>> GetListAsync(SearchParams param) {
            var list = await Task.Run(() => this.GetList(param));
            return list;
        }

        public void Save(Hotel hotel, LoginUser user) {

            if (hotel.is_new) {
                CheckInvalidCharsForCode(hotel.hotel_cd);
            }

            using (var tr = this.Context.BeginTrans()) {
                try {
                    Hotel oldModel = null;
                    Hotel item_db = null;
                    if (hotel.is_new) {
                        if (this.Context.Hotels.Any(i => i.hotel_cd == hotel.hotel_cd)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", hotel.hotel_cd));
                        }
                        item_db = new Hotel() { hotel_cd = hotel.hotel_cd };
                    } else {
                        item_db = this.Find(hotel.hotel_cd);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", hotel.hotel_cd));
                        }
                        oldModel = Mapper.Map(item_db, typeof(Hotel), typeof(Hotel)) as Hotel;
                    }

                    Mapper.Map<Hotel, Hotel>(hotel, item_db);      //AutoMapperを使って全てのプロパティをコピー
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (hotel.is_new) {
                        Context.Set<Hotel>().Add(item_db);
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

        public async Task SaveAsync(Hotel hotel, LoginUser user) {
            await Task.Run(() => this.Save(hotel, user));
        }

        public void Delete(string hotel_cd, LoginUser user) {
            var item = this.Find(hotel_cd);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", hotel_cd));
            }
            Context.Set<Hotel>().Remove(item);
            this.SaveChanges();

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            log.InsertLogForDelete(user.login_id, null, item);
        }

        public async Task DeleteAsync(string hotel_cd, LoginUser user) {
            await Task.Run(() => this.Delete(hotel_cd, user));
        }



    }
}
