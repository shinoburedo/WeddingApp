using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CBAF;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using MauloaDemo.Repository.Exceptions;


namespace MauloaDemo.Repository {

    public class SalesRepository : BaseRepository<Sales> {

        public class SearchParams {
            public string item_type { get; set; }
            public string church_cd { get; set; }
            public string item_cd { get; set; }
            public string item_name { get; set; }
            public DateTime? wed_date { get; set; }
        }

        public class SalesResult {
            public int op_seq { get; set; }
            //public Sales sales { get; set; }
            public string message { get; set; }
        }


        public SalesRepository() {
        }

        public SalesRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            Mapper.CreateMap<Sales, Sales>();
            Mapper.AssertConfigurationIsValid();
        }


        //Findにパラメータを追加してあるので、もし間違って基底クラスのFindが呼ばれた場合は例外を発生する。
        public new Sales Find(params object[] keyValues) {
            throw new NotImplementedException("Use Find(int op_seq, LoginUser user) instead.");
        }

        //Findにパラメータを追加してあるので、もし間違って基底クラスのFindAsyncが呼ばれた場合は例外を発生する。
        public new async Task<Sales> FindAsync(params object[] keyValues) {
            await Task.Run(() => {
                //Dummy. Nothing to do actually.
            });
            throw new NotImplementedException("Use FindAsync(int op_seq, LoginUser user) instead.");
        }

        /// <summary>
        /// op_seqからSalesオブジェクトを取得する。
        /// </summary>
        /// <param name="op_seq"></param>
        /// <returns></returns>
        public Sales Find(int op_seq, LoginUser user) {

            var sales = Context.Sales
                            .Include("Item")
                            .Include("Item.ItemType")
                            .AsNoTracking()
                            .Where(s => s.op_seq == op_seq)
                            .SingleOrDefault();

            if (sales == null) return null;

            //排他制御用のTime Stampを専用のプロパティに保持。
            sales.update_date_stamp = sales.update_date;

            //文字列のTrim処理など。
            ApplyMappings(sales);

            //次に選択可能なStatusのリスト。
            sales.StatusList = BookingStatus.GetAvailableStatusList(sales.book_status, user.IsStaff());

            //inv_agentがNULLの場合はCUSTに変換。
            if (string.IsNullOrEmpty(sales.inv_agent)) {
                sales.inv_agent = Sales.INV_AGENT_CUST;
            }

            ////Arrangementsのリスト。
            //sales.Arrangements = GetArrangements(op_seq);

            //info_typeに応じて各種infoテーブルの情報を読み込む。
            this.LoadInfo(sales);

            return sales;
        }

        public async Task<Sales> FindAsync(int op_seq, LoginUser user) {
            var sales = await Task.Run(() => this.Find(op_seq, user));
            return sales;
        }

        public void LoadInfo(Sales sales) {
            if (sales == null) return;
            if (sales.Item == null) return;
            if (sales.Item.ItemType == null) return;

            switch (sales.Item.ItemType.info_type) { 
                case "DLV":
                    var dlv = Context.DeliveryInfos.Where(i => i.op_seq == sales.op_seq).SingleOrDefault();
                    ObjectReflectionHelper.TrimStrings(dlv);
                    if (dlv == null) dlv = new DeliveryInfo { op_seq = sales.op_seq };
                    sales.DeliveryInfo = dlv;
                    break;
                case "MKS":
                    var mks = Context.MakeInfos.Where(i => i.op_seq == sales.op_seq).SingleOrDefault();
                    ObjectReflectionHelper.TrimStrings(mks);
                    if (mks == null) mks = new MakeInfo { op_seq = sales.op_seq };
                    sales.MakeInfo = mks;
                    break;
                case "RCP":
                    var rcp = Context.RcpInfos.Where(i => i.op_seq == sales.op_seq).SingleOrDefault();
                    ObjectReflectionHelper.TrimStrings(rcp);
                    if (rcp == null) rcp = new RcpInfo{ op_seq = sales.op_seq };
                    sales.ReceptionInfo = rcp;
                    break;
                case "SHO":
                case "SHT":
                    var sht = Context.ShootInfos.Where(i => i.op_seq == sales.op_seq).SingleOrDefault();
                    ObjectReflectionHelper.TrimStrings(sht);
                    if (sht == null) sht = new ShootInfo{ op_seq = sales.op_seq };
                    sales.ShootInfo = sht;
                    break;
                case "TRN":
                    var trn = Context.TransInfos.Where(i => i.op_seq == sales.op_seq).SingleOrDefault();
                    ObjectReflectionHelper.TrimStrings(trn);
                    if (trn == null) {
                        trn = new TransInfo { op_seq = sales.op_seq };
                    } else {
                        trn.pickup_hotel_name = Context.Hotels.Where(i => i.hotel_cd == trn.pickup_hotel).Select(m => m.hotel_name).SingleOrDefault();
                        trn.dropoff_hotel_name = Context.Hotels.Where(i => i.hotel_cd == trn.dropoff_hotel).Select(m => m.hotel_name).SingleOrDefault();
                    }
                    sales.TransInfo = trn;
                    break;
            }
        }


        public async Task<IEnumerable<SalesListItem>> GetOptions(string c_num, string agent_cd) {
            var sql = @"EXEC usp_get_options @c_num, @agent_cd ";
            var prms = new SqlParamSet();
            prms.AddChar("@c_num", 7, c_num);
            prms.AddChar("@agent_cd", 6, agent_cd);

            var list = await Task.Run(() => Context.Database.SqlQuery<SalesListItem>(sql, prms.ToArray()).ToList());
            ObjectReflectionHelper.TrimStrings(list);
            var repo = new ArrangementRepository(this);
            foreach (var item in list) {
                item.arrangements = repo.GetArrangementsByOpSeq(item.op_seq).ToList();
            }
            return list;
        }

        public IEnumerable<SalesListItem> GetChildren(int op_seq, string item_cd) {
            var sql = @"EXEC usp_get_sales_children @op_seq, @item_cd ";
            var prms = new SqlParamSet();
            prms.AddInt("@op_seq", op_seq);
            prms.AddChar("@item_cd", 15, item_cd);

            var list = Context.Database.SqlQuery<SalesListItem>(sql, prms.ToArray()).ToList();
            ObjectReflectionHelper.TrimStrings(list);
            var repo = new ArrangementRepository(this);
            foreach (var item in list) {
                item.arrangements = repo.GetArrangementsByOpSeq(item.op_seq).ToList();
            }

            return list;
        }

        public async Task<IEnumerable<SalesListItem>> GetChildrenAsync(int op_seq, string item_cd) {
            return await Task.Run(() => GetChildren(op_seq, item_cd));
        }

        public SalesResult Save(Sales sales, LoginUser user, IDbTransaction tr) {
            var result = new SalesResult {
                op_seq = sales.op_seq,
                message = String.Empty
            };

            if (sales.inv_agent == Sales.INV_AGENT_CUST || string.IsNullOrEmpty(sales.inv_agent)) {
                sales.inv_agent = null;
                sales.cust_collect = true;
            } else {
                sales.cust_collect = false;
            }
             
            sales.last_person = user.login_id;
            var isNew = (sales.op_seq == 0);
            Sales oldModel = null;

            try {
                if (string.IsNullOrEmpty(sales.item_cd)) {
                    throw new Exception("Please input an item code for the sales.");
                }
                if (string.IsNullOrEmpty(sales.sub_agent_cd)) {
                    throw new Exception("Please select a sub agent code for the sales.");
                }
                if (!sales.sales_post_date.HasValue) {
                    throw new Exception("Wedding Date(Sales Post Date) is required.");
                }
                if (string.IsNullOrEmpty(sales.last_person)) {
                    throw new Exception("User id is required for the sales.");
                }

                if (string.IsNullOrEmpty(sales.staff)) {
                    if (user.staff_required) {
                        throw new Exception("Staff is required for the sales.");
                    } else { 
                        sales.staff = user.UserName;
                    }
                }

                if (user.branch_staff_required && string.IsNullOrEmpty(sales.branch_staff)) {
                    throw new Exception("Branch staff name is required for the sales.");
                }

                //オプション追加時はカスタマーの挙式日が先に決まっているかどうかチェック。
                if (sales.item_type != "PKG" && sales.item_type != "PHP" && !sales.parent_op_seq.HasValue && tr == null) {
                    if (!"XNC".Contains(sales.book_status)) { 
                        var customerRepo = new CustomerRepository(this);
                        var wed_date = customerRepo.GetCustomerWedDate(sales.c_num);
                        if (!wed_date.HasValue) {
                            throw new Exception("Wedding date is not saved yet. Please save customer's wedding date first.");
                        }
                    }
                }

                if (sales.sales_post_date.HasValue) {
                    //念のためsales_post_dateから時刻を除去。
                    sales.sales_post_date = sales.sales_post_date.Value.Date;
                }

                //sub_agent_cdの存在確認・親agent_cd取得。
                var subAgentRepository = new SubAgentRepository(this);
                var subAgent = subAgentRepository.Find(sales.sub_agent_cd);
                if (subAgent == null) {
                    throw new Exception(string.Format("Cannot find '{0}' as a Sub Agent.", sales.sub_agent_cd));
                }
                sales.agent_cd = subAgent.GetParentCd();

                if (!string.IsNullOrEmpty(sales.inv_agent)) {
                    //請求先の親Agentと手配元の親Agentが一致するかチェック。
                    var inv_agent_parent = subAgentRepository.GetAgentCd(sales.inv_agent);
                    if (!string.Equals(inv_agent_parent, sales.agent_cd)) {
                        throw new Exception(string.Format("Agent code '{0}' and Invoice agent '{1}' does not match.", sales.agent_cd, inv_agent_parent));
                    }
                }

                if (isNew) {
                    //Itemマスターでrq_defaultフラグが立っている場合はRQ申込みにする。(小アイテムでない場合のみ)
                    if (sales.book_status == BookingStatus.OK && (sales.parent_op_seq ?? 0) == 0) { 
                        if (sales.Item == null) {
                            //Itemの情報を取得。
                            sales.Item = new ItemRepository(this).Find(sales.item_cd);
                        }
                        if (sales.Item.rq_default) {
                            sales.book_status = BookingStatus.RQ;
                        }
                    }
                } else {
                    //ステータスの変更可能条件をチェック。
                    if (!ValidateNewStatus(sales.c_num, sales.op_seq, sales.book_status, user.IsStaff())) {
                        throw new Exception(string.Format("New booking status '{0}' is invalid.", sales.book_status));
                    }

                    //変更前の状態を取得。
                    oldModel = this.Find(sales.op_seq, user);
                    if (oldModel == null) {
                        throw new Exception(string.Format("Sales not found. op_seq={0}", sales.op_seq));
                    }

                    if ("NC".Contains(oldModel.book_status)) {
                        throw new Exception(string.Format("Item cannot be modified because it is already canceled. op_seq={0}", sales.op_seq));
                    }

                    //価格が変更されたかどうかチェック。
                    sales.price_changed = (oldModel.org_price != sales.price);
                    if (sales.price_changed) {
                        if (string.IsNullOrWhiteSpace(sales.price_change_reason)) {
                            throw new Exception(string.Format("Price Change Reason is required. Original: {0:c} Changed: {1:c}", oldModel.org_price, sales.price));
                        }
                    } else {
                        sales.price_change_reason = null;
                    }

                    if (!sales.parent_op_seq.HasValue) {
                        //親アイテムのみ楽観的排他制御を行う。 (JSを経由するとミリ秒の情報が欠落するため秒単位で比較する。)
                        if (TypeHelper.CompareDateBySecond(sales.update_date_stamp, oldModel.update_date_stamp) != 0) {
                            throw new OptimisticLockException("Sales", user.IsStaff() ? oldModel.last_person : "");
                        }
                    }
                }

                var procName = "";
                var prms = new SqlParamSet();
                var return_int = prms.AddInt("@RETURN", null, System.Data.ParameterDirection.ReturnValue);
                SqlParameter return_mess = null;

                if (isNew) {
                    procName = "usp_ins_sales";
                    prms.AddChar("@c_num", 7, sales.c_num);
                    prms.AddChar("@item_cd", 15, sales.item_cd);
                    prms.AddBit("@cust_collect", sales.cust_collect);
                    prms.AddChar("@agent_cd", 6, sales.agent_cd);
                    prms.AddChar("@sub_agent_cd", 6, sales.sub_agent_cd);
                    prms.AddChar("@inv_agent", 6, sales.inv_agent);
                    prms.AddNVarChar("@staff", 50, sales.staff);
                    prms.AddNVarChar("@branch_staff", 50, sales.branch_staff);
                    prms.AddSmallInt("@quantity", sales.quantity);
                    prms.AddChar("@book_status", 1, sales.book_status);
                    prms.AddDateTime("@price_date", sales.sales_post_date);
                    prms.AddInt("@parent_op_seq", sales.parent_op_seq);
                    prms.AddVarChar("@last_person", 15, sales.last_person);
                } else {
                    procName = "usp_upd_sales";
                    prms.AddInt("@op_seq", sales.op_seq);
                    prms.AddChar("@agent_cd", 6, sales.agent_cd);
                    prms.AddChar("@sub_agent_cd", 6, sales.sub_agent_cd);
                    prms.AddChar("@inv_agent", 6, sales.inv_agent);
                    prms.AddNVarChar("@staff", 50, sales.staff);
                    prms.AddNVarChar("@branch_staff", 50, sales.branch_staff);
                    prms.AddDecimal("@price", sales.price, 10, 2);
                    prms.AddBit("@price_changed", sales.price_changed);
                    prms.AddNVarChar("@price_change_reason", 200, sales.price_change_reason);
                    prms.AddSmallInt("@quantity", sales.quantity);
                    prms.AddChar("@book_status", 1, sales.book_status);
                    prms.AddDecimal("@cxl_charge", sales.cxl_charge, 10, 2);
                    prms.AddVarChar("@last_person", 15, sales.last_person);
                    return_mess = prms.AddVarChar("@return_mess", 10, null, ParameterDirection.Output);
                }

                var need_commit = false;
                if (tr == null) {
                    tr = Context.BeginTrans();
                    need_commit = true;
                }
                try {
                    Context.ExecuteStoredProcedure(procName, prms.ToArray(), tr);
                    if (isNew) {
                        sales.op_seq = TypeHelper.GetInt(return_int.Value);
                        result.op_seq = sales.op_seq;
                        if (sales.op_seq <= 0) {
                            throw new Exception(string.Format("Could not insert the new sales. ({0})", sales.op_seq));
                        }
                        result.message = "ok";
                    } else {
                        result.message = TypeHelper.GetStr(return_mess.Value);
                        if (!"ok".Equals(result.message)) {
                            throw new Exception(string.Format("Could not update the sales. op_seq={0}, message='{1}'", sales.op_seq, result.message));
                        }
                    }

                    //追加情報(Infoテーブル)を保存
                    var info_changed = this.SaveInfos(sales, tr, isNew, oldModel);

                    //Salesの変更ログを保存。(数量、担当者の変更など)
                    //if (!info_changed || !isNew) {
                    Sales newModel = this.Find(sales.op_seq, user);

                    var log = new LogChangeRepository(this);
                    log.InsertLog(user.login_id, sales.c_num, oldModel, newModel);
                    //}

                    ////PEND: 子アイテムを保存。
                    //    foreach (var child in sales.Children) {
                    //        child.parent_op_seq = sales.op_seq;
                    //        child.last_person = sales.last_person;
                    //        Save(child, tr);
                    //    }

                    //Arrangementがあれば保存。
                    if (sales.Arrangements != null && !isNew) {
                        var arrRepository = new ArrangementRepository(this);
                        foreach (var arr in sales.Arrangements) {
                            arr.c_num = sales.c_num;
                            arr.op_seq = sales.op_seq;
                            arr.last_person = sales.last_person;
                            arr.update_date = DateTime.UtcNow;

                            //SalesがCXLされている場合はArrangementもCXLする。
                            if (sales.book_status == BookingStatus.CXL) {
                                arr.cxl = true;
                                arr.cxl_date = DateTime.UtcNow;
                                arr.cxl_vend_by = sales.last_person;
                            }

                            var arrResult = arrRepository.Save(arr, tr);
                            if (!"ok".Equals(arrResult.status)) {
                                throw new Exception(string.IsNullOrEmpty(arrResult.message) ? "Could not save arrangement." : arrResult.message);
                            }
                        }
                    }

                    if (need_commit) tr.Commit();

                } catch (Exception ex) {
                    if (need_commit) tr.Rollback();
                    throw ex;
                }
            } catch (Exception ex) {
                result.message = ex.Message;
            }
            return result;
        }

        public bool SaveInfos(Sales sales, IDbTransaction tr, bool isNew, Sales oldModel) {
            var changed = false;
            //item_typeからinfo_typeを取得。
            var itemTypeRepository = new ItemTypeRepository(this);
            var itemType = itemTypeRepository.Find(sales.item_type);
            if (itemType == null) return false;

            switch (itemType.info_type) { 
                case "DLV":
                    changed = this.SaveDlvInfo(sales, tr);
                    break;
                case "MKS":
                    changed = this.SaveMksInfo(sales, tr);
                    break;
                case "RCP":
                    changed = this.SaveRcpInfo(sales, tr); 
                    break;
                case "SHO":
                case "SHT":
                    changed = this.SaveShtInfo(sales, tr); 
                    break;
                case "TRN":
                    changed = this.SaveTrnInfo(sales, tr); 
                    break;
                default:
                    changed = this.SaveOtherInfo(sales, tr, isNew, oldModel); 
                    break;
            }
            return changed;
        }

        public bool SaveDlvInfo(Sales sales, IDbTransaction tr) {
            var info = sales.DeliveryInfo;
            if (info == null) return false;
            var isNew = (info.info_id == 0);

            DeliveryInfo oldModel = null;
            if (!isNew) {
                //変更前の状態を取得。(ログに差分を保存するため。)
                oldModel = Context.DeliveryInfos.AsNoTracking().FirstOrDefault(m => m.info_id == info.info_id);
            }

            var procName = "usp_delivery_info";
            var prms = new SqlParamSet();
            var return_int = prms.AddInt("@RETURN", null, System.Data.ParameterDirection.ReturnValue);

            prms.AddInt("@info_id", info.info_id);
            prms.AddInt("@op_seq", sales.op_seq);
            prms.AddDateTime("@delivery_date", info.delivery_date);
            prms.AddDateTime("@delivery_time", info.delivery_time);
            prms.AddNVarChar("@delivery_place", 100, info.delivery_place);
            prms.AddNVarCharMax("@note", info.note);
            prms.AddVarChar("@last_person", 15, sales.last_person);

            Context.ExecuteStoredProcedure(procName, prms.ToArray(), tr);
            if (isNew) {
                info.info_id = TypeHelper.GetInt(return_int.Value);
                if (info.info_id <= 0) {
                    throw new Exception(string.Format("Could not insert the delivery info. ({0})", sales.op_seq));
                }
            }

            DeliveryInfo newModel = Context.DeliveryInfos.AsNoTracking().FirstOrDefault(m => m.info_id == info.info_id);

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            return log.InsertLog(sales.last_person, sales.c_num, oldModel, newModel);
        }

        public bool SaveMksInfo(Sales sales, IDbTransaction tr) {
            var info = sales.MakeInfo;
            if (info == null) return false;
            var isNew = (info.info_id == 0);

            MakeInfo oldModel = null;
            if (!isNew) {
                //変更前の状態を取得。(ログに差分を保存するため。)
                oldModel = Context.MakeInfos.AsNoTracking().FirstOrDefault(m => m.info_id == info.info_id);
            }

            var procName = "usp_make_info";
            var prms = new SqlParamSet();
            var return_int = prms.AddInt("@RETURN", null, System.Data.ParameterDirection.ReturnValue);

            prms.AddInt("@info_id", info.info_id);
            prms.AddInt("@op_seq", sales.op_seq);
            prms.AddDateTime("@make_date", info.make_date);
            prms.AddDateTime("@make_time", info.make_time);
            prms.AddNVarChar("@make_place", 100, info.make_place);
            prms.AddDateTime("@make_in_time", info.make_in_time);
            prms.AddNVarCharMax("@note", info.note);
            prms.AddVarChar("@last_person", 15, sales.last_person);

            Context.ExecuteStoredProcedure(procName, prms.ToArray(), tr);
            if (isNew) {
                info.info_id = TypeHelper.GetInt(return_int.Value);
                if (info.info_id <= 0) {
                    throw new Exception(string.Format("Could not insert the make info. ({0})", sales.op_seq));
                }
            }

            MakeInfo newModel = Context.MakeInfos.AsNoTracking().FirstOrDefault(m => m.info_id == info.info_id);

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            return log.InsertLog(sales.last_person, sales.c_num, oldModel, newModel);
        }

        public bool SaveRcpInfo(Sales sales, IDbTransaction tr) {
            var info = sales.ReceptionInfo;
            if (info == null) return false;
            var isNew = (info.info_id == 0);

            RcpInfo oldModel = null;
            if (!isNew) {
                //変更前の状態を取得。(ログに差分を保存するため。)
                oldModel = Context.RcpInfos.AsNoTracking().FirstOrDefault(m => m.info_id == info.info_id);
            }

            var procName = "usp_rcp_info";
            var prms = new SqlParamSet();
            var return_int = prms.AddInt("@RETURN", null, System.Data.ParameterDirection.ReturnValue);

            prms.AddInt("@info_id", info.info_id);
            prms.AddInt("@op_seq", sales.op_seq);
            prms.AddDateTime("@party_date", info.party_date);
            prms.AddDateTime("@party_time", info.party_time);
            prms.AddChar("@rest_cd", 5, info.rest_cd);
            prms.AddNVarCharMax("@note", info.note);
            prms.AddVarChar("@last_person", 15, sales.last_person);

            Context.ExecuteStoredProcedure(procName, prms.ToArray(), tr);
            if (isNew) {
                info.info_id = TypeHelper.GetInt(return_int.Value);
                if (info.info_id <= 0) {
                    throw new Exception(string.Format("Could not insert the reception info. ({0})", sales.op_seq));
                }
            }

            RcpInfo newModel = Context.RcpInfos.AsNoTracking().FirstOrDefault(m => m.info_id == info.info_id);

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            return log.InsertLog(sales.last_person, sales.c_num, oldModel, newModel);
        }

        public bool SaveShtInfo(Sales sales, IDbTransaction tr) {
            var info = sales.ShootInfo;
            if (info == null) return false;
            var isNew = (info.info_id == 0);

            ShootInfo oldModel = null;
            if (!isNew) {
                //変更前の状態を取得。(ログに差分を保存するため。)
                oldModel = Context.ShootInfos.AsNoTracking().FirstOrDefault(m => m.info_id == info.info_id);
            }

            var procName = "usp_shoot_info";
            var prms = new SqlParamSet();
            var return_int = prms.AddInt("@RETURN", null, System.Data.ParameterDirection.ReturnValue);

            prms.AddInt("@info_id", info.info_id);
            prms.AddInt("@op_seq", sales.op_seq);
            prms.AddDateTime("@shoot_date", info.shoot_date);
            prms.AddDateTime("@shoot_time", info.shoot_time);
            prms.AddNVarChar("@shoot_place", 100, info.shoot_place);
            prms.AddNVarCharMax("@note", info.note);
            prms.AddVarChar("@last_person", 15, sales.last_person);

            Context.ExecuteStoredProcedure(procName, prms.ToArray(), tr);
            if (isNew) {
                info.info_id = TypeHelper.GetInt(return_int.Value);
                if (info.info_id <= 0) {
                    throw new Exception(string.Format("Could not insert the shooting info. ({0})", sales.op_seq));
                }
            }

            ShootInfo newModel = Context.ShootInfos.AsNoTracking().FirstOrDefault(m => m.info_id == info.info_id);

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            return log.InsertLog(sales.last_person, sales.c_num, oldModel, newModel);
        }

        public bool SaveTrnInfo(Sales sales, IDbTransaction tr) {
            var info = sales.TransInfo;
            if (info == null) return false;
            var isNew = (info.info_id == 0);

            TransInfo oldModel = null;
            if (!isNew) {
                //変更前の状態を取得。(ログに差分を保存するため。)
                oldModel = Context.TransInfos.AsNoTracking().FirstOrDefault(m => m.info_id == info.info_id);
            }

            var procName = "usp_trans_info";
            var prms = new SqlParamSet();
            var return_int = prms.AddInt("@RETURN", null, System.Data.ParameterDirection.ReturnValue);

            prms.AddInt("@info_id", info.info_id);
            prms.AddInt("@op_seq", sales.op_seq);
            prms.AddDateTime("@pickup_date", info.pickup_date);
            prms.AddDateTime("@pickup_time", info.pickup_time);
            prms.AddChar("@pickup_hotel", 3, info.pickup_hotel);
            prms.AddNVarChar("@pickup_place", 100, info.pickup_place);
            prms.AddDateTime("@dropoff_time", info.dropoff_time);
            prms.AddChar("@dropoff_hotel", 3, info.dropoff_hotel);
            prms.AddNVarChar("@dropoff_place", 100, info.dropoff_place);
            prms.AddNVarCharMax("@note", info.note);
            prms.AddVarChar("@last_person", 15, sales.last_person);

            Context.ExecuteStoredProcedure(procName, prms.ToArray(), tr);
            if (isNew) {
                info.info_id = TypeHelper.GetInt(return_int.Value);
                if (info.info_id <= 0) {
                    throw new Exception(string.Format("Could not insert the trans info. ({0})", sales.op_seq));
                }
            }

            TransInfo newModel = Context.TransInfos.AsNoTracking().FirstOrDefault(m => m.info_id == info.info_id);

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            return log.InsertLog(sales.last_person, sales.c_num, oldModel, newModel);
        }

        public bool SaveOtherInfo(Sales sales, IDbTransaction tr, bool isNew, Sales oldModel) {
            if (!isNew && oldModel != null) {
                if (string.Equals(TypeHelper.GetStr(sales.note), TypeHelper.GetStr(oldModel.note))) {
                    //noteが変わっていなければ何もしない。
                    return false;
                }
            }

            var sql = @"UPDATE sales 
                        SET note = @note, last_person = @last_person, update_date = getutcdate() 
                        WHERE (op_seq = @op_seq) ";
            var prms = new SqlParamSet();
            var return_int = prms.AddInt("@RETURN", null, System.Data.ParameterDirection.ReturnValue);
            prms.AddInt("@op_seq", sales.op_seq);
            prms.AddNVarCharMax("@note", sales.note);
            prms.AddVarChar("@last_person", 15, sales.last_person);
            Context.ExecuteSQL(sql, prms.ToArray(), tr);
            return false;           //ここは常にfalseを返す。（呼び出し元でログを保存しているため。)
        }

        /// <summary>
        ///  現在のbook_statusをsalesから取得する。
        /// </summary>
        /// <param name="op_seq"></param>
        /// <returns></returns>
        public string GetBookingStatus(int op_seq) {
            string result = Context.Sales
                                .Where(w => w.op_seq == op_seq)
                                .Select(w => w.book_status)
                                .SingleOrDefault();
            return result;
        }

        public async Task<string> GetBookingStatusAsync(int op_seq) {
            return await Task.Run(() => GetBookingStatus(op_seq));
        }

        public string GetItemTypeByOpSeq(int op_seq) {
            string result = Context.Sales
                                .Where(w => w.op_seq == op_seq)
                                .Select(w => w.item_type)
                                .SingleOrDefault();
            return result;
        }
        public async Task<string> GetItemTypeByOpSeqAsync(int op_seq) {
            return await Task.Run(() => GetItemTypeByOpSeq(op_seq));
        }


        public bool ValidateNewStatus(string c_num, int op_seq, string new_status, bool is_staff) {
            //現在のbook_statusをsalesから取得する。
            var cur_status = GetBookingStatus(op_seq);
            if (string.IsNullOrEmpty(cur_status)) {
                throw new Exception(string.Format("Current plan information cannot be found. (op_seq={0}) ", op_seq));
            }

            //不正なステータス変更は不可。
            List<BookingStatus> avail = BookingStatus.GetAvailableStatusList(cur_status, is_staff);
            if (!avail.Exists(s => s.value == new_status)) {
                throw new Exception(string.Format("New booking status '{0}' is invalid.", new_status));
            }

            ////ステータスをOK以外からOKに変更する場合
            //if (cur_status != BookStatus.OK_VAL && new_status == BookStatus.OK_VAL) {
            //    var item_type = GetItemTypeByOpSeq(op_seq);

            //    //挙式プラン、フォトプランの場合は
            //    if ("PKG".Equals(item_type) || "PHP".Equals(item_type)) {
            //        //同じカスタマー & item_typeで既にOKステータスのものがある場合は不可。
            //        var ok_sales_exists = Context.Sales.Any(s => s.c_num == c_num
            //                                                && s.item_type == item_type
            //                                                && s.book_status == BookStatus.OK_VAL
            //                                                && s.op_seq != op_seq);
            //        if (ok_sales_exists) {
            //            throw new Exception(string.Format("Another plan with OK status already exists. Please cancel it first."));
            //        }
            //    }
            //}

            return true;
        }

        public async Task<IEnumerable<BookingReport.Sales>> GetBookingReportAsync(BookingReport.SearchParam param) {
            return await Task.Run(() => GetBookingReport(param));
        }

        public IEnumerable<BookingReport.Sales> GetBookingReport(BookingReport.SearchParam param) {

            var sql = new StringBuilder(

                "SELECT c.cust_cxl, "
                + "    c.c_num, "
                + "    c.wed_date, "
                + "    c.wed_time, "
                + "    c.g_last, "
                + "    c.g_first, "
                + "    c.b_last, "
                + "    c.b_first, "
                + "    a.vendor_cd, "
                + "    c.church_cd, "
                + "    c.agent_cd, "
                + "    s.jpn_cfm, "
                + "    s.book_status, "
                + "    s.item_cd, "
                + "    s.item_type, "
                + "    c.final_date, "
                + "    s.op_seq "
                + " FROM customer c "
                + "    INNER JOIN sales s ON (c.c_num = s.c_num)"
                + "    LEFT JOIN arrangement a ON (a.op_seq = s.op_seq) ");

            var criteria = new StringBuilder();

            var prms = new List<SqlParameter>();

            Context.AddSqlParam(criteria, "c.wed_date >= {0}", prms, "@wed_date_from", param.date_from);
            Context.AddSqlParam(criteria, "c.wed_date <= {0}", prms, "@wed_date_to", param.date_to);
            //Context.AddSqlParam(criteria, "c.order_date >= {0}", prms, "@order_date_from", bookingDateFrom);
            //Context.AddSqlParam(criteria, "c.order_date <= {0}", prms, "@order_date_to", bookingDateTo);
            Context.AddSqlParam(criteria, "c.agent_cd = {0}", prms, "@agent_cd", param.agent_cd);
            Context.AddSqlParam(criteria, "c.church_cd = {0}", prms, "@church_cd", param.church_cd);
            Context.AddSqlParam(criteria, "c.area_cd = {0}", prms, "@area_cd", param.area_cd);
            Context.AddSqlParam(criteria, "s.item_type = {0}", prms, "@item_type", param.item_type);
            Context.AddSqlParam(criteria, "a.vendor_cd = {0}", prms, "@vendor_cd", param.vendor_cd);
            Context.AddSqlParam(criteria, "s.item_cd = {0}", prms, "@item_cd", param.item_cd);

            Context.AddSqlParamIfTrue(param.not_finalized_only, criteria, "c.final_date IS NULL");
            //Context.AddSqlParamIfTrue(param.isWedCust, criteria, "c.wed_cust = 1");
            //Context.AddSqlParamIfTrue(param.isNotWedCust, criteria, "c.wed_cust = 0");
            Context.AddSqlParamIfTrue(!param.include_cust_cxl, criteria, "c.cust_cxl = 0");
            Context.AddSqlParamIfTrue(!param.include_sales_cxl, criteria, "(s.book_status <> '" + BookingStatus.CXL + "') AND (s.book_status <> '" + BookingStatus.NG + "') AND (a.cxl = 0 OR a.cxl IS NULL)");
            //Context.AddSqlParamIfTrue(param.isNotSalesCfmd, criteria, "s.cfmd = 0 AND (a.cfmd = 0 OR a.cfmd IS NULL)");

            Context.MergeCriteriaToSql(sql, criteria);

            sql.Append(" ORDER BY ");
            switch (param.sort_by) {
                case BookingReport.SortBy.wed_date:
                    sql.Append("c.wed_date, CONVERT(VARCHAR(5), c.wed_time, 8), c.church_cd, c.c_num, s.op_seq ");
                    break;

                case BookingReport.SortBy.vendor_cd:
                    sql.Append("a.vendor_cd, c.wed_date, CONVERT(VARCHAR(5), c.wed_time, 8), c.church_cd, c.c_num, s.op_seq ");
                    break;

                case BookingReport.SortBy.agent_cd:
                    sql.Append("c.agent_cd, c.wed_date, CONVERT(VARCHAR(5), c.wed_time, 8), c.church_cd, c.c_num, s.op_seq ");
                    break;

                default:
                    sql.Append("convert(int, c.c_num) ");
                    break;
            }

            var result = Context.Database.SqlQuery<BookingReport.Sales>(sql.ToString(), prms.ToArray()).ToList();
            ObjectReflectionHelper.TrimStrings(result);
            return result;

        }

    }

}
