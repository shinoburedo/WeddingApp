using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
using System.Configuration;
using System.Threading.Tasks;
using AutoMapper;
using CBAF;
using CBAF.Attributes;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;

namespace MauloaDemo.Repository {

    public class SchedulePatternRepository : BaseRepository<SchedulePattern> {

        public SchedulePatternRepository() {
        }

        public SchedulePatternRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<SchedulePattern, SchedulePattern>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        //Findにパラメータを追加してあるので、もし間違って基底クラスのFindが呼ばれた場合は例外を発生する。
        public new SchedulePattern Find(params object[] keyValues) {
            throw new NotImplementedException("Use Find(int op_seq, LoginUser user) instead.");
        }

        //Findにパラメータを追加してあるので、もし間違って基底クラスのFindAsyncが呼ばれた場合は例外を発生する。
        public new async Task<SchedulePattern> FindAsync(params object[] keyValues) {
            await Task.Run(() => {
                //Dummy. Nothing to do actually.
            });
            throw new NotImplementedException("Use FindAsync(int op_seq, LoginUser user) instead.");
        }

        public SchedulePattern Find(int sch_pattern_id) {
            return this.Find(sch_pattern_id, null);
        }

        public async Task<SchedulePattern> FindAsync(int sch_pattern_id) {
            return await Task.Run(() => this.Find(sch_pattern_id));
        }


        public SchedulePattern Find(int sch_pattern_id, LoginUser user) {

            //Find時にはレポート毎のパラメータ(SchedulePatternLine)まで一気に読み込む。。
            var pattern = Context.SchedulePatterns
                                .Include("Lines")
                                .Include("Items")
                                .Include("Notes")
                                .Where(m => m.sch_pattern_id == sch_pattern_id)
                                .SingleOrDefault();
            if (pattern != null) {
                ObjectReflectionHelper.TrimStrings(pattern);
                pattern.SortLines();
                pattern.SortNotes();
            }
            return pattern;
        }

        public async Task<SchedulePattern> FindScheduleInfoAsync(int sch_pattern_id, LoginUser user) {
            return await Task.Run(() => this.FindScheduleInfo(sch_pattern_id, user));
        }

        public SchedulePattern FindScheduleInfo(int sch_pattern_id, LoginUser user) {

            //SchedulePatternとLines
            var pattern = Context.SchedulePatterns
                                .Include("Lines")
                                .Where(m => m.sch_pattern_id == sch_pattern_id)
                                .SingleOrDefault();

            if (pattern != null) {
                ObjectReflectionHelper.TrimStrings(pattern);
                pattern.SortLines();
            }
            return pattern;

        }

        public async Task<IEnumerable<SchedulePatternItemInfo>> FindScheduleItemInfoAsync(int sch_pattern_id, LoginUser user) {
            return await Task.Run(() => this.GetItemList(sch_pattern_id, user));
        }

        public IEnumerable<SchedulePatternItemInfo> GetItemList(int sch_pattern_id, LoginUser user) {
            //Items
            var sql = @"SELECT i.*, item.item_type, item.item_name_jpn as item_name FROM schedule_pattern_item i
                            INNER JOIN item 
                                ON item.item_cd = i.item_cd 
                            WHERE i.sch_pattern_id = @sch_pattern_id ";
            var prms = new SqlParamSet();
            prms.AddInt("@sch_pattern_id", sch_pattern_id);
            var list_item = Context.Database.SqlQuery<SchedulePatternItemInfo>(sql, prms.ToArray()).ToList();
            if (list_item != null) {
                ObjectReflectionHelper.TrimStrings(list_item);
            }
            return list_item;
        }

        public async Task<IEnumerable<SchedulePatternNoteInfo>> FindScheduleNoteInfoAsync(int sch_pattern_id, LoginUser user) {
            return await Task.Run(() => this.GetNoteList(sch_pattern_id, user));
        }

        public IEnumerable<SchedulePatternNoteInfo> GetNoteList(int sch_pattern_id, LoginUser user) {
            //Notes
            var sql = @"SELECT DISTINCT pn.*, LEFT(nt.note_jpn, 20) as note_jpn FROM schedule_pattern_note pn
                            INNER JOIN schedule_note_template nt
                                ON pn.template_cd = nt.template_cd 
                            WHERE pn.sch_pattern_id = @sch_pattern_id
                            ORDER BY pn.disp_seq ";
            var prms = new SqlParamSet();
            prms.AddInt("@sch_pattern_id", sch_pattern_id);
            var list_note = Context.Database.SqlQuery<SchedulePatternNoteInfo>(sql, prms.ToArray()).ToList();

            if (list_note != null) {
                ObjectReflectionHelper.TrimStrings(list_note);
            }
            return list_note;
        }

        public async Task<SchedulePattern> FindAsync(int sch_pattern_id, LoginUser user) {
            return await Task.Run(() => this.FindScheduleInfo(sch_pattern_id, user));
        }

        public IEnumerable<SchedulePattern> GetList(string item_cd, string description, LoginUser user) {

            var query = Context.SchedulePatterns
                               .Include("Items")
                               .AsQueryable();

            //if (!string.IsNullOrEmpty(item_cd)) {
                query = query.Where(m => m.Items.Any(i => i.item_cd == item_cd));
            //}

            if (!string.IsNullOrEmpty(description)) {
                query = query.Where(m => m.description.Contains(description));
            }

            //PEND: ユーザーの権限チェック

            query = query.OrderBy(m => m.sch_pattern_id).ThenBy(m => m.description);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);

            return list;
        }
        public async Task<IEnumerable<SchedulePattern>> GetListAsync(string item_cd, string description, LoginUser user) {
            return await Task.Run(() => this.GetList(item_cd, description, user));
        }

        public IEnumerable<SchedulePattern> GetList(string item_type, string item_cd, string description, LoginUser user) {

            var sql = @"SELECT DISTINCT p.* FROM schedule_pattern p 
                        LEFT JOIN schedule_pattern_item i 
                            ON p.sch_pattern_id = i.sch_pattern_id ";

            var exist_where = false;
            var prms = new SqlParamSet();

            if (!string.IsNullOrEmpty(item_type)) {
                sql += " WHERE ";
                sql += " EXISTS (SELECT * FROM item WHERE item_type = @item_type AND item_cd = i.item_cd) ";
                exist_where = true;
                prms.AddChar("@item_type", 3, item_type);
            }

            if (!string.IsNullOrEmpty(item_cd)) {
                if (!exist_where) {
                    sql += " WHERE ";
                    sql += " (i.item_cd = @item_cd) ";
                    exist_where = true;
                } else {
                    sql += " AND (i.item_cd = @item_cd) ";
                }
                prms.AddChar("@item_cd", 15, item_cd);
            }

            if (!string.IsNullOrEmpty(description)) {
                if (!exist_where) {
                    sql += " WHERE ";
                    sql += " (p.description LIKE '%' + @description + '%') ";
                    exist_where = true;
                } else {
                    sql += " AND (p.description LIKE '%' + @description + '%') ";
                }
                prms.AddNVarChar("@description", 100, description);
            }

            sql += " ORDER BY p.sch_pattern_id ";

            var list = Context.Database.SqlQuery<SchedulePattern>(sql, prms.ToArray()).ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<SchedulePattern>> GetListAsync(string item_type, string item_cd, string description, LoginUser user) {
            return await Task.Run(() => this.GetList(item_type, item_cd, description, user));
        }


        public void Save(SchedulePattern pattern, LoginUser user) {

            using (var tr = this.Context.BeginTrans()) {
                try {
                    SchedulePattern pattern_db = null;
                    if (pattern.sch_pattern_id == 0) {
                        pattern_db = new SchedulePattern() { sch_pattern_id = pattern.sch_pattern_id };
                    } else {
                        pattern_db = this.Find(pattern.sch_pattern_id, user);
                        if (pattern_db == null) {
                            throw new Exception("Data not found.");
                        }

                        //子レコードを一旦全て削除。
                        var prms = new SqlParamSet();
                        prms.AddInt("@sch_pattern_id", pattern.sch_pattern_id);
                        this.Context.ExecuteSQL("DELETE FROM [schedule_pattern_line] WHERE (sch_pattern_id = @sch_pattern_id)", prms.ToArray(), tr);

                        prms = new SqlParamSet();
                        prms.AddInt("@sch_pattern_id", pattern.sch_pattern_id);
                        this.Context.ExecuteSQL("DELETE FROM [schedule_pattern_item] WHERE (sch_pattern_id = @sch_pattern_id)", prms.ToArray(), tr);

                        prms = new SqlParamSet();
                        prms.AddInt("@sch_pattern_id", pattern.sch_pattern_id);
                        this.Context.ExecuteSQL("DELETE FROM [schedule_pattern_note] WHERE (sch_pattern_id = @sch_pattern_id)", prms.ToArray(), tr);
                    }

                    pattern_db.description = pattern.description;
                    pattern_db.last_person = user.login_id;
                    pattern_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(pattern_db);
                    UpperCaseHelper.Apply(pattern_db);
                    LowerCaseHelper.Apply(pattern_db);
                    ObjectReflectionHelper.TrimStrings(pattern_db, true);        //空白文字をnullに変換。

                    if (pattern.Lines == null) {
                        pattern.Lines = new List<SchedulePatternLine>();
                    }
                    if (pattern_db.Lines == null) {
                        pattern_db.Lines = new List<SchedulePatternLine>();
                    }

                    if (pattern.Notes == null) {
                        pattern.Notes = new List<SchedulePatternNote>();
                    }
                    if (pattern_db.Notes == null) {
                        pattern_db.Notes = new List<SchedulePatternNote>();
                    }

                    //全ての子レコードを保存。(Lines)
                    foreach (var p in pattern.Lines) {
                        var p2 = new SchedulePatternLine();
                        p2.sch_pattern_line_id = 0;
                        p2.sch_pattern_id = pattern_db.sch_pattern_id;
                        p2.min_offset = p.min_offset;
                        p2.title = p.title;
                        p2.title_eng = p.title_eng;
                        p2.description_eng = p.description_eng;
                        p2.item_type = p.item_type;
                        p2.last_person = user.login_id;
                        p2.update_date = pattern_db.update_date;

                        ObjectReflectionHelper.TrimStrings(p2, true);        //空白文字をnullに変換。
                        this.Context.SchedulePatternLines.Add(p2);
                    }

                    //全ての子レコードを保存。(Items)
                    foreach (var p in pattern.Items) {
                        var p2 = new SchedulePatternItem();
                        p2.row_id = 0;
                        p2.sch_pattern_id = pattern_db.sch_pattern_id;
                        p2.item_cd = p.item_cd;
                        p2.last_person = user.login_id;
                        p2.update_date = pattern_db.update_date;
                        ObjectReflectionHelper.TrimStrings(p2, true);        //空白文字をnullに変換。
                        this.Context.SchedulePatternItems.Add(p2);
                    }

                    //全ての子レコードを保存。(Notes)
                    foreach (var p in pattern.Notes) {
                        var p2 = new SchedulePatternNote();
                        p2.row_id = 0;
                        p2.sch_pattern_id = pattern_db.sch_pattern_id;
                        p2.template_cd = p.template_cd;
                        p2.disp_seq = p.disp_seq;
                        p2.last_person = user.login_id;
                        p2.update_date = pattern_db.update_date;
                        ObjectReflectionHelper.TrimStrings(p2, true);        //空白文字をnullに変換。
                        this.Context.SchedulePatternNotes.Add(p2);
                    }

                    //親レコードを保存。
                    if (pattern.sch_pattern_id == 0) {
                        this.Add(pattern_db);
                    } else {
                        this.SetModified(pattern_db);
                    }
                    this.SaveChanges();

                    tr.Commit();
                }
                catch (Exception) {
                    tr.Rollback();
                    throw;
                }
            }
        }

        public void Delete(int sch_pattern_id, LoginUser user) {
            var pattern = this.Find(sch_pattern_id, user);
            if (pattern == null) {
                throw new Exception(string.Format("SchedulePattern '{0}' not found.", sch_pattern_id));
            }
            using (var tr = this.Context.BeginTrans()) {
                try {
                    //子レコードを全て削除。
                    var prms = new SqlParamSet();
                    prms.AddInt("@sch_pattern_id", sch_pattern_id);
                    this.Context.ExecuteSQL("DELETE FROM [schedule_pattern_line] WHERE (sch_pattern_id = @sch_pattern_id)", prms.ToArray(), tr);

                    prms = new SqlParamSet();
                    prms.AddInt("@sch_pattern_id", sch_pattern_id);
                    this.Context.ExecuteSQL("DELETE FROM [schedule_pattern_item] WHERE (sch_pattern_id = @sch_pattern_id)", prms.ToArray(), tr);

                    prms = new SqlParamSet();
                    prms.AddInt("@sch_pattern_id", sch_pattern_id);
                    this.Context.ExecuteSQL("DELETE FROM [schedule_pattern_note] WHERE (sch_pattern_id = @sch_pattern_id)", prms.ToArray(), tr);

                    this.Remove(pattern);
                    this.SaveChanges();
                    tr.Commit();
                }
                catch (Exception) {
                    tr.Rollback();
                    throw;
                }
            }

        }

    }
}
