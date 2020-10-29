using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Threading.Tasks;
using CBAF;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using MauloaDemo.Models.Helpers;
using MauloaDemo.Repository.Exceptions;

namespace MauloaDemo.Repository {

    public class WedInfoRepository : BaseRepository<WedInfo> {

        public class SearchParams {
            public string c_num { get; set; }
            public DateTime? date_from { get; set; }
            public DateTime? date_to { get; set; }

            public SearchParams() {
                date_from = null;
                date_to = null;
            }
        }

        public WedInfoRepository() {
        }
        public WedInfoRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
        }

        /// <summary>
        /// 挙式・フォトプランの保存処理の結果を保持するクラス。
        /// </summary>
        public class WedInfoResult {
            public string c_num { get; set; }
            public int op_seq { get; set; }
            public string message { get; set; }
            private string _status;
            public IEnumerable<Customer> dupList { get; set; }

            public string status { 
                get{
                    return _status;
                } 
                set{
                    _status = value;

                    //ステータスに応じてmessageを自動的にセット。
                    switch (_status) { 
                        case "ok":
                            this.message = "";
                            break;
                        case "invalid":
                            this.message = "Order cannot be saved because of invalid op_seq.";
                            break;
                        case "status":
                            this.message = "Order cannot be saved because of invalid status.";
                            break;
                        case "booked":
                            this.message = "Order has been saved as 'RQ' because the block is already booked.";
                            break;
                        case "closed":
                            this.message = "Order has been saved as 'RQ' because the block is closed.";
                            break;
                        case "agent":
                            this.message = "Order has been saved as 'RQ' because the block is not for this agent.";
                            break;
                        default:
                            break;
                    }
                } 
            }
        }

        //Findにパラメータを追加してあるので、もし間違って基底クラスのFindが呼ばれた場合は例外を発生する。
        public new WedInfo Find(params object[] keyValues) {
            throw new NotImplementedException("Use Find(int info_id, LoginUser user) instead.");
        }

        //Findにパラメータを追加してあるので、もし間違って基底クラスのFindAsyncが呼ばれた場合は例外を発生する。
        public new async Task<WedInfo> FindAsync(params object[] keyValues) {
            await Task.Run(() => {
                //Dummy. Nothing to do actually.
            });
            throw new NotImplementedException("Use FindAsync(int info_id, LoginUser user) instead.");
        }

        public WedInfo FindByOpSeq(int op_seq, LoginUser user) {
            string sql = @"SELECT w.*
                                , s.agent_cd
                                , s.sub_agent_cd
                                , s.inv_agent
                                , s.cust_collect
                                , s.quantity
                                , s.price
                                , s.amount
                                , s.book_status
                                , s.staff
                                , s.branch_staff
                                , s.item_type
                                , s.item_cd
                                , i.item_name 
                                , i.item_name_jpn
                                , i.rq_default
                                , c.church_cd
                                , c.church_name 
                                , c.church_name_jpn
                            FROM wed_info w
                                INNER JOIN sales s ON (s.op_seq = w.op_seq) 
                                INNER JOIN item i ON (s.item_cd = i.item_cd)
                                INNER JOIN church c ON (i.church_cd = c.church_cd)
                            WHERE (w.op_seq = @op_seq)";

            var prms = new SqlParamSet();
            prms.AddInt("@op_seq", op_seq);

            var info = Context.Database.SqlQuery<WedInfo>(sql, prms.ToArray())
                            .SingleOrDefault();
            if (info != null) {
                ObjectReflectionHelper.TrimStrings(info);
                info.StatusList = BookingStatus.GetAvailableStatusList(info.book_status, user.IsStaff());
            }

            return info;
        }

        public async Task<WedInfo> FindByOpSeqAsync(int op_seq, LoginUser user) {
            return await Task<WedInfo>.Run(() => this.FindByOpSeq(op_seq, user));
        }

        public WedInfo Find(int info_id, LoginUser user) {
            string sql = @"SELECT w.*
                                , s.agent_cd
                                , s.sub_agent_cd
                                , s.inv_agent
                                , s.cust_collect
                                , s.quantity
                                , s.price
                                , s.amount
                                , s.book_status
                                , s.staff
                                , s.branch_staff
                                , s.item_type
                                , s.item_cd
                                , i.item_name 
                                , i.item_name_jpn
                                , i.rq_default
                                , c.church_cd
                                , c.church_name 
                                , c.church_name_jpn
                            FROM wed_info w
                                INNER JOIN sales s ON (s.op_seq = w.op_seq) 
                                INNER JOIN item i ON (s.item_cd = i.item_cd)
                                INNER JOIN church c ON (i.church_cd = c.church_cd)
                            WHERE (w.info_id = @info_id)";

            var prms = new SqlParamSet();
            prms.AddInt("@info_id", info_id);

            var info = Context.Database.SqlQuery<WedInfo>(sql, prms.ToArray())
                            .SingleOrDefault();

            if (info != null) {
                //文字列のTrim処理など。
                ObjectReflectionHelper.TrimStrings(info);

                AfterLoad(info, user);
            }

            return info;
        }

        public async Task<WedInfo> FindAsync(int info_id, LoginUser user) {
            var info = await Task<WedInfo>.Run(() => Find(info_id, user));
            return info;
        }

        private void AfterLoad(WedInfo info, LoginUser user) {
            //次に選択可能なStatusのリスト。
            info.StatusList = BookingStatus.GetAvailableStatusList(info.book_status, user.IsStaff());

            //inv_agentがNULLの場合はCUSTに変換。
            if (string.IsNullOrEmpty(info.inv_agent)) {
                info.inv_agent = Sales.INV_AGENT_CUST;
            }

            //フォトプランで時間が「Sunset」の時間の場合はis_sunsetフラグを立てる。
            if ("PHP".Equals(info.item_type)) {
                var SunsetBlockTime = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["SunsetBlockTime"]);
                if (string.Equals(info.req_wed_time.ToString("HH:mm"), SunsetBlockTime)) {
                    info.is_sunset = true;
                }
            }
        }

        public async Task<IEnumerable<WedInfo>> GetList(string c_num, string plan_type, LoginUser user) {

            string sql = @"SELECT w.*
                                , s.agent_cd
                                , s.sub_agent_cd
                                , s.inv_agent
                                , s.cust_collect
                                , s.quantity
                                , s.price
                                , s.amount
                                , s.book_status
                                , s.staff
                                , s.branch_staff
                                , s.item_type
                                , s.item_cd
                                , i.item_name 
                                , i.item_name_jpn
                                , i.rq_default
                                , c.church_cd
                                , c.church_name 
                                , c.church_name_jpn
                            FROM wed_info w
                                INNER JOIN sales s ON (s.op_seq = w.op_seq) 
                                INNER JOIN item i ON (s.item_cd = i.item_cd)
                                INNER JOIN church c ON (i.church_cd = c.church_cd)
                            WHERE (w.c_num = @c_num)";

            var prms = new SqlParamSet();
            prms.AddChar("@c_num", 7, c_num);

            if (!string.IsNullOrEmpty(plan_type)) {
                sql += "    AND (i.item_type = @item_type)";
                prms.AddChar("@item_type", 3, "W".Equals(plan_type) ? "PKG" : "PHP");
            } 
            sql += " ORDER BY w.info_id DESC ";

            var list = await Task.Run(() => {
                            return Context.Database.SqlQuery<WedInfo>(sql, prms.ToArray())
                                                    .ToList();
                        });
            ObjectReflectionHelper.TrimStrings(list);

            list.ForEach(w => AfterLoad(w, user));

            return list;
        }


        public async Task<bool> Exists(int op_seq) {
            bool result = await Task.Run( () => Context.WedInfos.Count(e => e.op_seq == op_seq) > 0 );
            return result;
        }

        public List<Sales> GetPlanItems(int op_seq, LoginUser loginUser) {
            var salesRepo = new SalesRepository(this);
            Sales sales;

            List<Sales> salesList = new List<Sales>();
            var list = salesRepo.GetChildren(op_seq, null);
            foreach (var child in list) {
                sales = salesRepo.Find(child.op_seq, loginUser);
                salesList.Add(sales);
            }
            return salesList;
        }


        public async Task<WedInfoResult> CreateWithCustomer(Customer customer, WedInfo wedInfo, LoginUser loginUser) {
            var result = new WedInfoResult();

            try {
                if (!string.IsNullOrEmpty(customer.c_num)) {
                    throw new Exception(string.Format("Customer is already saved. (C#='{0}')", customer.c_num));
                }
                if (wedInfo.op_seq != 0) {
                    throw new Exception(string.Format("Wedding Info is already saved. (op_seq={0})", wedInfo.op_seq));
                }

                customer.last_person = loginUser.login_id;
                HankakuHelper.Apply(customer);
                UpperCaseHelper.Apply(customer);
                wedInfo.last_person = loginUser.login_id;
                HankakuHelper.Apply(wedInfo);
                UpperCaseHelper.Apply(wedInfo);

                customer.ValidateSave();
                wedInfo.ValidateSave();

                wedInfo.req_wed_date = wedInfo.req_wed_date.Date;       //挙式日に時刻が含まれない様に。

                using (var tr = Context.BeginTrans(IsolationLevel.ReadUncommitted)) {
                    try {
                        //カスタマーを新規保存。
                        var customerRepository = new CustomerRepository(this);
                        var cust_result = await customerRepository.SaveCustomer(customer, loginUser, tr);
                        if (cust_result.message != "ok") {
                            throw new Exception(cust_result.message);
                        }

                        //プラン申し込みを新規保存。
                        wedInfo.c_num = cust_result.c_num;
                        result = this.Save(wedInfo, loginUser, tr);

                        tr.Commit();
                    } catch (Exception) {
                        tr.Rollback();
                        throw;
                    }
                }

            } catch (CustNameDuplicateException) {
                //氏名の重複が見つかった場合はそのまま同じ例外を投げる。
                throw;

            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                result.message = ex.Message;
                result.status = "error";
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wedInfos"></param>
        /// <param name="loginUser"></param>
        /// <param name="tr"></param>
        /// <returns></returns>
        public List<WedInfoResult> SavePlans(WedInfo[] wedInfos, LoginUser loginUser, IDbTransaction tr) {
            List<WedInfoResult> results = new List<WedInfoResult>();

            //複数のOK申込みを行う事は出来ない。
            if (wedInfos.ToList().Count(i => i.book_status == BookingStatus.OK) >= 2) {
                results.Add(new WedInfoResult() { 
                    status = "error", 
                    message = loginUser.Language == "en" ? "Cannot save more than 2 plans with OK status." 
                                                         : "OKステータスのプランを2つ以上保存する事は出来ません。"
                });
            }

            //トランザクション開始前に全ての申込みに対してBeforeSaveメソッドを呼ぶ。
            wedInfos.ToList().ForEach(i => {
                try {
                    //各プロパティのValidate、2週間未満の新規OK申込みのRQへの変更などを行う。
                    this.BeforeSave(i, loginUser, false);
                } catch (Exception ex) {
                    while (ex.InnerException != null) { ex = ex.InnerException; }
                    results.Add(new WedInfoResult() { status = "error", message = ex.Message });
                }
            });

            //ここまでで何か問題があればこの時点で結果を返して終了。
            if (results.Count > 0) return results;


            //ここからトランザクション開始。
            var need_commit = false;
            if (tr == null) {
                tr = Context.BeginTrans(IsolationLevel.ReadUncommitted);
                need_commit = true;
            }
            try {
                List<int> existing_oks_in_db = new List<int>();

                //既存でCXL分だけを先に保存。
                wedInfos.Where(i => i.op_seq != 0 && i.book_status == BookingStatus.CXL).ToList().ForEach(i => {
                    results.Add(this.Save(i, loginUser, tr));
                });

                //OK申込みがある場合
                var ok_status = wedInfos.SingleOrDefault(p => p.book_status == BookingStatus.OK);
                if (ok_status != null) {
                    //item_cdからitem_typeを得る。
                    var itemRepository = new ItemRepository(this);
                    ok_status.item_type = itemRepository.GetItemTypeByItemCd(ok_status.item_cd);

                    //既に他にOKまたはCXLRQ中の申込みがあるかチェック。
                    existing_oks_in_db = Context.Sales.Where(s => s.c_num == ok_status.c_num
                                                             && s.op_seq != ok_status.op_seq
                                                             && (s.book_status == BookingStatus.OK || s.book_status == BookingStatus.CXLRQ)
                                                             && s.item_type == ok_status.item_type)
                                            .Select(s => s.op_seq)
                                            .ToList();
                    //既にある場合はそれらをCXLに変更する。
                    foreach (var op_seq in existing_oks_in_db) {
                        var wedInfo = this.FindByOpSeq(op_seq, loginUser);
                        wedInfo.last_person = loginUser.login_id;
                        wedInfo.book_status = BookingStatus.CXL;
                        //wedInfo.PlanItems = GetPlanItems(op_seq, loginUser);
                        this.Save(wedInfo, loginUser, tr, true, true);
                    }
                }

                //既存でCXL以外を保存。
                wedInfos.Where(i => i.op_seq != 0 && 
                                    i.book_status != BookingStatus.CXL && 
                                    !existing_oks_in_db.Contains(i.op_seq) )
                        .ToList()
                        .ForEach(i => {
                    results.Add(this.Save(i, loginUser, tr));
                });

                //新規分を保存。
                wedInfos.Where(i => i.op_seq == 0)
                        .ToList()
                        .ForEach(i => {
                    results.Add(this.Save(i, loginUser, tr));
                });

                if (need_commit) tr.Commit();

            } catch (Exception ex) {
                if (need_commit) tr.Rollback();

                while (ex.InnerException != null) { ex = ex.InnerException; }
                throw ex;
            }
            return results;
        }

        private void BeforeSave(WedInfo wedInfo, LoginUser loginUser, bool enforceStatus) {
            var isNew = (wedInfo.op_seq == 0);
            wedInfo.last_person = loginUser.login_id;
            HankakuHelper.Apply(wedInfo);
            wedInfo.ValidateSave();

            if (wedInfo.inv_agent == Sales.INV_AGENT_CUST || string.IsNullOrEmpty(wedInfo.inv_agent)) {
                wedInfo.inv_agent = null;
                wedInfo.cust_collect = true;
            } else {
                wedInfo.cust_collect = false;
            }

            //エージェントユーザーの場合はsub_agent_cdをチェック。
            if (loginUser.IsAgent()) {
                var subAgentRepo = new SubAgentRepository(this);
                if (!subAgentRepo.HasAccessTo(loginUser.sub_agent_cd, wedInfo.sub_agent_cd)) {
                    throw new InvalidOperationException("Agent code is invalid.");
                }
            }

            //item_cdからitem_typeを得る。
            var itemRepository = new ItemRepository(this);
            wedInfo.item_type = itemRepository.GetItemTypeByItemCd(wedInfo.item_cd);
            if (string.IsNullOrEmpty(wedInfo.item_type)) {
                throw new InvalidOperationException("Item code is invalid.");
            }

            //念のため挙式日から時刻を除去。
            wedInfo.req_wed_date = wedInfo.req_wed_date.Date;

            if (!enforceStatus) {
                //挙式日までの日数を計算。
                var days = wedInfo.req_wed_date.Subtract(RegionConfig.GetRegionToday(this.RegionCd)).Days;
                var Photo_RQ_LimitDays = TypeHelper.GetInt(ConfigurationManager.AppSettings["Photo_RQ_LimitDays"]);

                if (isNew) {
                    if (loginUser.IsAgent()) {
                        //新規の場合にプラン申込が「RQ」になる条件
                        if (wedInfo.item_type == "PKG") {
                            //挙式プランは常に「RQ」。
                            wedInfo.book_status = BookingStatus.RQ;
                        } else {
                            //フォトプランなら実施日まで2週間未満の場合は「RQ」、2週間以上の場合は「OK」とする。
                            if (days < Photo_RQ_LimitDays) {
                                wedInfo.book_status = BookingStatus.RQ;
                            }

                            //フォトプランの特定の撮影場所(church_cd)については常に「RQ」とする。
                            var RQ_churches = TypeHelper.GetStr(ConfigurationManager.AppSettings["Photo_RQ_Places"]);
                            if (RQ_churches.Contains("[" + wedInfo.req_church_cd + "]")) {
                                wedInfo.book_status = BookingStatus.RQ;
                            }
                        }
                    }

                    //Itemマスターでrq_defaultフラグが立っている場合はRQ申込みにする。
                    if (wedInfo.rq_default && wedInfo.book_status == BookingStatus.OK) {
                        wedInfo.book_status = BookingStatus.RQ;
                    }

                } else {
                    //既存の場合、ステータスの変更可能条件をチェック。
                    var salesRepository = new SalesRepository(this);
                    salesRepository.ValidateNewStatus(wedInfo.c_num, wedInfo.op_seq, wedInfo.book_status, loginUser.IsStaff());

                    //現在のbook_statusをsalesから取得する。
                    var cur_status = salesRepository.GetBookingStatus(wedInfo.op_seq);

                    //エージェントユーザーがステータスを「OK」から「CXL」に変更した時
                    if (loginUser.IsAgent() && cur_status == BookingStatus.OK && wedInfo.book_status == BookingStatus.CXL) {
                        if (wedInfo.item_type == "PKG") {
                            //挙式プランなら常に「CXLRQ」。
                            wedInfo.book_status = BookingStatus.CXLRQ;
                        } else {
                            //フォトプランなら実施日まで2週間未満の場合は「CXLRQ」、2週間以上の場合は「CXL」とする。
                            if (days < Photo_RQ_LimitDays) {
                                wedInfo.book_status = BookingStatus.CXLRQ;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="wedInfo"></param>
        /// <param name="loginUser"></param>
        /// <param name="tr"></param>
        /// <returns></returns>
        public WedInfoResult Save(
                        WedInfo wedInfo, 
                        LoginUser loginUser, 
                        IDbTransaction tr, 
                        bool enforceStatus = false, 
                        bool withChildren = false) {

            var result = new WedInfoResult();
            var isNew = (wedInfo.op_seq == 0);

            try {
                this.BeforeSave(wedInfo, loginUser, enforceStatus);

                var subAgentRepository = new SubAgentRepository(this);
                var agent_cd = subAgentRepository.GetAgentCd(wedInfo.sub_agent_cd);
                if (string.IsNullOrEmpty(agent_cd)) {
                    throw new Exception(string.Format("Cannot find '{0}' as a Sub Agent.", wedInfo.sub_agent_cd));
                }
                wedInfo.agent_cd = agent_cd;

                if (!string.IsNullOrEmpty(wedInfo.inv_agent)) {
                    var inv_agent_parent = subAgentRepository.GetAgentCd(wedInfo.inv_agent);
                    //請求先の親Agentと手配元の親Agentが一致しない場合はエラーとする。
                    if (!string.Equals(inv_agent_parent, wedInfo.agent_cd)) {
                        throw new Exception(string.Format("Agent code '{0}' and Invoice agent '{1}' does not match.", wedInfo.agent_cd, inv_agent_parent));
                    }
                }

                if ((wedInfo.book_status == BookingStatus.OK || 
                     wedInfo.book_status == BookingStatus.RQ) 
                    && loginUser.branch_staff_required 
                    && string.IsNullOrEmpty(wedInfo.branch_staff)) {
                    throw new Exception("Branch staff name is required.");
                }


                WedInfo oldModel = null;
                if (!isNew) {
                    //変更前の状態を取得。(ログに差分を保存するため。)
                    oldModel = this.Find(wedInfo.info_id, loginUser);
                }

                var procName = "";
                var prms = new SqlParamSet();
                var return_int = prms.AddInt("@RETURN", null, System.Data.ParameterDirection.ReturnValue);

                if (isNew) {
                    procName = "usp_ins_plan_sales";
                    prms.AddChar("@c_num", 7, wedInfo.c_num);
                    prms.AddChar("@item_cd", 15, wedInfo.item_cd);
                    prms.AddBit("@cust_collect", wedInfo.cust_collect);
                    prms.AddChar("@agent_cd", 6, wedInfo.agent_cd);
                    prms.AddChar("@sub_agent_cd", 6, wedInfo.sub_agent_cd);
                    prms.AddChar("@inv_agent", 6, wedInfo.inv_agent);
                    prms.AddChar("@church_cd", 5, wedInfo.req_church_cd);
                    prms.AddDateTime("@wed_date", wedInfo.req_wed_date);
                    prms.AddDateTime("@wed_time", wedInfo.req_wed_time);
                    prms.AddBit("@is_irregular_time", wedInfo.is_irregular_time);
                    prms.AddNVarChar("@staff", 50, wedInfo.staff);
                    prms.AddNVarChar("@branch_staff", 50, wedInfo.branch_staff);
                    prms.AddChar("@book_status", 1, wedInfo.book_status);
                    prms.AddNVarCharMax("@note", wedInfo.note);
                    prms.AddVarChar("@last_person", 15, wedInfo.last_person);
                } else {
                    procName = "usp_upd_plan_sales";
                    prms.AddInt("@op_seq", wedInfo.op_seq);
                    prms.AddChar("@agent_cd", 6, wedInfo.agent_cd);
                    prms.AddChar("@sub_agent_cd", 6, wedInfo.sub_agent_cd);
                    prms.AddChar("@inv_agent", 6, wedInfo.inv_agent);
                    prms.AddNVarChar("@staff", 50, wedInfo.staff);
                    prms.AddNVarChar("@branch_staff", 50, wedInfo.branch_staff);
                    prms.AddChar("@book_status", 1, wedInfo.book_status);
                    prms.AddNVarCharMax("@note", wedInfo.note);
                    prms.AddVarChar("@last_person", 15, wedInfo.last_person);
                    prms.AddBit("@with_children", withChildren);
                }
                var return_mess = prms.AddVarChar("@return_mess", 10, null, ParameterDirection.Output);

                var need_commit = false;
                if (tr == null) {
                    tr = Context.BeginTrans();
                    need_commit = true;
                }
                try {
                    //プランを保存。
                    Context.ExecuteStoredProcedure(procName, prms.ToArray(), tr);

                    //"ok", "invalid", "status", "booked", "closed", "agent"のいずれかが返って来る。
                    var rtn_mess = TypeHelper.GetStrTrim(return_mess.Value).ToLower();

                    if (isNew) {
                        wedInfo.op_seq = TypeHelper.GetInt(return_int.Value);
                        if (wedInfo.op_seq == 0) {
                            throw new Exception("Could not save the plan order. (Returned op_seq was 0.)");
                        }
                    }

                    if (!"ok".Equals(rtn_mess)) {
                        var changed_status = Context.Sales
                                        .Where(s => s.op_seq == wedInfo.op_seq)
                                        .Select(s => s.book_status)
                                        .SingleOrDefault();

                        if (!string.IsNullOrEmpty(changed_status) && !string.Equals(changed_status, wedInfo.book_status)) {
                            wedInfo.book_status = changed_status;
                        }
                    }

                    var newModel = this.FindByOpSeq(wedInfo.op_seq, loginUser);

                    //変更ログを保存。
                    var log = new LogChangeRepository(this);
                    log.InsertLog(wedInfo.last_person, wedInfo.c_num, oldModel, newModel);

                    var salesRepository = new SalesRepository(this);

                    //新規もしくは、既存かつストアド内で子アイテムを処理*しない*場合、
                    if (isNew || !withChildren) {
                        //プランに含まれる子アイテムを保存。
                        foreach (Sales item in wedInfo.PlanItems) {
                            var sales = new Sales {
                                op_seq = item.op_seq,
                                c_num = wedInfo.c_num,
                                item_type = item.item_type,
                                item_cd = item.item_cd,
                                staff = wedInfo.staff,
                                branch_staff = wedInfo.branch_staff,
                                quantity = item.quantity,
                                note = item.note,
                                cust_collect = item.cust_collect,
                                tentative_price = item.tentative_price,
                                price = item.price,
                                amount = item.amount,
                                parent_op_seq = wedInfo.op_seq,
                                create_by = wedInfo.create_by,
                                create_date = wedInfo.create_date,
                                last_person = wedInfo.last_person,
                                update_date = wedInfo.update_date,
                                agent_cd = wedInfo.agent_cd,
                                sub_agent_cd = wedInfo.sub_agent_cd,
                                inv_agent = wedInfo.inv_agent,
                                book_status = wedInfo.book_status,
                                sales_post_date = wedInfo.req_wed_date,

                                DeliveryInfo = item.DeliveryInfo,
                                MakeInfo = item.MakeInfo,
                                ShootInfo = item.ShootInfo,
                                ReceptionInfo = item.ReceptionInfo,
                                TransInfo = item.TransInfo
                            };

                            if (sales.op_seq != 0) {
                                sales.Arrangements = new ArrangementRepository(this)
                                                        .GetArrangementsByOpSeq(sales.op_seq);
                            }

                            salesRepository.Save(sales, loginUser, tr);
                        }
                    }

                    if (need_commit) tr.Commit();

                    result.c_num = wedInfo.c_num;
                    result.op_seq = wedInfo.op_seq;

                    //statusプロパティのSetterの中で "ok", "invalid", "status", "booked", "closed", "agent" から画面に表示する文字列に変換してmessageプロパティにセット。
                    result.status = rtn_mess;   

                } catch (Exception ex) {
                    if (need_commit) tr.Rollback();
                    throw ex;
                }

            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                result.status = "error";
                result.message = ex.Message;
            }

            return result;
        }

        public async Task<WedInfoResult> SaveAsync(WedInfo wedInfo, LoginUser loginUser, IDbTransaction tr) {
            return await Task.Run(() => this.Save(wedInfo, loginUser, tr));
        }

        public async Task<ChurchOrderSheetInfo> GetChurchOrderSheetInfo(string c_num, int info_id, LoginUser user) {
            //ログインユーザーのカスタマーへの権限をチェック。
            var loginUserRepo = new LoginUserRepository(this);
            if (!loginUserRepo.CanViewCustomer(user, c_num)) {
                throw new InvalidOperationException("Not authorized.");
            }

            var info = new ChurchOrderSheetInfo();

            await Task.Run(() => info.WedInfo = this.Find(info_id, user) );
            if (info.WedInfo == null) return null;
            if (!string.Equals(info.WedInfo.c_num, c_num)) return null;

            var customerRepository = new CustomerRepository(this);
            info.Customer = customerRepository.Find(info.WedInfo.c_num);
            if (info.Customer == null) return null;

            var churchRepository = new ChurchRepository(this);
            info.Church = churchRepository.Find(info.WedInfo.req_church_cd);

            return info;
        }

    }
}
