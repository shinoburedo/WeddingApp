using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using CBAF;
using CBAF.Attributes;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using System.Data;

namespace MauloaDemo.Repository {

    /// <summary>
    /// NotMapped属性の付いたプロパティをJsonシリアライズ対象から除外する。
    /// 
    /// 参考URL：　http://james.newtonking.com/json/help/index.html?topic=html/ConditionalProperties.htm
    /// </summary>
    public class ShouldSerializeContractResolver : DefaultContractResolver {
        public static readonly ShouldSerializeContractResolver Instance = new ShouldSerializeContractResolver();

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            if (member.GetCustomAttribute<NotMappedAttribute>() != null) {
                property.ShouldSerialize = instance => false;
            }
            return property;
        }
    }

    public class LogChangeRepository : BaseRepository<LogChange> {

        protected JsonSerializerSettings serializeSettings;

        public LogChangeRepository() {
            //NotMapped属性が付いているプロパティをJSONシリアライズ対象から除外。
            serializeSettings = new JsonSerializerSettings() {
                ContractResolver = ShouldSerializeContractResolver.Instance
            };
        }

        public LogChangeRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
        }

        public bool InsertLog(string login_id, string c_num, string table_name, int? key_id, string key_cd, string action, string changes) {

            //差分が何も無い場合は保存しない。
            if (action == "U" && string.IsNullOrWhiteSpace(changes)) {
                return false;
            }

            var logChange= new LogChange() {
                login_id = login_id,
                c_num = c_num,
                table_name = table_name,
                key_id = key_id,
                key_cd = key_cd, 
                action = action,
                changes = changes
            };
            Context.LogChanges.Add(logChange);
            this.SaveChanges();

            return true;
        }

        public bool InsertLog(string login_id, string c_num, object oldModel, object newModel) {
            string table_name = "";
            int? key_id = null;
            string key_cd = null;
            string changes = null;
            string action;

            if (oldModel != null && newModel != null) {
                action = "U";
                table_name = ObjectReflectionHelper.GetTableName(newModel);
                ObjectReflectionHelper.GetKeyValue(newModel, ref key_id, ref key_cd);

                //変更の場合は差分だけをJSON文字列として取得。
                var changeDiffs = ObjectReflectionHelper.GetChanges(oldModel, newModel);
                if (changeDiffs != null && changeDiffs.Count() > 0) {

                    //Booking Statusが変わった場合はステータスの値を分り易い文字列に変換する。(例：　K => 「OK」, X => 「CXLRQ」など)
                    foreach (var item in changeDiffs) {
                        if (item.key == "book_status") {
                            item.oldValue = BookingStatus.GetTextForValue(TypeHelper.GetStr(item.oldValue));
                            item.newValue = BookingStatus.GetTextForValue(TypeHelper.GetStr(item.newValue));
                        }
                    }

                    changes = Newtonsoft.Json.JsonConvert.SerializeObject(changeDiffs, serializeSettings);
                }

            } else if (oldModel != null) {
                action = "D";
                table_name = ObjectReflectionHelper.GetTableName(oldModel);
                ObjectReflectionHelper.GetKeyValue(oldModel, ref key_id, ref key_cd);
                //削除の場合はオブジェクト全体をJSON化。
                changes = Newtonsoft.Json.JsonConvert.SerializeObject(oldModel, serializeSettings);
            } else {
                action = "I";
                table_name = ObjectReflectionHelper.GetTableName(newModel);
                ObjectReflectionHelper.GetKeyValue(newModel, ref key_id, ref key_cd);
                //新規の場合はオブジェクト全体をJSON化。
                changes = Newtonsoft.Json.JsonConvert.SerializeObject(newModel, serializeSettings);
            }

            if (!string.IsNullOrEmpty(key_cd) && key_cd.Length > 20) {
                try {
                    DateTime dt = DateTime.Parse(key_cd);
                    key_cd = dt.ToString("yyyy-MM-dd");
                } catch (FormatException) {
                    key_cd = key_cd.Substring(0, 20);
                }
            }

            return this.InsertLog(login_id, c_num, table_name, key_id, key_cd, action, changes);
        }

        public void InsertLogForDelete(string login_id, string c_num, object oldModel) {
            string table_name = "";
            int? key_id = null;
            string key_cd = null;
            string changes = null;
            string action = "D";
            table_name = ObjectReflectionHelper.GetTableName(oldModel);

            ObjectReflectionHelper.GetKeyValue(oldModel, ref key_id, ref key_cd);
            //削除の場合はオブジェクト全体をJSON化。
            changes = Newtonsoft.Json.JsonConvert.SerializeObject(oldModel, serializeSettings);

            this.InsertLog(login_id, c_num, table_name, key_id, key_cd, action, changes);
        }


        public void Archive(int log_id, LoginUser loginUser, IDbTransaction tr) {
            //var exists = Context.LogChangeArchives
            //                        .Where(a => a.log_id == log_id && a.sub_agent_cd == loginUser.sub_agent_cd)
            //                        .Any();
            //if (exists) return;  //既に同じsub_agent_cdでアーカイブされている場合は何もしない。

            //var logChangeArchive = new LogChangeArchive() {
            //    log_id = log_id,
            //    archive_by = loginUser.login_id,
            //    sub_agent_cd = loginUser.sub_agent_cd
            //};
            //Context.LogChangeArchives.Add(logChangeArchive);

            var procName = "usp_log_archive_on";
            var prms = new SqlParamSet();
            prms.AddInt("@log_id", log_id);
            prms.AddChar("@sub_agent_cd", 6, loginUser.sub_agent_cd);
            prms.AddVarChar("@archive_by", 15, loginUser.login_id);
            Context.ExecuteStoredProcedure(procName, prms.ToArray(), tr);
        }

        public void UnArchive(int log_id, LoginUser loginUser, IDbTransaction tr) {
            //var existing = Context.LogChangeArchives
            //                        .Where(a => a.log_id == log_id 
            //                                && a.sub_agent_cd == loginUser.sub_agent_cd
            //                                && a.archive_by == loginUser.login_id)
            //                        .ToList();
            //existing.ForEach(i => Context.LogChangeArchives.Remove(i));

            var procName = "usp_log_archive_off";
            var prms = new SqlParamSet();
            prms.AddInt("@log_id", log_id);
            prms.AddChar("@sub_agent_cd", 6, loginUser.sub_agent_cd);
            prms.AddVarChar("@archive_by", 15, loginUser.login_id);
            Context.ExecuteStoredProcedure(procName, prms.ToArray(), tr);
        }

        public void ArchiveOrUnArchive(LogChangeArchive.ChangedRows rows, LoginUser loginUser) {

            using (var tr = this.Context.BeginTrans()) {
                try {
                    //スケジュールの新規作成時のログをアーカイブする場合は同じカスタマーで他のスケジュール新規作成ログもアーカイブする。
                    // グリッド上はlog_idが最小の1行だけを表示しているため。
                    List<int> added_ids = new List<int>();
                    rows.ids_on.ToList().ForEach(log_id => {
                        var log = this.Find(log_id);
                        if (log != null && log.action == "I" && log.table_name == "schedule_phrase") {
                            var list = Context.LogChanges.Where(g => g.log_id < log.log_id 
                                                    && g.c_num == log.c_num 
                                                    && g.action == log.action 
                                                    && g.table_name == log.table_name).ToList();
                            list.ForEach(a => added_ids.Add(a.log_id));
                        }
                    });
                    rows.ids_on = rows.ids_on.Concat(added_ids).ToArray();

                    rows.ids_on.ToList().ForEach(a => this.Archive(a, loginUser, tr));
                    rows.ids_off.ToList().ForEach(a => this.UnArchive(a, loginUser, tr));

                    tr.Commit();

                } catch (Exception) {
                    tr.Rollback();
                    throw;
                }
            }
        }



    }
}
