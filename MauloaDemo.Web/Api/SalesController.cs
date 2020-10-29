using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using CBAF;
using MauloaDemo.Web.Controllers;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using MauloaDemo.Repository;


namespace MauloaDemo.Web.Api {

    public class SalesController : BaseApiController {

        private SalesRepository db = new SalesRepository();

        //// GET api/Sales
        //public async Task<IEnumerable<Sales>> GetSales(SalesRepository.SearchParams prms) {
        //    return await db.GetList(prms);
        //}

        //[HttpPost]
        //[Route("api/Sales/search")]
        //public async Task<IEnumerable<Sales>> Search(SalesRepository.SearchParams prms) {
        //    return await db.GetList(prms);
        //}

        [Route("api/Sales/getoptions")]
        public IEnumerable<SalesListItem> GetOptions(string c_num) {
            //エージェントユーザーの場合はCustomerのsub_agent_cdで制限を掛ける。
            if (_loginUser.IsAgent()) {
                var loginUserRepo = new LoginUserRepository();
                if (!loginUserRepo.CanViewCustomer(_loginUser, c_num)) {
                    throw new HttpResponseException(HttpStatusCode.NotFound);
                }
            }
            var agent_cd = _loginUser.IsAgent() ? _loginUser.agent_cd  : "";

            var list = Task.Run(() => db.GetOptions(c_num, agent_cd)).Result.ToList();

            //ユーザーのTime Zoneを取得。
            var time_zone = this._loginUser.time_zone;

            // 処理日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            list.ForEach(i => {
                i.book_proc_date = i.book_proc_date.HasValue ? i.book_proc_date.Value.AddHours(time_zone) : i.book_proc_date;
                i.update_date = i.update_date.AddHours(time_zone);
            });
            return list;
        }

        [Route("api/Sales/getchildren")]
        public async Task<IEnumerable<SalesListItem>> GetChildren(int op_seq, string item_cd) {
            return await db.GetChildrenAsync(op_seq, item_cd);
        }

        [Route("api/Sales/bookingreport")]
        public async Task<IEnumerable<BookingReport.Sales>> GetBookingReport(DateTime? date_from, DateTime? date_to, string agent_cd,
            string church_cd, string area_cd, string item_type, string vendor_cd, string item_cd,
            bool include_cust_cxl, bool include_sales_cxl, bool not_finalized_only, BookingReport.SortBy? sort_by) {
            var param = new BookingReport.SearchParam() {
                date_from = date_from,
                date_to = date_to,
                agent_cd = agent_cd,
                church_cd = church_cd,
                area_cd = area_cd,
                item_cd = item_cd,
                item_type = item_type,
                vendor_cd = vendor_cd,
                include_cust_cxl = include_cust_cxl,
                include_sales_cxl = include_sales_cxl,
                not_finalized_only = not_finalized_only,
                sort_by = sort_by
            };
            return await db.GetBookingReportAsync(param);
        }

        // GET api/Sales/{op_seq}
        [ResponseType(typeof(Sales))]
        public async Task<IHttpActionResult> GetSales(int id) {
            Sales sales = await db.FindAsync(id, this._loginUser);
            if (sales == null) {
                return NotFound();
            }

            //ユーザーのTime Zoneを取得。
            var time_zone = this._loginUser.time_zone;

            //各種日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            sales.book_proc_date = sales.book_proc_date.HasValue ? sales.book_proc_date.Value.AddHours(time_zone) : sales.book_proc_date;
            sales.jpn_cfm_date = sales.jpn_cfm_date.HasValue ? sales.jpn_cfm_date.Value.AddHours(time_zone) : sales.jpn_cfm_date;
            sales.create_date = sales.create_date.AddHours(time_zone);
            sales.update_date = sales.update_date.AddHours(time_zone);

            return Ok(sales);
        }

        // POST api/sales
        [ApiAccessLevelFilter(3)]
        [ResponseType(typeof(SalesRepository.SalesResult))]
        public async Task<IHttpActionResult> PostSales(Sales sales) {
            if (sales == null) {
                return NotFound();
            }

            var result = await Task.Run(() =>  db.Save(sales, this._loginUser, null));
            return Ok(result);
        }



        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }

}