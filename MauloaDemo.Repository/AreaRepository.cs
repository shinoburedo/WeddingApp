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

    public class AreaRepository : BaseRepository<Area>{

        public AreaRepository() {
        }

        public AreaRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<Area, Area>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public Area FindByCnum(string c_num) {
            Area area = null;
            var cust = Context.Customers.Find(c_num);
            if (cust != null) {
                area = Context.Areas.Find(cust.area_cd);                
            }
            return area;
        }

        public async Task<Area> FindByCnumAsync(string c_num) {
            Area area = null;
            var cust = await Context.Customers.FindAsync(c_num);
            if (cust != null) {
                area = Context.Areas.Find(cust.area_cd);
            }
            return area;
        }

        public IEnumerable<Area> GetList(string area_cd, string description) {
            var query = Context.Areas.AsQueryable();

            if (!string.IsNullOrEmpty(area_cd)) {
                query = query.Where(a => a.area_cd.StartsWith(area_cd));
            }

            if (!string.IsNullOrEmpty(description)) {
                query = query.Where(a => a.desc_eng.Contains(description) 
                                      || a.desc_jpn.Contains(description));
            }

            query = query.OrderBy(a =>a.area_seq).ThenBy(a => a.area_seq);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<Area>> GetListAsync(string area_cd, string description) {
            var list = await Task.Run(() => this.GetList(area_cd, description));
            return list;
        }


        public void Save(Area area, LoginUser user) {

            if (area.is_new) {
                CheckInvalidCharsForCode(area.area_cd);
            }

            using (var tr = this.Context.BeginTrans()) {
                try {
                    Area oldModel = null;
                    Area item_db = null;
                    if (area.is_new) {
                        if (this.Context.Areas.Any(i => i.area_cd == area.area_cd)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", area.area_cd));
                        }
                        item_db = new Area() { area_cd = area.area_cd };
                    } else {
                        item_db = this.Find(area.area_cd);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", area.area_cd));
                        }
                        oldModel = Mapper.Map(item_db, typeof(Area), typeof(Area)) as Area;
                    }

                    Mapper.Map<Area, Area>(area, item_db);      //AutoMapperを使って全てのプロパティをコピー
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (area.is_new) {
                        Context.Set<Area>().Add(item_db);
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

        public async Task SaveAsync(Area area, LoginUser user) {
            await Task.Run(() => this.Save(area, user));
        }

        public void Delete(string area_cd, LoginUser user) {
            var item = this.Find(area_cd);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", area_cd));
            }
            Context.Set<Area>().Remove(item);
            this.SaveChanges();

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            log.InsertLogForDelete(user.login_id, null, item);
        }

        public async Task DeleteAsync(string area_cd, LoginUser user) {
            await Task.Run(() => this.Delete(area_cd, user));
        }



    }
}