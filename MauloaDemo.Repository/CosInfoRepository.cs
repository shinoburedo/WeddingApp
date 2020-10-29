using System;
using System.Collections.Generic;
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

    public class CosInfoRepository : BaseRepository<CosInfo>
    {

        public CosInfoRepository() {
        }

        public CosInfoRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        public class CosInfoResult {
            public int info_id { get; set; }
            public string message { get; set; }
        }



        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            Mapper.CreateMap<CosInfo, CosInfo>();
            Mapper.AssertConfigurationIsValid();
        }

        public async Task<IEnumerable<CosInfo>> GetList(string c_num) {
            var list = await Task.Run(() =>
                          Context.CosInfos
                                      .Where(o => o.c_num == c_num)
                                      .OrderBy(o => o.info_id)
                                      .ToList()
                      );
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        //間違って基底クラスのFindが呼ばれた場合は例外を発生する。
        public new CosInfo Find(params object[] keyValues) {
            throw new NotImplementedException("Use Find(int id) instead.");
        }

        //間違って基底クラスのFindAsyncが呼ばれた場合は例外を発生する。
        public new async Task<CosInfo> FindAsync(params object[] keyValues) {
            await Task.Run(() => {
                //Dummy. Nothing to do actually.
            });
            throw new NotImplementedException("Use FindAsync(int id) instead.");
        }

        /// <summary>
        /// info_idからCosInfoオブジェクトを取得する。
        /// </summary>
        /// <param name="info_id"></param>
        /// <returns></returns>
        public CosInfo Find(int info_id) {
            var info = Context.CosInfos
                            .Where(a => a.info_id == info_id)
                            .SingleOrDefault();

            ApplyMappings(info);     //文字列のTrim処理など。
            return info;
        }
        public async Task<CosInfo> FindAsync(int info_id) {
            return await Task.Run(() => this.Find(info_id));
        }


        public async Task<CostumeOrderSheetInfo> GetCosInfo(string c_num, LoginUser user) {
            //ログインユーザーのカスタマーへの権限をチェック。
            var loginUserRepo = new LoginUserRepository(this);
            if (!loginUserRepo.CanViewCustomer(user, c_num)) {
                throw new InvalidOperationException("Not authorized.");
            }

            var info = new CostumeOrderSheetInfo();

            //await Task.Run(() => info.CosInfo = this.Find(info_id));
            //if (info.CosInfo == null) return null;
            //if (!string.Equals(info.CosInfo.c_num, c_num)) return null;

            var customerRepository = new CustomerRepository(this);
            info.Customer = customerRepository.Find(c_num);
            if (info.Customer == null) return null;

            var churchRepository = new ChurchRepository(this);
            var church = churchRepository.Find(info.Customer.church_cd);
            info.church_name = church == null ? "" : church.church_name;

            var hotelRepository = new HotelRepository(this);
            var hotel = hotelRepository.Find(info.Customer.hotel_cd);
            info.hotel_name = hotel == null ? "" : hotel.hotel_name;

            var agentRepository = new AgentRepository(this);
            var agent = agentRepository.Find(info.Customer.agent_cd);
            info.agent_name = agent == null ? "" : agent.agent_name;

            var wedInfoRepository = new WedInfoRepository(this);
            var list = await wedInfoRepository.GetList(c_num, "", user);
            info.plan_name = (list == null || list.Count() < 1) ? "" : list.First().item_name;

            var arrangementRepository = new ArrangementRepository(this);
            var vendor_list = arrangementRepository.GetVendorByItemTypeAndCNum(c_num, "MKS").Where(a => a.vendor_cd != "PROM").ToList();
            info.MksVendorList = vendor_list;

            var vendor = arrangementRepository.GetVendorByItemTypeAndCNum(c_num, "COS").Where(a => a.vendor_cd != "PROM").FirstOrDefault();
            info.CosVendor = vendor;

            var dlv_list = GetFlwDeliveryInf(c_num).ToList();
            info.FlwDlvList = dlv_list;

            return info;
        }

        public IEnumerable<CostumeOrderSheetDeliveryInfo> GetFlwDeliveryInf(string c_num) {
            var sql = @"SELECT d.*, i.item_cd, i.item_name FROM 
                        (SELECT * FROM [sales]
                            WHERE (book_status = 'K') 
                            AND (item_type = 'FLW')  
                            AND (c_num = @c_num)) s, [delivery_info] d, [item] i
                        WHERE (d.op_seq = s.op_seq) 
                            AND (s.item_cd = i.item_cd) 
                        ORDER BY d.delivery_date, d.delivery_time, i.item_cd ";
            var prms = new SqlParamSet();
            prms.AddChar("@c_num", 7, c_num);

            var list = Context.Database.SqlQuery<CostumeOrderSheetDeliveryInfo>(sql, prms.ToArray()).ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;

        }

        public void Save(CosInfo cosInfo, LoginUser user) {
            CosInfo oldModel = null;
            CosInfo existing = null;

            if (cosInfo.info_id == 0) {
                existing = cosInfo;
                existing.create_by = user.login_id;
                existing.create_date = DateTime.UtcNow;
                existing.last_person = user.login_id;
                existing.update_date = existing.create_date;
                this.Add(existing);
            } else {
                existing = this.Find(cosInfo.info_id);
                if (existing == null) {
                    throw new Exception(string.Format("The key value '{0}' was not found.", cosInfo.info_id));
                }
                oldModel = Mapper.Map<CosInfo>(existing);

                //楽観的排他制御 (JSを経由するとミリ秒の情報が欠落するため秒単位で比較する。)
                if (TypeHelper.CompareDateBySecond(cosInfo.update_date, existing.update_date) != 0) {
                    throw new OptimisticLockException("Costume", user.IsStaff() ? existing.last_person : "");
                }
                existing.pax_type = cosInfo.pax_type;
                existing.height = cosInfo.height;
                existing.chest = cosInfo.chest;
                existing.waist = cosInfo.waist;
                existing.cloth_size = cosInfo.cloth_size;
                existing.shoe_size = cosInfo.shoe_size;
                existing.note = cosInfo.note;
                existing.last_person = user.login_id;
                existing.update_date = DateTime.UtcNow;
                this.SetModified(existing);
            }
            this.SaveChanges();

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            log.InsertLog(user.login_id, cosInfo.c_num, oldModel, existing);
        }

        public async Task SaveAsync(CosInfo cosInfo, LoginUser user) {
            await Task.Run(() => this.Save(cosInfo, user));
        }

        public void Delete(int info_id, LoginUser user) {
            var item = this.Find(info_id);
            if (item == null) {
                throw new Exception(string.Format("The key value '{0}' was not found.", info_id));
            }
            Context.Set<CosInfo>().Remove(item);
            this.SaveChanges();

            //変更ログを保存。
            var log = new LogChangeRepository(this);
            log.InsertLogForDelete(user.login_id, item.c_num, item);
        }

        public async Task DeleteAsync(int info_id, LoginUser user) {
            await Task.Run(() => this.Delete(info_id, user));
        }

        public async Task<IEnumerable<SalesListItem>> GetWedInfoChildrenByCNum(string c_num, LoginUser user) {
            var wed_repo = new WedInfoRepository(this);
            var order_info = await wed_repo.GetList(c_num, null, user);
            order_info = order_info.Where(a => a.book_status == BookingStatus.OK || a.book_status == BookingStatus.RQ);
            var sales_repo = new SalesRepository(this);
            var sales_list = new List<SalesListItem>();
            foreach (var order in order_info) {
                var sales = sales_repo.GetChildren(order.op_seq, order.item_cd).Where(a => a.item_type == "COS" && (a.book_status == BookingStatus.OK || a.book_status == BookingStatus.RQ));
                foreach (var item in sales) {
                    if (item.arrangements != null && item.arrangements.Count() > 0) {
                        item.note = item.arrangements[0].note;
                    }
                    sales_list.Add(item);
                }
            }
            return sales_list;
        }
    }
}
