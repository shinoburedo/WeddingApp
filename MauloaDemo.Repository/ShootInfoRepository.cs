using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CBAF;
using ProjectM.Models;
using ProjectM.Models.Combined;

namespace ProjectM.Repository {

    public class ShootInfoRepository : BaseRepository<ShootInfo> {

        public class SearchParams {
            public string c_num { get; set; }
            public DateTime? date_from { get; set; }
            public DateTime? date_to { get; set; }

            public SearchParams() {
                date_from = null;
                date_to = null;
            }
        }

        public class ShootInfoResult {
            public int op_seq { get; set; }
            public string message { get; set; }
        }

        public ShootInfoRepository() {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
        }

        public async Task<IEnumerable<ShootInfo>> GetList(string c_num) {
            var list = await Task.Run(() => 
                            Context.ShootInfos
                                .Where(s => s.c_num == c_num)
                                .OrderByDescending(c => c.info_id)
                                .ToList()
                       );
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }


        public async Task<IEnumerable<ShootInfo>> Search(SearchParams prms) {
            if (prms == null) {
                prms = new SearchParams();
            }

            var query = Context.ShootInfos.AsQueryable();

            if (!string.IsNullOrEmpty(prms.c_num)) {
                query = query.Where(c => c.c_num == prms.c_num);
            }

            if (prms.date_from.HasValue) { 
                var from_dt = prms.date_from.Value.Date;
                query = query.Where(c => c.shoot_date >= from_dt);
            }

            if (prms.date_to.HasValue) { 
                var to_dt = prms.date_to.Value.Date.AddDays(1);        //To日付は「<」で比較するので1日加算する。
                query = query.Where(c => c.shoot_date < to_dt);
            }

            query = query.OrderBy(c => c.shoot_date).ThenBy(c=> c.shoot_time).ThenBy(c => c.info_id);

            var list = await Task.Run( () =>  query.ToList() );
            ObjectReflectionHelper.TrimStrings(list);
            return list; 
        }

        public async Task<bool> Exists(int op_seq) {
            bool result = await Task.Run( () => Context.ShootInfos.Count(e => e.op_seq == op_seq) > 0 );
            return result;
        }


        //public async Task<ShootInfoResult> Save(ShootInfo shootInfo) {
        //    var result = new ShootInfoResult();

        //    try {

        //        if (string.IsNullOrEmpty(shootInfo.item_cd) || string.IsNullOrEmpty(shootInfo.sub_agent_cd)) {
        //            throw new Exception("Photo Plan must have Sales info such as item code and agent code.");
        //        }

        //        if (shootInfo.op_seq == 0) {
        //            await Task.Run(() => {
        //                var procName = "usp_ins_plan_sales";
        //                var prms = new SqlParamSet();
        //                var return_int = prms.AddInt("@RETURN", null, System.Data.ParameterDirection.ReturnValue);
        //                prms.AddChar("@c_num", 7, shootInfo.c_num);
        //                prms.AddChar("@item_cd", 15, shootInfo.item_cd);
        //                prms.AddBit("@cust_collect", shootInfo.cust_collect);
        //                prms.AddChar("@agent_cd", 6, shootInfo.agent_cd);
        //                prms.AddChar("@sub_agent_cd", 6, shootInfo.sub_agent_cd);
        //                //prms.AddChar("@church_cd", 5, shootInfo.church_cd);
        //                prms.AddDateTime("@wed_date", shootInfo.shoot_date);
        //                prms.AddDateTime("@wed_time", shootInfo.shoot_time);
        //                //prms.AddNVarChar("@shoot_place", 100, shootInfo.shoot_place);
        //                prms.AddNVarChar("@staff", 50, shootInfo.staff);
        //                prms.AddChar("@book_status", 1, shootInfo.book_status);
        //                prms.AddNVarCharMax("@note", shootInfo.note);
        //                prms.AddVarChar("@last_person", 15, shootInfo.last_person);
        //                var return_mess = prms.AddVarChar("@return_mess", 10, null, System.Data.ParameterDirection.Output);

        //                using (var tr = Context.BeginTrans()) {
        //                    try {
        //                        Context.ExecuteStoredProcedure(procName, prms.ToArray(), tr);

        //                        var op_seq = TypeHelper.GetInt(return_int.Value);
        //                        if (op_seq > 0) {
        //                            result.op_seq = op_seq;
        //                            result.message = "ok";
        //                            tr.Commit();
        //                        } else {
        //                            var s = string.Format("Could not save Photo Plan. C#:'{0}', Rtn:'{1}'", shootInfo.c_num, op_seq);
        //                            throw new Exception(s);
        //                        }
        //                    } catch (Exception ex) {
        //                        tr.Rollback();
        //                        throw ex;
        //                    }
        //                }
        //            });

        //        } else {
        //            await Task.Run(() => {
        //                var procName = "usp_upd_plan_sales";
        //                var prms = new SqlParamSet();
        //                var return_int = prms.AddInt("@RETURN", null, System.Data.ParameterDirection.ReturnValue);
        //                prms.AddInt("@op_seq", shootInfo.op_seq);
        //                prms.AddChar("@book_status", 1, shootInfo.book_status);
        //                prms.AddNVarCharMax("@note", shootInfo.note);
        //                prms.AddVarChar("@last_person", 15, shootInfo.last_person);

        //                using (var tr = Context.BeginTrans()) {
        //                    try {
        //                        Context.ExecuteStoredProcedure(procName, prms.ToArray(), tr);

        //                        var rtn = TypeHelper.GetInt(return_int.Value);
        //                        if (rtn == 0) {
        //                            result.op_seq = shootInfo.op_seq;
        //                            result.message = "ok";
        //                            tr.Commit();
        //                        } else {
        //                            var s = string.Format("Could not update Photo Plan. op_seq:'{0}', rtn:'{1}'", shootInfo.op_seq, rtn);
        //                            throw new Exception(s);
        //                        }
        //                    } catch (Exception ex) {
        //                        tr.Rollback();
        //                        throw ex;
        //                    }
        //                }
        //            });
        //        }



        //    } catch (Exception ex) {
        //        var msg = "";
        //        while (ex.InnerException != null) {
        //            ex = ex.InnerException;
        //        }
        //        msg = ex.Message;
        //        result.message = msg;
        //    }

        //    return result;
        //}

    }
}
