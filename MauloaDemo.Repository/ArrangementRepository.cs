using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CBAF;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;

namespace MauloaDemo.Repository {

    public class ArrangementRepository : BaseRepository<Arrangement> {

        public class SearchParams {
            public int op_seq {get; set;}
            public string c_num { get; set; }
            public string vendor_cd { get; set; }
        }

        public class ArrangementResult {
            public int arrangement_id { get; set; }
            public string status { get; set; }
            public string message { get; set; }
        }


        public ArrangementRepository() {
        }

        public ArrangementRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);
        }

        //間違って基底クラスのFindが呼ばれた場合は例外を発生する。
        public new Arrangement Find(params object[] keyValues) {
            throw new NotImplementedException("Use Find(int id) instead.");
        }

        //間違って基底クラスのFindAsyncが呼ばれた場合は例外を発生する。
        public new async Task<Arrangement> FindAsync(params object[] keyValues) {
            await Task.Run(() => {
                //Dummy. Nothing to do actually.
            });
            throw new NotImplementedException("Use FindAsync(int id) instead.");
        }

        /// <summary>
        /// arramgent_idからArrangementオブジェクトを取得する。
        /// </summary>
        /// <param name="op_seq"></param>
        /// <returns></returns>
        public Arrangement Find(int arrangement_id) {

            var arr = Context.Arrangements
                            .Where(a => a.arrangement_id == arrangement_id)
                            .SingleOrDefault();
            ApplyMappings(arr);     //文字列のTrim処理など。
            return arr;
        }

        public async Task<Arrangement> FindAsync(int arrangement_id) {
            var arr = await Task.Run(() => this.Find(arrangement_id));
            return arr;
        }

        public IEnumerable<Arrangement> GetArrangementsByOpSeq(int op_seq) {
            var sql = @"SELECT * FROM [arrangement] WHERE (op_seq = @op_seq) ORDER BY arrangement_id ";
            var prms = new SqlParamSet();
            prms.AddInt("@op_seq", op_seq);

            var list = Context.Database.SqlQuery<Arrangement>(sql, prms.ToArray()).ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<Arrangement>> GetArrangementsByOpSeqAsync(int op_seq) {
            var list = await Task.Run(() => GetArrangementsByOpSeq(op_seq));
            return list;
        }

        public async Task<IEnumerable<VendorConfirmation>> GetArragementsForVendorConfirmationAsync(DateTime? date_from, DateTime? date_to)
        {
            var list = await Task.Run(() => GetArragementsForVendorConfirmation(date_from, date_to));
            return list;
        }

        public IEnumerable<Vendor> GetVendorByItemTypeAndCNum(string c_num, string item_type) {
            var sql = @"SELECT DISTINCT v.* FROM [arrangement] a, [sales] s, [vendor] v 
                        WHERE (a.op_seq = s.op_seq) 
                            AND (a.vendor_cd = v.vendor_cd)  
                            AND (s.book_status = 'K')  
                            AND (s.item_type = @item_type)  
                            AND (s.c_num = @c_num) 
                        ORDER BY v.vendor_cd ";
            var prms = new SqlParamSet();
            prms.AddChar("@item_type", 3, item_type);
            prms.AddChar("@c_num", 7, c_num);

            var list = Context.Database.SqlQuery<Vendor>(sql, prms.ToArray()).ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public IEnumerable<VendorConfirmation> GetArragementsForVendorConfirmation(DateTime? date_from, DateTime? date_to)
        {
            var sql = @"SELECT vendor_cd, COUNT(s.op_seq) AS count FROM [arrangement] a, [sales] s 
                        WHERE (a.op_seq = s.op_seq) 
                            AND (s.book_status = 'K')  
                            AND (s.sales_post_date >= @date_from)  
                            AND (s.sales_post_date <= @date_to) 
                        GROUP BY vendor_cd
                        ORDER BY vendor_cd ";
            var prms = new SqlParamSet();
            prms.AddDateTime("@date_from", date_from);
            prms.AddDateTime("@date_to", date_to);

            var list = Context.Database.SqlQuery<VendorConfirmation>(sql, prms.ToArray()).ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public ArrangementResult Save(Arrangement arrangement, IDbTransaction tr)
        {
            var result = new ArrangementResult {
                arrangement_id = arrangement.arrangement_id,
                status = String.Empty,
                message = String.Empty
            };

            var isNew = (arrangement.arrangement_id == 0);
            try {
                if (string.IsNullOrEmpty(arrangement.vendor_cd)) {
                    throw new Exception("Please select a vendor for the Arrangement.");
                }
                if (string.IsNullOrEmpty(arrangement.c_num)) {
                    throw new Exception("Customer number is required for the Arrangement.");
                }
                if (arrangement.op_seq == 0) {
                    throw new Exception("Invalid op_seq (0).");
                }
                if (string.IsNullOrEmpty(arrangement.last_person)) {
                    throw new Exception("User id is required for the Arrangement.");
                }

                if (arrangement.cfmd_date.HasValue) {
                    arrangement.cfmd_date = arrangement.cfmd_date.Value.Date;   //時刻を除去。
                }
                if (arrangement.cxl_date.HasValue) {
                    arrangement.cxl_date = arrangement.cxl_date.Value.Date;   //時刻を除去。
                }

                var procName = "";
                var prms = new SqlParamSet();
                //var return_int = prms.AddInt("@RETURN", null, System.Data.ParameterDirection.ReturnValue);
                SqlParameter out_newid = null;

                if (isNew) {
                    procName = @"INSERT INTO arrangement (
					                op_seq,
					                c_num,
					                vendor_cd,
                                    cfmd,
                                    cfmd_by,
                                    cfmd_date,
                                    cxl,
                                    cxl_vend_by,
                                    cxl_date,
                                    note,
					                quantity,
					                cost,
					                jnl_started,
					                create_by,
					                create_date,
					                last_person,
					                update_date
				                ) VALUES (
					                @op_seq,
					                @c_num,
					                @vendor_cd,
                                    @cfmd,
                                    @cfmd_by,
                                    @cfmd_date,
                                    @cxl,
                                    @cxl_vend_by,
                                    @cxl_date,
                                    @note,
					                1,
					                @cost,
					                @jnl_started,
					                @last_person,
					                getutcdate(),
					                @last_person,
					                getutcdate()
				                );
                                SELECT @newid = @@IDENTITY;";
                    prms.AddInt("@op_seq", arrangement.op_seq);
                    prms.AddChar("@c_num", 7, arrangement.c_num);
                    prms.AddChar("@vendor_cd", 8, arrangement.vendor_cd);
                    prms.AddBit("@cfmd", arrangement.cfmd);
                    prms.AddNVarChar("@cfmd_by", 100, arrangement.cfmd_by);
                    prms.AddDateTime("@cfmd_date", arrangement.cfmd_date);
                    prms.AddBit("@cxl", arrangement.cxl);
                    prms.AddNVarChar("@cxl_vend_by", 100, arrangement.cxl_vend_by);
                    prms.AddDateTime("@cxl_date", arrangement.cxl_date);
                    prms.AddNVarCharMax("@note", arrangement.note);
                    prms.AddDecimal("@cost", arrangement.cost, 10, 2);
                    prms.AddBit("@jnl_started", arrangement.jnl_started);
                    prms.AddVarChar("@last_person", 15, arrangement.last_person);
                    out_newid = prms.AddInt("@newid", 0, ParameterDirection.Output);
                } else {
                    procName = "usp_upd_arrangement";
                    prms.AddInt("@arrangement_id", arrangement.arrangement_id);
                    prms.AddChar("@vendor_cd", 8, arrangement.vendor_cd);
                    prms.AddBit("@cfmd", arrangement.cfmd);
                    prms.AddNVarChar("@cfmd_by", 100, arrangement.cfmd_by);
                    prms.AddDateTime("@cfmd_date", arrangement.cfmd_date);
                    prms.AddBit("@cxl", arrangement.cxl);
                    prms.AddNVarChar("@cxl_vend_by", 100, arrangement.cxl_vend_by);
                    prms.AddDateTime("@cxl_date", arrangement.cxl_date);
                    prms.AddNVarCharMax("@note", arrangement.note);
                    prms.AddVarChar("@last_person", 15, arrangement.last_person);
                }

                var need_commit = false;
                if (tr == null) {
                    tr = Context.BeginTrans();
                    need_commit = true;
                }
                try {
                    if (isNew) {
                        Context.ExecuteSQL(procName, prms.ToArray(), tr);

                        arrangement.arrangement_id = TypeHelper.GetInt(out_newid.Value);
                        result.arrangement_id = arrangement.arrangement_id;
                        if (arrangement.arrangement_id <= 0) {
                            throw new Exception("Could not insert the new Arrangement.");
                        }
                        result.status = "ok";
                    } else {
                        Context.ExecuteStoredProcedure(procName, prms.ToArray(), tr);

                        result.status = "ok";
                    }
                    if (need_commit) tr.Commit();

                } catch (Exception ex) {
                    if (need_commit) tr.Rollback();
                    throw ex;
                }
            } catch (Exception ex) {
                result.status = "error";
                result.message = ex.Message;
            }
            return result;
        }

        public async Task<VendorOrderSheetInfo> GetVendorOrderSheetInfo(string c_num, int info_id, LoginUser user) {
            //ログインユーザーのカスタマーへの権限をチェック。
            var loginUserRepo = new LoginUserRepository(this);
            if (!loginUserRepo.CanViewCustomer(user, c_num)) {
                throw new InvalidOperationException("Not authorized.");
            }

            var info = new VendorOrderSheetInfo();

            await Task.Run(() => info.Arrangement = this.Find(info_id));
            if (info.Arrangement == null) return null;
            if (!string.Equals(info.Arrangement.c_num, c_num)) return null;

            var customerRepository = new CustomerRepository(this);
            info.Customer = customerRepository.Find(info.Arrangement.c_num);
            if (info.Customer == null) return null;
            info.Customer.hotel_name = Context.Hotels.Where(i => i.hotel_cd == info.Customer.hotel_cd).Select(m => m.hotel_name).SingleOrDefault();

            var vendorRepository = new VendorRepository(this);
            info.Vendor= vendorRepository.Find(info.Arrangement.vendor_cd);

            var salesRepository = new SalesRepository(this);
            info.Sales = salesRepository.Find(info.Arrangement.op_seq, user);

            var itemRepository = new ItemRepository(this);
            info.Item = itemRepository.Find(info.Sales.item_cd);

            var churchRepository = new ChurchRepository(this);
            info.Church = churchRepository.Find(info.Customer.church_cd);

            return info;
        }


        public List<VendorConfirmationReport> GetVendorConfirmationInfo(string vendor_cd, DateTime date_from, DateTime date_to)
        {
            var sql = @"EXEC usp_rpt_vendor_confirmation @vendor_cd, @date_from, @date_to";
            var prms = new SqlParamSet();
            prms.AddChar("@vendor_cd", 8, vendor_cd);
            prms.AddDateTime("@date_from", date_from);
            prms.AddDateTime("@date_to", date_to);
            var list = Context.Database.SqlQuery<VendorConfirmationReport>(sql, prms.ToArray()).ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public IEnumerable<Vendor> GetVendorByOpSeq(int op_seq) {
            var sql = @"SELECT DISTINCT v.* FROM [arrangement] a, [vendor] v 
                        WHERE (a.vendor_cd = v.vendor_cd)  
                            AND (a.op_seq = @op_seq)  
                        ORDER BY v.vendor_cd ";
            var prms = new SqlParamSet();
            prms.AddInt("@op_seq", op_seq);

            var list = Context.Database.SqlQuery<Vendor>(sql, prms.ToArray()).ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }


    }
}
