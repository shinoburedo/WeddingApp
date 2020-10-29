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

    public class PickupPlaceRepository : BaseRepository<PickupPlace> {

        public class SearchParams {
            public string place_name { get; set; } 
            public string hotel_cd { get; set; }
            public bool getAllList { get; set; }
        }

        public PickupPlaceRepository() {
        }

        public PickupPlaceRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<PickupPlace, PickupPlace>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public new async Task<IEnumerable<PickupPlace>> GetList() {
            return await GetList(null);
        }

        public async Task<IEnumerable<PickupPlace>> GetList(SearchParams param) {
            if (param == null) {
                param = new SearchParams();
            }

            var list = await Task.Run(() => {
                    var query = Context.PickupPlaces.AsQueryable();

                    if (!string.IsNullOrEmpty(param.place_name)) {
                        query = query.Where(t =>  t.place_name.Contains(param.place_name));
                    }

                    if (!string.IsNullOrEmpty(param.hotel_cd) && !param.getAllList) {
                        query = query.Where(t => t.hotel_cd == param.hotel_cd || t.hotel_cd == null);
                    } else if (!string.IsNullOrEmpty(param.hotel_cd) && param.getAllList) {
                        query = query.Where(t => t.hotel_cd == param.hotel_cd);
                    } else if (param.getAllList) {
                    } else {
                        query = query.Where(t => t.hotel_cd == null);
                    }

                    query = query.OrderBy(t => t.place_order)
                                 .ThenBy(t => t.place_name);
                    return query.ToList();
                });
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public void Save(PickupPlace pickupPlace, LoginUser user) {

            using (var tr = this.Context.BeginTrans()) {
                try {
                    PickupPlace oldModel = null;
                    PickupPlace item_db = null;
                    if (pickupPlace.is_new) {
                        if (this.Context.PickupPlaces.Any(i => i.place_id == pickupPlace.place_id)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", pickupPlace.place_id));
                        }
                        item_db = new PickupPlace() { place_id = pickupPlace.place_id };
                    } else {
                        item_db = this.Find(pickupPlace.place_id);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", pickupPlace.place_id));
                        }
                        oldModel = Mapper.Map(item_db, typeof(PickupPlace), typeof(PickupPlace)) as PickupPlace;
                    }

                    Mapper.Map<PickupPlace, PickupPlace>(pickupPlace, item_db);      //AutoMapperを使って全てのプロパティをコピー
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (pickupPlace.is_new) {
                        item_db.create_by = user.login_id;
                        item_db.create_date = DateTime.UtcNow;
                        Context.Set<PickupPlace>().Add(item_db);
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

        public async Task SaveAsync(PickupPlace pickupPlace, LoginUser user) {
            await Task.Run(() => this.Save(pickupPlace, user));
        }

        public void Delete(int place_id, LoginUser user) {
            var item = this.Find(place_id);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", place_id));
            }
            Context.Set<PickupPlace>().Remove(item);
            this.SaveChanges();

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            log.InsertLogForDelete(user.login_id, null, item);
        }

        public async Task DeleteAsync(int place_id, LoginUser user) {
            await Task.Run(() => this.Delete(place_id, user));
        }
    }
}
