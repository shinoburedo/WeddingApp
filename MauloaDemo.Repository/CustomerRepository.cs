using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Threading.Tasks;
using AutoMapper;
using CBAF;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using MauloaDemo.Models.Helpers;
using MauloaDemo.Repository.Exceptions;

namespace MauloaDemo.Repository {


    public class CustomerRepository : BaseRepository<Customer> {

        public class SearchParams {

            public const string SRCH_CHANGE = "change";
            public const string SRCH_ORDER = "order";
            public const string SRCH_CUSTOMER = "customer";
            public const string SRCH_DUPLICATE = "duplicate";

            public int take { get; set; }
            public int skip { get; set; }
            public int page { get; set; }
            public int pageSize { get; set; }

            public string srch_type { get; set; }           //'change', 'order', 'customer'のいずれか。
            public string c_num { get; set; }
            public string cust_name { get; set; }
            public string area_cd { get; set; }
            public string agent_cd { get; set; }
            public string sub_agent_cd { get; set; }
            public string church_cd { get; set; }
            public DateTime date_from { get; set; }
            public DateTime date_to { get; set; }
            public string date_type { get; set; }           //W=wed_date, C=create_date, U=update_date
            public int time_zone { get; set; }
            public string item_type { get; set; }
            public string item_cd { get; set; }
            public string item_name { get; set; }
            public string book_status { get; set; }         //K(=OK), Q(=RQ), N(=NG), X(=CXLRQ), C(=CXL), 1=OK以外, 2=RQ/CXLRQ
            public string action { get; set; }              //I=New, U=Update, D=Delete

            public bool include_archived { get; set; }      //変更ログ検索時にアーカイブ済を含めるかどうか。

            public bool include_dup_check_done { get; set; }      //重複チェック検索時にチェック済を含めるかどうか。

            public bool not_finalized_only { get; set; }      //Customer検索時にfinalizeが済んでいないレコードのみにするかどうか。

            public SearchParams() {
                take = 10;
                skip = 0;
                page = 1;
                pageSize = 10;

                srch_type = SRCH_CHANGE;
                date_from = DateTime.Today.AddMonths(0);
                date_to = DateTime.Today.AddMonths(3);
                date_type = "U";
                time_zone = -10;

                include_archived = false;
            }
        }

        public class CustomerResult {
            public string c_num { get; set; }
            public string message { get; set; }
            public IEnumerable<Customer> dupList { get; set; }
        }


        public CustomerRepository() {
        }
        public CustomerRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            Mapper.CreateMap<Customer, Customer>();
            Mapper.AssertConfigurationIsValid();
        }


        //間違って基底クラスのFindが呼ばれた場合は例外を発生する。
        public new Customer Find(params object[] keyValues) {
            throw new NotImplementedException("Use Find(sring c_num) instead.");
        }

        //間違って基底クラスのFindAsyncが呼ばれた場合は例外を発生する。
        public new async Task<Customer> FindAsync(params object[] keyValues) {
            await Task.Run(() => {
                //Dummy. Nothing to do actually.
            });
            throw new NotImplementedException("Use FindAsync(string c_num) instead.");
        }

        public Customer Find(string c_num) {
            var customer = Context.Customers
                            .AsNoTracking()
                            .SingleOrDefault(c => c.c_num == c_num);
            if (customer == null) return null;

            //排他制御用のTime Stampを専用のプロパティに保持。
            customer.update_date_stamp = customer.update_date;

            ApplyMappings(customer);

            if (!string.IsNullOrEmpty(customer.church_cd)) {
                var item_type = GetWedItemType(c_num);
                if ("PHP".Equals(item_type)) {
                    var SunsetBlockTime = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["SunsetBlockTime"]);
                    if (string.Equals(customer.wed_time_s, SunsetBlockTime)) {
                        customer.is_sunset = true;
                    }
                }
            }

            return customer;
        }

        public async Task<Customer> FindAsync(string c_num) {
            return await Task.Run(() => Find(c_num));
        }


        public GridResult<CustomerListItem> Search(SearchParams param, LoginUser loginUser) {

            if (param.srch_type == SearchParams.SRCH_DUPLICATE && !loginUser.IsStaff()) {
                throw new InvalidOperationException("Not authorized.");
            }

            param.c_num = TypeHelper.GetStrTrim(param.c_num);
            param.cust_name = TypeHelper.GetStrTrim(param.cust_name);
            param.agent_cd = TypeHelper.GetStrTrim(param.agent_cd);
            param.sub_agent_cd = TypeHelper.GetStrTrim(param.sub_agent_cd);
            param.church_cd = TypeHelper.GetStrTrim(param.church_cd);

            var sql = @"EXEC ";
            switch (param.srch_type) { 
                case SearchParams.SRCH_CHANGE:
                    sql += " usp_search_log_change @skip, @take, @total output, " 
                        + " @c_num, @cust_name, @area_cd, @agent_cd, @sub_agent_cd, @church_cd, " 
                        + " @item_type, @item_cd, @item_name, @book_status, "
                        + " @include_archived, @viewer_login_id, @action ";
                    break;
                case SearchParams.SRCH_ORDER:
                    sql += " usp_search_order @skip, @take, @total output, " 
                        + " @c_num, @cust_name, @area_cd, @agent_cd, @sub_agent_cd, @church_cd, " 
                        + " @date_from, @date_to, @date_type, @time_zone, @item_type, @item_cd, @item_name, " 
                        + " @book_status";
                    break;
                case SearchParams.SRCH_DUPLICATE:
                    sql += " usp_dup_check @skip, @take, @total output, @date_from, @include_dup_check_done ";
                    break;
                default:
                    sql += " usp_search_customer @skip, @take, @total output, "
                        + " @c_num, @cust_name, @area_cd, @agent_cd, @sub_agent_cd, @church_cd, " 
                        + " @date_from, @date_to, @date_type, @time_zone, @item_type, @item_cd, @item_name, " 
                        + " @not_finalized_only";
                    break;
            }

            var prms = new SqlParamSet();
            prms.AddInt("@skip", param.skip);
            prms.AddInt("@take", param.take);
            prms.AddInt("@total", 0, ParameterDirection.InputOutput);

            if (param.srch_type == SearchParams.SRCH_DUPLICATE) {
                prms.AddDateTime("@date_from", DateTime.UtcNow);
                prms.AddBit("@include_dup_check_done", param.include_dup_check_done);

            } else {
                prms.AddChar("@c_num", 7, param.c_num);
                prms.AddNVarChar("@cust_name", 50, param.cust_name);
                prms.AddChar("@area_cd", 3, param.area_cd);
                prms.AddChar("@agent_cd", 6, param.agent_cd);
                prms.AddChar("@sub_agent_cd", 6, param.sub_agent_cd);
                prms.AddChar("@church_cd", 5, param.church_cd);

                if (param.srch_type == SearchParams.SRCH_ORDER || param.srch_type == SearchParams.SRCH_CUSTOMER) {
                    prms.AddDateTime("@date_from", param.date_from.Date);
                    prms.AddDateTime("@date_to", param.date_to.Date);
                    prms.AddChar("@date_type", 1, param.date_type);
                    prms.AddInt("@time_zone", param.time_zone);
                }

                prms.AddChar("@item_type", 3, param.item_type);
                prms.AddVarChar("@item_cd", 15, param.item_cd);
                prms.AddNVarChar("@item_name", 200, param.item_name);

                if (param.srch_type == SearchParams.SRCH_CHANGE || param.srch_type == SearchParams.SRCH_ORDER) {
                    prms.AddChar("@book_status", 1, param.book_status);
                }

                if (param.srch_type == SearchParams.SRCH_CHANGE) {
                    prms.AddBit("@include_archived", param.include_archived);
                    prms.AddVarChar("@viewer_login_id", 15, loginUser.login_id);
                    prms.AddChar("@action", 1, param.action);
                }

                if (param.srch_type == SearchParams.SRCH_CUSTOMER) {
                    prms.AddBit("@not_finalized_only", param.not_finalized_only);
                }
            }

            var list = Context.Database.SqlQuery<CustomerListItem>(sql, prms.ToArray())
                                       .ToList();
            int total = TypeHelper.GetInt(prms.GetValue("@total"));
            ObjectReflectionHelper.TrimStrings(list);

            if (param.srch_type == SearchParams.SRCH_CHANGE) {
                //staff_noteのみの変更などの場合はエージェントには見せないために結果から削除する。
                var count_before = list.Count;
                if (loginUser.IsStaff()) {
                    list = list.Where(item => !string.IsNullOrWhiteSpace(item.ChangesStrForStaff)).ToList();
                } else {
                    list = list.Where(item => !string.IsNullOrWhiteSpace(item.ChangesStrForAgent)).ToList();
                }
                var count_deleted = count_before - list.Count;
                total -= count_deleted;         //削除対象行があった場合は合計数を調整。
            }

            return new GridResult<CustomerListItem>() { 
                data = list, 
                total = total 
            };
        }

        public async Task<bool> Exists(string c_num) {
            bool result = await Task.Run( () => Context.Customers.Count(e => e.c_num == c_num) > 0 );
            return result;
        }


        public async Task<CustomerResult> SaveCustomer(Customer cust, LoginUser user, IDbTransaction tr) {
            var result = new CustomerResult();

            var isNew = string.IsNullOrEmpty(cust.c_num);
            if (cust.wed_date.HasValue) {
                //念のため挙式日から時刻の情報を除去。
                cust.wed_date = cust.wed_date.Value.Date;

                if (cust.wed_time.HasValue) {
                    //wed_timeの日付部分を挙式日に合わせる。
                    var new_wed_time = TypeHelper.GetDateTime(cust.wed_date.Value.ToString("yyyy/MM/dd ") +  cust.wed_time.Value.ToString("HH:mm"));
                    cust.wed_time = new_wed_time;
                }
            }

            if (string.IsNullOrEmpty(cust.agent_cd) && !string.IsNullOrEmpty(cust.sub_agent_cd)) { 
                var subAgentRepo = new SubAgentRepository(this);
                cust.agent_cd = subAgentRepo.GetAgentCd(cust.sub_agent_cd);
            }

            try {
                HankakuHelper.Apply(cust);
                UpperCaseHelper.Apply(cust);
                cust.ValidateSave();

                //氏名の重複チェックを実行。
                if (isNew && !cust.noDupCheck) {
                    var dup_list = GetDuplicatedCustomers(cust.g_last, cust.g_first, cust.b_last, cust.b_first, cust.agent_cd);
                    if (dup_list.Count() > 0) {
                        throw new CustNameDuplicateException(dup_list);
                    }
                }

                await Task.Run(() => {

                    Customer oldModel = null;
                    if (!isNew) {
                        //変更前の状態を取得。(ログに差分を保存するため。)
                        oldModel = this.Find(cust.c_num);

                        //楽観的排他制御 (JSを経由するとミリ秒の情報が欠落するため秒単位で比較する。)
                        if (TypeHelper.CompareDateBySecond(oldModel.update_date_stamp, cust.update_date_stamp) != 0) {
                            throw new OptimisticLockException("Customer", user.IsStaff() ? oldModel.last_person : "");
                        }
                    }

                    var procName = "usp_save_customer";
                    var prms = new SqlParamSet();
                    var return_int = prms.AddInt("@RETURN", null, System.Data.ParameterDirection.ReturnValue);
                    prms.AddChar("@c_num", 7, cust.c_num);
                    prms.AddChar("@sub_agent_cd", 6, cust.sub_agent_cd);
                    prms.AddVarChar("@g_last", 20, cust.g_last);
                    prms.AddVarChar("@g_first", 20, cust.g_first);
                    prms.AddVarChar("@b_last", 20, cust.b_last);
                    prms.AddVarChar("@b_first", 20, cust.b_first);
                    prms.AddNVarChar("@g_last_kana", 20, cust.g_last_kana);
                    prms.AddNVarChar("@g_first_kana", 20, cust.g_first_kana);
                    prms.AddNVarChar("@b_last_kana", 20, cust.b_last_kana);
                    prms.AddNVarChar("@b_first_kana", 20, cust.b_first_kana);
                    prms.AddNVarChar("@g_last_kanji", 20, cust.g_last_kanji);
                    prms.AddNVarChar("@g_first_kanji", 20, cust.g_first_kanji);
                    prms.AddNVarChar("@b_last_kanji", 20, cust.b_last_kanji);
                    prms.AddNVarChar("@b_first_kanji", 20, cust.b_first_kanji);

                    prms.AddChar("@area_cd", 3, cust.area_cd);
                    prms.AddNVarChar("@tour_cd", 20, cust.tour_cd);
                    prms.AddChar("@church_cd", 5, cust.church_cd);
                    prms.AddDateTime("@wed_date", cust.wed_date);
                    prms.AddDateTime("@wed_time", cust.wed_time);
                    prms.AddDateTime("@htl_pick", cust.htl_pick);

                    prms.AddDateTime("@bf_date", cust.bf_date);
                    prms.AddDateTime("@bf_time", cust.bf_time);
                    prms.AddNVarChar("@bf_place", 100, cust.bf_place);
                    prms.AddDateTime("@ft_date", cust.ft_date);
                    prms.AddDateTime("@ft_time", cust.ft_time);
                    prms.AddNVarChar("@ft_place", 100, cust.ft_place);

                    prms.AddVarChar("@in_flight", 10, cust.in_flight);
                    prms.AddChar("@in_dep", 3, cust.in_dep);
                    prms.AddDateTime("@in_dep_date", cust.in_dep_date);
                    prms.AddDateTime("@in_dep_time", cust.in_dep_time);
                    prms.AddChar("@in_arr", 3, cust.in_arr);
                    prms.AddDateTime("@in_arr_date", cust.in_arr_date);
                    prms.AddDateTime("@in_arr_time", cust.in_arr_time);

                    prms.AddVarChar("@out_flight", 10, cust.out_flight);
                    prms.AddChar("@out_dep", 3, cust.out_dep);
                    prms.AddDateTime("@out_dep_date", cust.out_dep_date);
                    prms.AddDateTime("@out_dep_time", cust.out_dep_time);
                    prms.AddChar("@out_arr", 3, cust.out_arr);
                    prms.AddDateTime("@out_arr_date", cust.out_arr_date);
                    prms.AddDateTime("@out_arr_time", cust.out_arr_time);

                    prms.AddChar("@hotel_cd", 5, cust.hotel_cd);
                    prms.AddVarChar("@room_number", 10, cust.room_number);
                    prms.AddDateTime("@checkin_date", cust.checkin_date);
                    prms.AddDateTime("@checkout_date", cust.checkout_date);

                    prms.AddNVarCharMax("@note", cust.note);
                    prms.AddNVarCharMax("@staff_note", cust.staff_note);
                    prms.AddSmallInt("@attend_count", cust.attend_count);
                    prms.AddNVarChar("@attend_name", 100, cust.attend_name);
                    prms.AddNVarCharMax("@attend_memo", cust.attend_memo);

                    prms.AddVarChar("@last_person", 15, cust.last_person);

                    var need_commit = false;
                    if (tr == null) {
                        tr = Context.BeginTrans();
                        need_commit = true;
                    }
                    try {
                        Context.ExecuteStoredProcedure(procName, prms.ToArray(), tr);

                        var int_c_num = TypeHelper.GetInt(return_int.Value);
                        if (int_c_num > 0) {
                            result.c_num = TypeHelper.GetStr(int_c_num);
                            cust.c_num = result.c_num;
                            result.message = "ok";

                            //保存後の状態を取得。
                            var saved = this.Find(result.c_num);

                            //変更ログを保存。
                            var log = new LogChangeRepository(this);
                            log.InsertLog(cust.last_person, result.c_num, oldModel, saved);

                            if (need_commit) tr.Commit();
                        } else {
                            throw new Exception(string.Format("Could not save customer. C#:'{0}', Rtn:'{1}'", cust.c_num, int_c_num));
                        }
                    } catch (Exception ex) {
                        if (need_commit) tr.Rollback();
                        throw ex;
                    }
                }); 

            } catch (CustNameDuplicateException) {
                //氏名の重複が見つかった場合はそのまま同じ例外を投げる。
                throw;      

            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                result.message = ex.Message;
            }

            return result;
        }

        public string GetCustomerAgentCd(string c_num) {
            string agent_cd = Context.Customers
                                    .Where(e => e.c_num == c_num)
                                    .Select(c => c.agent_cd)
                                    .SingleOrDefault();
            agent_cd = TypeHelper.GetStrTrim(agent_cd);
            return agent_cd;
        }
        public async Task<string> GetCustomerAgentCdAsync(string c_num) {
            return await Task.Run(() => GetCustomerAgentCd(c_num));
        }

        public string GetCustomerSubAgentCd(string c_num) {
            string sub_agent_cd = Context.Customers
                                        .Where(e => e.c_num == c_num)
                                        .Select(c => c.sub_agent_cd)
                                        .SingleOrDefault();
            sub_agent_cd = TypeHelper.GetStrTrim(sub_agent_cd);
            return sub_agent_cd;
        }
        public async Task<string> GetCustomerSubAgentCdAsync(string c_num) {
            return await Task.Run(() => GetCustomerSubAgentCd(c_num));
        }

        public DateTime? GetCustomerWedDate(string c_num) {
            DateTime? result = Context.Customers
                                    .Where(e => e.c_num == c_num)
                                    .Select(c => c.wed_date)
                                    .SingleOrDefault();
            return result;
        }
        public async Task<DateTime?> GetCustomerWedDateAsync(string c_num) {
            return await Task.Run(() => GetCustomerWedDate(c_num));
        }

        public string GetPlanKind(string c_num) {
            var hasWedPlans = Context.Sales
                .Any(s => s.c_num == c_num && s.book_status == BookingStatus.OK && s.item_type == "PKG");
            if (hasWedPlans) return "W";            //Wedding Planが確定していれば必ず 'W'を返す。

            var hasPhotoPlans = Context.Sales
                .Any(s => s.c_num == c_num && s.book_status == BookingStatus.OK && s.item_type == "PHP");
            if (hasPhotoPlans) return "P";          //Wedding Planが無くPhoto Planが確定していれば'P'を返す。

            return string.Empty;                    //Wedding PlanもPhoto Planも無ければ空文字列を返す。
        }
        public async Task<string> GetPlanKindAsync(string c_num) {
            return await Task.Run(() => GetPlanKind(c_num));
        }

        public string GetWedItemType(string c_num) {
            var sql = @"SELECT TOP 1 s.item_type
                        FROM Sales s
	                        INNER JOIN customer c ON (s.c_num = c.c_num)
	                        INNER JOIN wed_info w ON (s.op_seq = w.op_seq)
	                        INNER JOIN item i ON (s.item_cd = i.item_cd)
                        WHERE (s.c_num = @c_num)
	                        AND (c.church_cd = i.church_cd)
	                        AND (s.book_status = '" + BookingStatus.OK + @"')
                        ORDER BY w.update_date DESC";
            var prms = new SqlParamSet();
            prms.AddChar("@c_num", 7, c_num);
            var item_type = Context.Database.SqlQuery<string>(sql, prms.ToArray())
                                    .SingleOrDefault();
            return item_type;              
        }
        public async Task<string> GetWedItemTypeAsync(string c_num) {
            return await Task.Run(() => GetWedItemType(c_num) );
        }

        public async Task<FinalInfoSheetInfo> GetFolderSheetInfo(string c_num, bool english, LoginUser user) {
            //ログインユーザーのカスタマーへの権限をチェック。
            var loginUserRepo = new LoginUserRepository(this);
            if (!loginUserRepo.CanViewCustomer(user, c_num)) {
                throw new InvalidOperationException("Not authorized.");
            }
            FinalInfoSheetInfo finalInfoSheetInfo = new FinalInfoSheetInfo();

            //var list = await GetFolderSheetItems(c_num, english, user);
            //finalInfoSheetInfo.list = list.ToList();

            finalInfoSheetInfo.Customer = this.Find(c_num);
            if (finalInfoSheetInfo.Customer == null) return null;

            var churchRepository = new ChurchRepository(this);
            var church = churchRepository.Find(finalInfoSheetInfo.Customer.church_cd);
            finalInfoSheetInfo.church_name = church == null ? "" : (english ? church.church_name : church.church_name_jpn);

            var hotelRepository = new HotelRepository(this);
            var hotel = hotelRepository.Find(finalInfoSheetInfo.Customer.hotel_cd);
            finalInfoSheetInfo.Customer.hotel_name = hotel == null ? "" : (english ? hotel.hotel_name : hotel.hotel_name_jpn);

            var agentRepository = new AgentRepository(this);
            var agent = agentRepository.Find(finalInfoSheetInfo.Customer.agent_cd);
            finalInfoSheetInfo.agent_name = agent == null ? "" : (english ? agent.agent_name : agent.agent_name_jpn);

            var addrInfoRepository = new AddressInfoRepository(this);
            var address_list = await addrInfoRepository.GetList(c_num);
            finalInfoSheetInfo.AddressInfos = address_list.ToList();

            var cosInfoRepository = new CosInfoRepository(this);
            var cos_list = await cosInfoRepository.GetList(c_num);
            finalInfoSheetInfo.CosInfos = cos_list.ToList();

            var salesRepository = new SalesRepository(this);
            var option_list = await salesRepository.GetOptions(c_num, "");
            var options = option_list.Where(i => i.book_status == BookingStatus.OK).ToList();
            //if (option_list != null) {
            //    foreach (var option in option_list) {
            //        var arrangementRepository = new ArrangementRepository(this);
            //        var vendor_list = arrangementRepository.GetVendorByOpSeq(option.op_seq).ToList();
            //        option.Vendors = vendor_list;
            //    }
            //}
            finalInfoSheetInfo.list = options;

            var wedInfoRepository = new WedInfoRepository(this);
            var wed_list = await wedInfoRepository.GetList(c_num, "", user);
            wed_list = wed_list.Where(i => i.book_status == BookingStatus.OK);
            if (wed_list != null) {
                foreach (var wedInfo in wed_list) {
                    var items = salesRepository.GetChildren(wedInfo.op_seq, wedInfo.item_cd);
                    if (wedInfo.item_type == "PKG") {
                        finalInfoSheetInfo.WedPlanItems = items.Where(i => i.book_status == BookingStatus.OK).ToList(); 
                        finalInfoSheetInfo.WedPlan = wedInfo;
                    } else {
                        finalInfoSheetInfo.PhotoPlanItems = items.Where(i => i.book_status == BookingStatus.OK).ToList(); 
                        finalInfoSheetInfo.PhotoPlan = wedInfo;
                    }
                }
            }

            var schedule_repository = new SchedulePhraseRepository(this);
            var schedule_info = await Task<ScheduleSheetInfo>.Run(() => schedule_repository.GetScheduleSheetInfo(c_num, english, user));
            finalInfoSheetInfo.Schedule = schedule_info;            

            return finalInfoSheetInfo;
        }

        public async Task<IEnumerable<FinalInfoSheetItem>> GetFolderSheetItems(string c_num, bool english, LoginUser user) {

            //Agentユーザーの場合は自身のAgentCd、PROMユーザーの場合はnullを渡す。
            var agent_cd = user.IsAgent() ? user.agent_cd : "";

            var sql = @"EXEC usp_rpt_finalinfo_sheet @c_num, @english, @agent_cd";
            var prms = new SqlParamSet();
            prms.AddChar("@c_num", 7, c_num);
            prms.AddBit("@english", english);
            prms.AddChar("@agent_cd", 6, agent_cd);

            var list = await Task.Run(() => Context.Database.SqlQuery<FinalInfoSheetItem>(sql, prms.ToArray())
                        .ToList());
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<FinalInfoSheetItem>> GetWedInfoItems(string c_num, bool english, LoginUser user) {
            //ログインユーザーのカスタマーへの権限をチェック。
            var loginUserRepo = new LoginUserRepository(this);
            if (!loginUserRepo.CanViewCustomer(user, c_num)) {
                throw new InvalidOperationException("Not authorized.");
            }

            //Agentユーザーの場合は自身のAgentCd、PROMユーザーの場合はカスタマーのAgentCdを使う。
            var agent_cd = user.IsAgent() ? user.agent_cd : this.GetCustomerAgentCd(c_num);

            var sql = @"EXEC usp_rpt_finalinfo_sheet @c_num, @english, @agent_cd";
            var prms = new SqlParamSet();
            prms.AddChar("@c_num", 7, c_num);
            prms.AddBit("@english", english);
            prms.AddChar("@agent_cd", 6, agent_cd);

            var list = await Task.Run(() => Context.Database.SqlQuery<FinalInfoSheetItem>(sql, prms.ToArray())
                        .ToList());
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public void UpdateFinalInfo(string c_num, string login_user, bool isFinalize) {

            var customer = this.Find(c_num);
            if (customer == null) {
                throw new Exception(string.Format("Customer #{0} not found.", c_num));
            }
            var oldModel = Mapper.Map<Customer>(customer);

            var tr = Context.BeginTrans();
            try {
                var sql = "UPDATE [customer] "
                    + " SET [last_person] = @last_person,"
                    + "     [update_date] = getutcdate(), ";

                var prms = new SqlParamSet();
                if (isFinalize) {
                    sql += " [final_date] = getutcdate(), "
                        + "  [final_by] = @final_by ";
                    prms.AddVarChar("@final_by", 15, login_user);

                    customer.final_by = login_user;
                } else {
                    sql += " [final_date] = NULL, "
                        + "  [final_by] = NULL ";

                    customer.final_by = "";
                }
                sql += " WHERE ([c_num] = @c_num) ";

                prms.AddVarChar("@last_person", 15, login_user);
                prms.AddChar("@c_num", 7, c_num);

                this.Context.ExecuteSQL(sql, prms.ToArray(), tr);

                //変更ログを保存。
                var log = new LogChangeRepository(this);
                log.InsertLog(login_user, c_num, oldModel, customer);

                tr.Commit();
            }
            catch (Exception ex) {
                tr.Rollback();
                throw ex;
            }
        }

        public void UpdateDupChkDone(string c_num, bool dup_check_done, string login_user, IDbTransaction tr) {
                var sql = "UPDATE [customer] "
                    + " SET [last_person] = @last_person,"
                    + "     [update_date] = getutcdate(), "
                    + "     [dup_check_done] = @dup_check_done "
                    + " WHERE ([c_num] = @c_num) ";
                var prms = new SqlParamSet();

                prms.AddVarChar("@last_person", 15, login_user);
                prms.AddBit("@dup_check_done", dup_check_done);
                prms.AddChar("@c_num", 7, c_num);

                this.Context.ExecuteSQL(sql, prms.ToArray(), tr);
        }

        public void SaveDupChkDone(Customer.ChangedRows rows, string login_user) {

            using (var tr = this.Context.BeginTrans()) {
                try {
                    for (int i = 0; i < rows.c_num.Count; i++) {
                        this.UpdateDupChkDone(rows.c_num.ElementAt(i), rows.dup_check_done.ElementAt(i), login_user, tr);
                    }
                    tr.Commit();
                } catch (Exception) {
                    tr.Rollback();
                    throw;
                }
            }
        }

        public IEnumerable<Customer> GetDuplicatedCustomers(string g_last, string g_first, string b_last, string b_first, string agent_cd) {
            var today = RegionConfig.GetRegionToday(this.RegionCd);

            g_last = string.IsNullOrWhiteSpace(g_last) ? null : g_last.ToUpper().Trim();
            g_first = string.IsNullOrWhiteSpace(g_first) ? null : g_first.ToUpper().Trim();
            b_last = string.IsNullOrWhiteSpace(b_last) ? null : b_last.ToUpper().Trim();
            b_first = string.IsNullOrWhiteSpace(b_first) ? null : b_first.ToUpper().Trim();

            var list = this.Context.Customers
                            .AsNoTracking()
                            .Where(c => c.g_last == g_last 
                                    && (c.g_first == g_first || (c.g_first == null && g_first == null))
                                    && (c.b_last == b_last || (c.b_last == null && b_last == null))
                                    && (c.b_first == b_first || (c.b_first == null && b_first == null))
                                    && c.agent_cd == agent_cd
                                    && (c.wed_date == null || c.wed_date >= today))
                            .OrderBy(c => c.wed_date)
                            .ThenBy(c => c.c_num)
                            .Take(100)
                            .ToList();

            var SunsetBlockTime = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["SunsetBlockTime"]);
            list.ForEach(c => {
                if (!string.IsNullOrEmpty(c.church_cd)) {
                    var item_type = GetWedItemType(c.c_num);
                    if ("PHP".Equals(item_type)) {
                        if (string.Equals(c.wed_time_s, SunsetBlockTime)) {
                            c.is_sunset = true;
                        }
                    }
                }
            });
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public IEnumerable<DailyMovement> GetDailyMovementReportList(
            DateTime? wed_date,
            string church_cd,
            string agent_cd) {

                var sql = @" EXEC usp_rpt_dailymovement
                              @wed_date,
                              @church_cd,
                              @agent_cd ";

            //church_cd = string.IsNullOrEmpty(church_cd) ? "%" : church_cd;
            //agent_cd = string.IsNullOrEmpty(agent_cd) ? "%" : agent_cd;

            var prms = new SqlParamSet();
            prms.AddVarChar("@wed_date", 8, TypeHelper.DateStrMDY(wed_date));
            prms.AddVarChar("@church_cd", 10, church_cd);
            prms.AddChar("@agent_cd", 6, agent_cd);

            var list = Context.Database.SqlQuery<DailyMovement>(sql, prms.ToArray())
                        .ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }
    
    }
}
