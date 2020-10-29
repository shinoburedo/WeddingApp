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

    public class VendorRepository : BaseRepository<Vendor> {

        public class SearchParams {
            public string vendor_cd { get; set; }
            public string vendor_name { get; set; }
            public string area_cd { get; set; }
            public string item_cd { get; set; }
        }

        public VendorRepository() {
        }

        public VendorRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<Vendor, Vendor>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public IEnumerable<Vendor> GetList(SearchParams param) {
            if (param == null) param = new SearchParams();

            var query = Context.Vendors.AsQueryable();

            if (!string.IsNullOrEmpty(param.vendor_cd)) {
                query = query.Where(a => a.vendor_cd.StartsWith(param.vendor_cd));
            }

            if (!string.IsNullOrEmpty(param.vendor_name)) {
                query = query.Where(a => a.vendor_name.Contains(param.vendor_name)
                                      || a.vendor_name_j.Contains(param.vendor_name));
            }

            if (!string.IsNullOrEmpty(param.area_cd)) {
                query = query.Where(a => a.area_cd.Equals(param.area_cd));
            }

            if (!string.IsNullOrEmpty(param.item_cd)) {
                query = query.Where(a => Context.ItemVendors.Any(iv => iv.vendor_cd == a.vendor_cd && iv.item_cd == param.item_cd));
            }

            query = query.OrderBy(o => o.vendor_cd);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<Vendor>> GetListAsync(SearchParams param) {
            var list = await Task.Run(() => this.GetList(param));
            return list;
        }

        public void Save(Vendor vendor, LoginUser user) {

            if (vendor.is_new) {
                CheckInvalidCharsForCode(vendor.vendor_cd);
            }

            using (var tr = this.Context.BeginTrans()) {
                try {
                    Vendor item_db = null;
                    if (vendor.is_new) {
                        if (this.Context.Vendors.Any(i => i.vendor_cd == vendor.vendor_cd)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", vendor.vendor_cd));
                        }
                        item_db = new Vendor() { vendor_cd = vendor.vendor_cd };
                    } else {
                        item_db = this.Find(vendor.vendor_cd);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", vendor.vendor_cd));
                        }
                    }

                    Mapper.Map<Vendor, Vendor>(vendor, item_db);      //AutoMapperを使って全てのプロパティをコピー
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (vendor.is_new) {
                        Context.Set<Vendor>().Add(item_db);
                    } else {
                        Context.Entry(item_db).State = System.Data.Entity.EntityState.Modified;
                    }

                    this.SaveChanges();
                    tr.Commit();

                } catch (Exception) {
                    tr.Rollback();
                    throw;
                }
            }
        }

        public async Task SaveAsync(Vendor vendor, LoginUser user) {
            await Task.Run(() => this.Save(vendor, user));
        }

        public void Delete(string vendor_cd, LoginUser user) {
            var item = this.Find(vendor_cd);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", vendor_cd));
            }
            Context.Set<Vendor>().Remove(item);
            this.SaveChanges();
        }

        public async Task DeleteAsync(string vendor_cd, LoginUser user) {
            await Task.Run(() => this.Delete(vendor_cd, user));
        }



    }
}
