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

    public class ItemPriceRepository : BaseRepository<ItemPrice> {

        public class SearchParams {
            public string item_cd { get; set; }
            public string agent_cd { get; set; }
        }

        public ItemPriceRepository() {
        }

        public ItemPriceRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<ItemPrice, ItemPrice>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public async Task<IEnumerable<ItemPrice>> GetListAsync(SearchParams param) {
            var list = await Task.Run(() => this.GetList(param));
            return list;
        }

        public IEnumerable<ItemPrice> GetList(SearchParams param) {
            if (param == null) {
                param = new SearchParams();
            }

            var query = Context.ItemPrices.AsQueryable();

            if (!string.IsNullOrEmpty(param.item_cd)) {
                query = query.Where(a => a.item_cd.Equals(param.item_cd));
            }

            if (!string.IsNullOrEmpty(param.agent_cd)) {
                query = query.Where(a => a.agent_cd.Equals(param.agent_cd));
            }

            query = query.OrderBy(o => o.item_cd).ThenBy(o => o.agent_cd);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public void Save(ItemPrice item_price, LoginUser user) {

            using (var tr = this.Context.BeginTrans()) {
                try {
                    ItemPrice item_db = null;
                    if (item_price.is_new) {
                        if (this.Context.ItemPrices.Any(i => i.item_price_id == item_price.item_price_id)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", item_price.item_price_id));
                        }
                        item_db = new ItemPrice() { item_price_id = item_price.item_price_id };
                    } else {
                        item_db = this.Find(item_price.item_price_id);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", item_price.item_price_id));
                        }
                    }

                    Mapper.Map<ItemPrice, ItemPrice>(item_price, item_db);      //AutoMapperを使って全てのプロパティをコピー
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (item_price.is_new) {
                        Context.Set<ItemPrice>().Add(item_db);
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

        public async Task SaveAsync(ItemPrice item_price, LoginUser user) {
            await Task.Run(() => this.Save(item_price, user));
        }

        public void Delete(int item_price_id, LoginUser user) {
            var item = this.Find(item_price_id);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", item_price_id));
            }
            Context.Set<ItemPrice>().Remove(item);
            this.SaveChanges();
        }

        public async Task DeleteAsync(int item_price_id, LoginUser user) {
            await Task.Run(() => this.Delete(item_price_id, user));
        }

        public async Task DeleteByItemCdAsync(string item_cd, LoginUser user) {
            var list = Context.ItemPrices.Where(a => a.item_cd.Equals(item_cd)).ToList();
            foreach (var item in list) {
                await Task.Run(() => this.Delete(item.item_price_id, user));
            }
        }
    }
}
