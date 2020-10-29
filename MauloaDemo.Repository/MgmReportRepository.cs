using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity.Validation;
using CBAF;
using MauloaDemo.Models;
using CBAF.Attributes;

namespace MauloaDemo.Repository {

    public class MgmReportDao {

        public class StoredProcParam {
            public string ParamName { get; set; }
            public string TypeName { get; set; }
            public int MaxLength { get; set; }
        }
    }

    public class MgmReportRepository : BaseRepository<MgmReport> {

        public MgmReportRepository() {
        }

        public MgmReportRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
        }


        //Findにパラメータを追加してあるので、もし間違って基底クラスのFindが呼ばれた場合は例外を発生する。
        public new MgmReport Find(params object[] keyValues) {
            throw new NotImplementedException("Use Find(string rep_cd, LoginUser user) instead.");
        }

        //Findにパラメータを追加してあるので、もし間違って基底クラスのFindAsyncが呼ばれた場合は例外を発生する。
        public new async Task<MgmReport> FindAsync(params object[] keyValues) {
            await Task.Run(() => {
                //Dummy. Nothing to do actually.
            });
            throw new NotImplementedException("Use FindAsync(string rep_cd, LoginUser user) instead.");
        }

        public MgmReport Find(string rep_cd, LoginUser user) {

            //Find時にはレポート毎のパラメータ(MgmReportParam)まで一気に読み込む。。
            var report = Context.MgmReports
                                .Include("Params")
                                .Where(m => m.rep_cd == rep_cd)
                                .SingleOrDefault();
            if (report != null) {
                ObjectReflectionHelper.TrimStrings(report);
                report.SortParams();
                //report.SetDefaultValues(user, this.RegionCd, RegionConfig.GetRegionTimeDiffFromUTC(this.RegionCd));
                report.SetDefaultValues(user, this.RegionCd, -10);      //region.configファイルが無いのでとりあえず。

                //PEND: ユーザーの権限チェック
            }
            return report;
        }

        public async Task<MgmReport> FindAsync(string rep_cd, LoginUser user) {
            return await Task.Run(() => this.Find(rep_cd, user));
        }

        public new IEnumerable<MgmReport> GetList() {
            throw new NotImplementedException("Please use another version with 'LoingUser' parameter.");
        }

        public IEnumerable<MgmReport> GetList(LoginUser user) {
            var menu_cd = string.Empty;
            var rep_cd = string.Empty;
            var rep_name = string.Empty;
            var includeHidden = false;
            return this.GetList(menu_cd, rep_cd, rep_name, includeHidden, user);
        }

        public async Task<IEnumerable<MgmReport>> GetListAsync(LoginUser user) {
            return await Task.Run(() => this.GetList(user));
        }

        public IEnumerable<MgmReport> GetList(string menu_cd, string rep_cd, string rep_name, bool includeHidden, LoginUser user) {

            //一覧取得時にはレポート毎のパラメータ(MgmReportParam)は読み込まない。
            var query = Context.MgmReports
                               .AsQueryable();

            if (!string.IsNullOrEmpty(menu_cd)) {
                query = query.Where(m => m.menu_cd == menu_cd);
            }

            if (!string.IsNullOrEmpty(rep_cd)) {
                query = query.Where(m => m.rep_cd.StartsWith(rep_cd));
            }

            if (!string.IsNullOrEmpty(rep_name)) {
                query = query.Where(m => m.rep_name.Contains(rep_name));
            }

            if (!includeHidden) {
                query = query.Where(m => m.hidden == false);
            }

            //PEND: ユーザーの権限チェック


            query = query.OrderBy(m => m.rep_seq)
                         .ThenBy(m => m.rep_cd);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);

            //一覧取得時にはレポート毎のパラメータ(MgmReportParam)は読み込まない。
            //foreach (var report in list) {
            //    report.SortParams();
            //    report.SetDefaultValues(user, this.RegionCd);
            //}

            return list;
        }
        public async Task<IEnumerable<MgmReport>> GetListAsync(string menu_cd, string rep_cd, string rep_name, bool includeHidden, LoginUser user) {
            return await Task.Run(() => this.GetList(menu_cd, rep_cd, rep_name, includeHidden, user));
        }

        public void Save(MgmReport report, LoginUser user) {

            if (report.is_new) {
                CheckInvalidCharsForCode(report.rep_cd);
            }

            using (var tr = this.Context.BeginTrans()) {
                try {
                    MgmReport report_db = null;
                    if (report.is_new) {
                        if (this.Context.MgmReports.Any(i => i.rep_cd == report.rep_cd)) {
                            throw new Exception(string.Format("Report code '{0}' already exists.", report.rep_cd));
                        }
                        report_db = new MgmReport() { rep_cd = report.rep_cd };
                    } else {
                        report_db = this.Find(report.rep_cd, user);
                        if (report_db == null) {
                            throw new Exception("Report was not found.");
                        }

                        //レポートのパラメータを一旦全て削除。
                        var prms = new SqlParamSet();
                        prms.AddVarChar("@rep_cd", 20, report.rep_cd);
                        this.Context.ExecuteSQL("DELETE FROM [mgm_report_param] WHERE (rep_cd = @rep_cd)", prms.ToArray(), tr);
                        //report.Params.ToList().ForEach(p => this.Context.MgmReportParams.Remove(p));
                    }

                    report_db.menu_cd = report.menu_cd;
                    report_db.rep_seq = report.rep_seq;
                    report_db.rep_name = report.rep_name;
                    report_db.rep_sp = report.rep_sp;
                    report_db.description = report.description;
                    report_db.output_type = report.output_type;
                    report_db.output_name = report.output_name;
                    report_db.excel_name = report.excel_name;
                    report_db.sheet_num = report.sheet_num;
                    report_db.start_pos = report.start_pos;
                    report_db.write_header = report.write_header;
                    report_db.hidden = report.hidden;
                    report_db.last_person = user.login_id;
                    report_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(report_db);
                    UpperCaseHelper.Apply(report_db);
                    LowerCaseHelper.Apply(report_db);
                    ObjectReflectionHelper.TrimStrings(report_db, true);        //空白文字をnullに変換。

                    if (report.Params == null) {
                        report.Params = new List<MgmReportParam>();
                    }
                    if (report_db.Params == null) {
                        report_db.Params = new List<MgmReportParam>();
                    }

                    //全てのパラメータを保存。
                    foreach (var p in report.Params) {
                        var p2 = new MgmReportParam();
                        p2.param_id = 0;
                        p2.rep_cd = report_db.rep_cd;
                        p2.param_seq = p.param_seq;
                        p2.param_name = p.param_name;
                        p2.param_type = p.param_type;
                        p2.caption = p.caption;
                        p2.prefix = p.prefix;
                        p2.suffix = p.suffix;
                        p2.break_after = p.break_after;
                        p2.field_length = p.field_length;
                        p2.decimal_length = p.decimal_length;
                        p2.required = p.required;
                        p2.default_value = p.default_value;
                        p2.list_cd = p.list_cd;
                        p2.last_person = user.login_id;
                        p2.update_date = report_db.update_date;

                        ObjectReflectionHelper.TrimStrings(p2, true);        //空白文字をnullに変換。
                        this.Context.MgmReportParams.Add(p2);
                    }

                    if (report.is_new) {
                        this.Add(report_db);
                    } else {
                        this.SetModified(report_db);
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

        public void Delete(string rep_cd, LoginUser user) {
            var report = this.Find(rep_cd, user);
            if (report == null) {
                throw new Exception(string.Format("Report '{0}' not found.", rep_cd));
            }
            this.Remove(report);
            this.SaveChanges();
        }

        public IEnumerable<string> GetStoreProcs(string startswith) {
            //var sql = "SELECT name FROM sysobjects WHERE [type]='P' ORDER BY name;";
            var sql = "SELECT TOP 200 name FROM sysobjects WHERE [type]='P' and name LIKE @startswith + '%' ORDER BY name;";
            var prms = new SqlParamSet();
            prms.AddNVarChar("@startswith", 20, startswith);
            var list = this.Context.Database.SqlQuery<string>(sql, prms.ToArray())
                            .ToList();
            return list;
        }

        public async Task<IEnumerable<string>> GetStoreProcsAsync(string startswith) {
            return await Task.Run(() => this.GetStoreProcs(startswith));
        }

        public IEnumerable<MgmReportDao.StoredProcParam> GetStoredProcParams(string proc_name) {
            var sql = 
                    @"SELECT 
	                    Q.name AS ParamName, 
	                    U.name AS TypeName, 
                        CASE WHEN U.name IN ('nchar', 'nvarchar') THEN Q.max_length / 2 ELSE Q.max_length END AS [MaxLength]
                      FROM sys.parameters AS Q 
	                    INNER JOIN sys.procedures AS P ON Q.object_id = P.object_id 
	                    INNER JOIN sys.types AS U ON Q.user_type_id = U.user_type_id 
                      WHERE P.name = @proc_name
                      ORDER BY Q.parameter_id";
            var prms = new SqlParamSet();
            prms.AddVarChar("@proc_name", 100, proc_name);
            var list = this.Context.Database.SqlQuery<MgmReportDao.StoredProcParam>(sql, prms.ToArray())
                                    .ToList();
            return list;
        }

        public async Task<IEnumerable<MgmReportDao.StoredProcParam>> GetStoredProcParamsAsync(string proc_name) {
            return await Task.Run(() => this.GetStoredProcParams(proc_name));
        }

        public IEnumerable<MgmReportComboList> SearchComboLists(string list_cd, string description) {
            var query = Context.MgmReportComboLists.AsQueryable();

            if (!string.IsNullOrEmpty(list_cd)) {
                query = query.Where(m => m.list_cd.StartsWith(list_cd));
            }

            if (!string.IsNullOrEmpty(description)) {
                query = query.Where(m => m.description.Contains(description));
            }

            query = query.OrderBy(m => m.list_cd);
            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<MgmReportComboList>> SearchComboListsAsync(string list_cd, string description) {
            return await Task.Run(() => this.SearchComboLists(list_cd, description));
        }


        public MgmReportComboList FindComboList(string list_cd) {
            var combo = Context.MgmReportComboLists.Find(list_cd);
            if (combo != null) {
                ObjectReflectionHelper.TrimStrings(combo);
            }
            return combo;
        }

        public async Task<MgmReportComboList> FindComboListAsync(string list_cd) {
            var combo = await Task.Run(() => this.FindComboList(list_cd));
            return combo;
        }

        public void SaveComboList(MgmReportComboList comboList, LoginUser user) {

            using (var tr = this.Context.BeginTrans()) {
                try {
                    MgmReportComboList item_db = null;
                    if (comboList.is_new) {
                        if (this.Context.MgmReportComboLists.Any(i => i.list_cd == comboList.list_cd)) {
                            throw new Exception(string.Format("List code '{0}' already exists.", comboList.list_cd));
                        }
                        item_db = new MgmReportComboList() { list_cd = comboList.list_cd };
                    } else {
                        item_db = this.FindComboList(comboList.list_cd);
                        if (item_db == null) {
                            throw new Exception("ComboList was not found.");
                        }
                    }

                    item_db.description = comboList.description;
                    item_db.query = comboList.query;
                    item_db.last_person = user.login_id;
                    item_db.update_date = DateTime.UtcNow;

                    HankakuHelper.Apply(item_db);
                    UpperCaseHelper.Apply(item_db);
                    LowerCaseHelper.Apply(item_db);
                    ObjectReflectionHelper.TrimStrings(item_db, true);        //空白文字をnullに変換。

                    if (comboList.is_new) {
                        Context.Set<MgmReportComboList>().Add(item_db);
                    } else {
                        Context.Entry(item_db).State = System.Data.Entity.EntityState.Modified;
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

        public void DeleteComboList(string list_cd, LoginUser user) {
            var item = this.FindComboList(list_cd);
            if (item == null) {
                throw new Exception(string.Format("Combo List '{0}' not found.", list_cd));
            }
            Context.Set<MgmReportComboList>().Remove(item);
            this.SaveChanges();
        }

        public async Task DeleteComboListAsync(string list_cd, LoginUser user) {
            await Task.Run(() => {
                this.DeleteComboList(list_cd, user);
            });
        }


        public List<MgmReportComboList.ValueTextPair> ExecuteComboList(string list_cd) {
            var combo = Context.MgmReportComboLists.Find(list_cd);
            if (combo != null) {
                ObjectReflectionHelper.TrimStrings(combo);
            }

            var query = combo.query;

            List<MgmReportComboList.ValueTextPair> list = null;
            if (query.StartsWith("[")) {
                list = MgmReportComboList.ParseString(query);
            } else {
                list = Context.Database.SqlQuery<MgmReportComboList.ValueTextPair>(query)
                                            .ToList();
            }
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<List<MgmReportComboList.ValueTextPair>> ExecuteComboListAsync(string list_cd) {
            return await Task.Run(() => this.ExecuteComboList(list_cd));
        }

        public DataTable ExecuteReport(MgmReport report, LoginUser user) {

            //PEND: ユーザーの権限チェック

            //rep_spがSELECTで始まる場合はそのまま実行、そうでなければストアドとして実行。
            string sql = report.rep_sp.ToLower();
            if (!sql.StartsWith("select")) {
                sql = "EXEC dbo.[" + sql + "] ";
                var first = true;
                foreach (var p in report.Params) {
                    if (!first) sql += ", ";
                    sql += " @" + p.param_name;
                    first = false;
                }
            }

            //パラメータを準備。
            var paramSet = new SqlParamSet();
            foreach (var p in report.Params) {
                switch (p.param_type) { 
                    case "number":
                        paramSet.AddDecimal(p.param_name, TypeHelper.GetDecimal(p.input_value), (byte)p.field_length, (byte)p.decimal_length);
                        break;
                    case "date":
                        paramSet.AddDateTime(p.param_name, TypeHelper.GetDateTime(p.input_value));
                        break;
                    case "check":
                        paramSet.AddBit(p.param_name, TypeHelper.GetBool(p.input_value));
                        break;
                    case "text":
                    case "combo":
                    case "radio":
                    case "hidden":
                    default:
                        paramSet.AddVarChar(p.param_name, p.field_length, p.input_value);
                        break;
                }
            }

            DataSet ds = Context.OpenDataSet(sql, paramSet.ToArray(), null, 60);
            DataTable dt = (ds != null && ds.Tables.Count > 0) ? ds.Tables[0] : null;
            return dt;
        }
    }
}
