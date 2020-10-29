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

    public class ItemOptionRepository : BaseRepository<ItemOption> {

        public class SearchParams {
            public string item_cd { get; set; }   
            public string item_type { get; set; }
        }

        public ItemOptionRepository() {
        }

        public ItemOptionRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<ItemOption, ItemOption>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public async Task<IEnumerable<ItemGrouping>> GetListAsync(SearchParams param) {
            return await Task.Run(() => this.Search(param));
        }

        public IEnumerable<ItemGrouping> Search(SearchParams param) {
            if (param == null) {
                param = new SearchParams();
            }

            var prms = new SqlParamSet();
            var sql = @"SELECT DISTINCT p.item_cd, p.item_name, p.item_name_jpn, p.item_type 
                        FROM item p 
                        WHERE EXISTS (SELECT * FROM item_option op WHERE op.item_cd = p.item_cd)
                       ";

            if (!string.IsNullOrEmpty(param.item_cd)) {
                sql += @" AND (p.item_cd LIKE @item_cd + '%') ";
                prms.AddVarChar("@item_cd", 15, param.item_cd);
            }

            if (!string.IsNullOrEmpty(param.item_type)) {
                sql += @" AND (p.item_type = @item_type) ";
                prms.AddChar("@item_type", 3, param.item_type);
            }

            sql += @" ORDER BY p.item_type, p.item_cd ";

            var list = Context.Database.SqlQuery<ItemGrouping>(sql, prms.ToArray()).ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<ItemGrouping>> GetOptionListAsync(SearchParams param) {
            return await Task.Run(() => this.SearchOptions(param));
        }

        public IEnumerable<ItemGrouping> SearchOptions(SearchParams param) {
            if (param == null) {
                param = new SearchParams();
            }

            var hasParam = false;
            var prms = new SqlParamSet();
            var sql = @"SELECT op.item_cd, op.child_cd, p.item_name, p.item_name_jpn, p.item_type 
                        FROM item p 
	                        INNER JOIN item_option op ON (p.item_cd = op.child_cd)
                       ";

            if (!string.IsNullOrEmpty(param.item_cd)) {
                sql += hasParam ? " AND " : " WHERE ";
                sql += @" (op.item_cd = @item_cd) ";
                prms.AddChar("@item_cd", 15, param.item_cd);
                hasParam = true;
            }

            if (!string.IsNullOrEmpty(param.item_type)) {
                sql += hasParam ? " AND " : " WHERE ";
                sql += @" (p.item_type = @item_type) ";
                prms.AddChar("@item_type", 3, param.item_type);
                hasParam = true;
            }

            sql += @" ORDER BY p.item_type, op.item_cd
                       ";
            var list = Context.Database.SqlQuery<ItemGrouping>(sql, prms.ToArray()).ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }
        //public ItemOption Find(string item_cd, string child_cd) {

        //    string result = Context.Database
        //                        .Where(i => i.item_cd == item_cd && i.child_cd == child_cd)
        //                        .SingleOrDefault();
        //    return result;
        //}

        //public async Task<IEnumerable<ItemOption>> SearchAsync(SearchParams param) {
        //    return await Task.Run(() => this.Search(param));
        //}

        public void Save(ItemOption item_option, LoginUser user) {

            if (item_option.is_new) {
                CheckInvalidCharsForCode(item_option.item_cd);
                CheckInvalidCharsForCode(item_option.child_cd);
            }

            using (var tr = this.Context.BeginTrans()) {
                try {
                    ItemOption oldModel = null;
                    ItemOption item_db = null;
                    if (item_option.is_new) {
                        if (this.Context.ItemOptions.Any(i => i.item_cd == item_option.item_cd && i.child_cd == item_option.child_cd)) {
                            throw new Exception(string.Format("The key value '{0}' & '{1}' already exists.", item_option.item_cd, item_option.child_cd));
                        }
                        item_db = new ItemOption() { item_cd = item_option.item_cd,
                                                    child_cd = item_option.child_cd};
                    } else {
                        item_db = this.Find(item_option.item_cd, item_option.child_cd);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' & '{1}' was not found.", item_option.item_cd, item_option.child_cd));
                        }
                        oldModel = Mapper.Map(item_db, typeof(ItemOption), typeof(ItemOption)) as ItemOption;
                    }

                    Mapper.Map<ItemOption, ItemOption>(item_option, item_db);      //AutoMapperを使って全てのプロパティをコピー
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (item_option.is_new) {
                        Context.Set<ItemOption>().Add(item_db);
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

        public async Task SaveAsync(ItemOption agent, LoginUser user) {
            await Task.Run(() => this.Save(agent, user));
        }

        public async Task SaveListAsync(string item_cd, List<ItemOption> list, bool is_new, LoginUser user) {

            using (var tr = this.Context.BeginTrans()) {
                try {
                    if (is_new) {
                        if (this.Context.ItemOptions.Any(i => i.item_cd == item_cd)) {
                            throw new Exception(string.Format("The key value '{0}' already exists.", item_cd));
                        }
                    } else {
                        SearchParams param = new SearchParams() { item_cd = item_cd };
                        var item_db = this.Search(param);
                        if (item_db == null) {
                            throw new Exception(string.Format("The key '{0}' was not found.", item_cd));
                        }
                    }
                    //ItemMasterにitem_cdが存在しなければエラー
                    if (!this.Context.Items.Any(i => i.item_cd == item_cd)) {
                        throw new Exception(string.Format("The key value '{0}' was not found in Item Master.", item_cd));
                    }

                    //item_cdをキーにitem_optionを削除
                    var prms = new SqlParamSet();
                    prms.AddChar("@item_cd", 15, item_cd);
                    await Task.Run(() =>　this.Context.ExecuteSQL("DELETE FROM [item_option] WHERE (item_cd = @item_cd)", prms.ToArray(), tr));

                    //Gridのレコードをテーブルに保存
                    foreach (var option in list) {
                        //ItemMasterにitem_cdが存在しなければエラー
                        if (!this.Context.Items.Any(i => i.item_cd == option.child_cd)) {
                            throw new Exception(string.Format("The key value '{0}' was not found in Item Master.", option.child_cd));
                        }
                        //item_cdとchild_cdが同じ場合エラー
                        if (option.child_cd == item_cd) {
                            throw new Exception(string.Format("Item Code and Child Code can't be same."));
                        }
                        option.item_cd = item_cd;
                        HankakuHelper.Apply(option);
                        UpperCaseHelper.Apply(option);
                        LowerCaseHelper.Apply(option);
                        option.last_person = user.login_id;
                        option.update_date = DateTime.UtcNow;
                        ObjectReflectionHelper.TrimStrings(option, true);        //空白文字をnullに変換。
                        Context.Set<ItemOption>().Add(option);
                        await Task.Run(() =>　this.SaveChanges());
                    }

                    tr.Commit();

                } catch (Exception) {
                    tr.Rollback();
                    throw;
                }
            }
        }

        public void DeleteOption(string item_cd, string child_cd, LoginUser user) {
            var item = this.Find(item_cd, child_cd);
            if (item == null) {
                return;
            }
            Context.Set<ItemOption>().Remove(item);
            this.SaveChanges();

        }

        public async Task DeleteOptionAsync(string item_cd, string child_cd, LoginUser user) {
            await Task.Run(() => this.DeleteOption(item_cd, child_cd, user));
        }

        public void Delete(string item_cd, LoginUser user) {
            var prms = new SqlParamSet();
            prms.AddChar("@item_cd", 15, item_cd);
            this.Context.ExecuteSQL("DELETE FROM [item_option] WHERE (item_cd = @item_cd)", prms.ToArray());

        }

        public async Task DeleteAsync(string item_cd, LoginUser user) {
            await Task.Run(() => this.Delete(item_cd, user));
        }

    }
}
