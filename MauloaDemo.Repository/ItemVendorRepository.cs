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

    public class ItemVendorRepository : BaseRepository<ItemVendor> {

        public class SearchParams {
            public string item_cd { get; set; }
            public string vendor_cd { get; set; }
        }

        public ItemVendorRepository() {
        }

        public ItemVendorRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<ItemVendor, ItemVendor>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public async Task<IEnumerable<ItemVendor>> GetListAsync(SearchParams param) {
            var list = await Task.Run(() => this.GetList(param));
            return list;
        }

        public IEnumerable<ItemVendor> GetList(SearchParams param) {
            if (param == null) {
                param = new SearchParams();
            }

            var query = Context.ItemVendors.AsQueryable();

            if (!string.IsNullOrEmpty(param.item_cd)) {
                query = query.Where(a => a.item_cd.Equals(param.item_cd));
            }

            if (!string.IsNullOrEmpty(param.vendor_cd)) {
                query = query.Where(a => a.vendor_cd.Equals(param.vendor_cd));
            }

            query = query.OrderBy(o => o.item_cd).ThenBy(o => o.vendor_cd);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public void Save(ItemVendor item_vendor, LoginUser user) {

            using (var tr = this.Context.BeginTrans()) {
                try {
                    ItemVendor item_db = null;
                    if (item_vendor.is_new) {
                        if (this.Context.ItemVendors.Any(i => i.item_vendor_id == item_vendor.item_vendor_id)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", item_vendor.item_vendor_id));
                        }
                        item_db = new ItemVendor() { item_vendor_id = item_vendor.item_vendor_id };
                    } else {
                        item_db = this.Find(item_vendor.item_vendor_id);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", item_vendor.item_vendor_id));
                        }
                    }

                    Mapper.Map<ItemVendor, ItemVendor>(item_vendor, item_db);      //AutoMapperを使って全てのプロパティをコピー
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (item_vendor.is_new) {
                        Context.Set<ItemVendor>().Add(item_db);
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

        public async Task SaveAsync(ItemVendor item_vendor, LoginUser user) {
            await Task.Run(() => this.Save(item_vendor, user));
        }

        public void Delete(int item_vendor_id, LoginUser user) {
            var item = this.Find(item_vendor_id);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", item_vendor_id));
            }
            Context.Set<ItemVendor>().Remove(item);
            this.SaveChanges();
        }

        public async Task DeleteAsync(int item_vendor_id, LoginUser user) {
            await Task.Run(() => this.Delete(item_vendor_id, user));
        }

        public async Task DeleteByItemCdAsync(string item_cd, LoginUser user) {
            var list = Context.ItemVendors.Where(a => a.item_cd.Equals(item_cd)).ToList();
            foreach (var item in list) {
                await Task.Run(() => this.Delete(item.item_vendor_id, user));
            }
        }
    }
}
