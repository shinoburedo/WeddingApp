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
using System.Configuration;

namespace MauloaDemo.Repository {

    public class ItemTypeRepository : BaseRepository<ItemType> {

        public class SearchParams {
            public string plan_type { get; set; }   //W=Wedding Plan / P=Photo Plan / O=Options
            public string info_type { get; set; }   //COS/DLV/MKS/WED...など
            public bool has_items { get; set; }     //true = 関連するItemが１件以上存在するもののみに絞る。
            public bool add_blank { get; set; }     //true = 先頭に空白行を付加する。
            public bool add_pkg_opt { get; set; }   //true = 「*PK」(パッケージ), 「*OP」(オプション)の行を追加する。

            public string item_type { get; set; }
            public string description { get; set; }
        }

        public ItemTypeRepository() {
        }

        public ItemTypeRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<ItemType, ItemType>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public new IEnumerable<ItemType> GetList() {
            return this.GetList(null);
        }

        public IEnumerable<ItemType> GetList(SearchParams param) {
            if (param == null) {
                param = new SearchParams();
            }

            var query = Context.ItemTypes.AsQueryable();

            if (!string.IsNullOrEmpty(param.item_type)) {
                query = query.Where(t => t.item_type.StartsWith(param.item_type));
            }

            if ("W".Equals(param.plan_type)) {
                query = query.Where(t => t.item_type == "PKG");
            } else if ("P".Equals(param.plan_type)) {
                query = query.Where(t => t.item_type == "PHP");
            } else if ("O".Equals(param.plan_type)) {
                query = query.Where(t => t.item_type != "PKG" && t.item_type != "PHP");
            }

            if (!string.IsNullOrEmpty(param.info_type)) {
                query = query.Where(t => t.info_type == param.info_type);
            }

            if (param.has_items) {
                query = query.Where(t => Context.Items.Any(i => i.item_type == t.item_type));
            }

            if (!string.IsNullOrEmpty(param.description)) {
                query = query.Where(a => a.desc_eng.Contains(param.description)
                                      || a.desc_jpn.Contains(param.description));
            }

            query = query.OrderBy(o => o.item_type);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);

            if (param.add_pkg_opt) {
                list.Insert(0, new ItemType() { item_type = "*OP", desc_jpn = "オプション", desc_eng = "Options" });
                list.Insert(0, new ItemType() { item_type = "*PK", desc_jpn = "パッケージ", desc_eng = "Packages" });
            }

            if (param.add_blank) {
                list.Insert(0, new ItemType() { item_type = "", desc_jpn = "", desc_eng = "" });
            }

            return list;
        }

        public async Task<IEnumerable<ItemType>> GetListAsync() {
            return await this.GetListAsync(null);
        }

        public async Task<IEnumerable<ItemType>> GetListAsync(SearchParams param) {
            return await Task.Run(() => this.GetList(param));
        }

        public IEnumerable<ItemTypeC> GetListForCustomer(string language)
        {

            var sql = new StringBuilder(
                      @"SELECT t.item_type,
                        	description = CASE WHEN @language = 'J' THEN desc_jpn ELSE desc_eng END
                        FROM item_type t ");
            sql.Append(@" WHERE EXISTS(select * from item i INNER JOIN c_item c ON
                                            (i.item_cd = c.item_cd AND c.open_to_cust = 1)
                                        WHERE t.item_type = i.item_type) ");
            sql.Append(@" ORDER BY 
                          CASE item_type WHEN 'PKG' then 1
                                        WHEN 'PHP' then 2 ELSE 3 end,
                          item_type ");
            
            var prmset = new SqlParamSet();
            prmset.AddChar("@language", 1, language);

            var list = Context.Database.SqlQuery<ItemTypeC>(sql.ToString(), prmset.ToArray()).ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public void Save(ItemType itemType, LoginUser user) {

            if (itemType.is_new) {
                CheckInvalidCharsForCode(itemType.item_type);
            }

            using (var tr = this.Context.BeginTrans()) {
                try {
                    ItemType oldModel = null;
                    ItemType item_db = null;
                    if (itemType.is_new) {
                        if (this.Context.ItemTypes.Any(i => i.item_type == itemType.item_type)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", itemType.item_type));
                        }
                        item_db = new ItemType() { item_type = itemType.item_type };
                    } else {
                        item_db = this.Find(itemType.item_type);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", itemType.item_type));
                        }
                        oldModel = Mapper.Map(item_db, typeof(ItemType), typeof(ItemType)) as ItemType;
                    }

                    Mapper.Map<ItemType, ItemType>(itemType, item_db);      //AutoMapperを使って全てのプロパティをコピー
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (itemType.is_new) {
                        Context.Set<ItemType>().Add(item_db);
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

        public async Task SaveAsync(ItemType itemType, LoginUser user) {
            await Task.Run(() => this.Save(itemType, user));
        }

        public void Delete(string item_type, LoginUser user) {
            var item = this.Find(item_type);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", item_type));
            }
            Context.Set<ItemType>().Remove(item);
            this.SaveChanges();

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            log.InsertLogForDelete(user.login_id, null, item);
        }

        public async Task DeleteAsync(string item_type, LoginUser user) {
            await Task.Run(() => this.Delete(item_type, user));
        }



    }
}
