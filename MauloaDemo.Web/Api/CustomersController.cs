using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using CBAF;
using CBAF.Attributes;
using MauloaDemo.Web.Controllers;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using MauloaDemo.Repository;
using MauloaDemo.Repository.Exceptions;
using System.Data.Entity.Validation;
//using System.Web.Mvc;

namespace MauloaDemo.Web.Api
{
    [ApiAccessLevelFilter(2)]
    public class CustomersController : BaseApiController {

        private CustomerRepository db = new CustomerRepository();

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // POST api/Customers/search
        [HttpPost]
        [Route("api/customers/search")]
        public async Task<GridResult<CustomerListItem>> Search(CustomerRepository.SearchParams prms) {

            //エージェントユーザーの場合は自分のsub_agent_cdで絞り込む。
            if (_loginUser.IsAgent()) {
                if (string.IsNullOrEmpty(prms.sub_agent_cd)) {
                    prms.sub_agent_cd = _loginUser.sub_agent_cd;
                }
            }

            //ユーザーのTime Zoneを反映。
            prms.time_zone = this._loginUser.time_zone;

            //変更履歴検索時はステータスで絞らない。
            if (prms.srch_type == CustomerRepository.SearchParams.SRCH_CHANGE) {
                prms.book_status = "";
            }

            //検索処理を実行。
            var result = await Task.Run(() => db.Search(prms, this._loginUser));

            if (prms.srch_type == CustomerRepository.SearchParams.SRCH_CHANGE) {
                //変更ログの日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
                foreach (var item in result.data) {
                    item.log_datetime = item.log_datetime.Value.AddHours(prms.time_zone);
                }
            } else {
                //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
                foreach (var item in result.data) {
                    item.create_date = item.create_date.AddHours(prms.time_zone);
                    item.update_date = item.update_date.AddHours(prms.time_zone);
                }
            }

            return result;
        }

        //// GET api/Customers?page=1&pagesize=20
        //public IEnumerable<Customer> GetCustomers(int page, int pagesize) {
        //    return db.GetCustomers(page, pagesize);
        //}


        // GET api/Customers/5
        [ResponseType(typeof(Customer))]
        public async Task<IHttpActionResult> GetCustomer(string id) {
            //ログインユーザーのカスタマーへの権限をチェック。
            var loginUserRepo = new LoginUserRepository(db);
            if (!loginUserRepo.CanViewCustomer(this._loginUser, id)) {
                throw new InvalidOperationException("Not authorized.");
            }

            Customer customer = await db.FindAsync(id);
            if (customer == null){
                return NotFound();
            }

            //ユーザーのTime Zoneを取得。
            var time_zone = this._loginUser.time_zone;

            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            customer.create_date = customer.create_date.AddHours(time_zone);
            customer.update_date = customer.update_date.AddHours(time_zone);

            return Ok(customer);
        }

        [ApiAccessLevelFilter(3)]
        [ResponseType(typeof(CustomerRepository.CustomerResult))]
        public async Task<IHttpActionResult> PostCustomer(Customer customer) {
            var result = new CustomerRepository.CustomerResult();
            try {
                if (!string.IsNullOrEmpty(customer.c_num)) {
                    //ログインユーザーのカスタマーへの権限をチェック。
                    var loginUserRepo = new LoginUserRepository(db);
                    if (!loginUserRepo.CanViewCustomer(this._loginUser, customer.c_num)) {
                        throw new InvalidOperationException("Not authorized.");
                    }
                }

                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(customer);
                UpperCaseHelper.Apply(customer);
                LowerCaseHelper.Apply(customer);
                customer.last_person = this._loginUser.login_id;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<Customer>(customer);
                if (!ModelState.IsValid) {
                    throw new Exception(buildModelStateErrorMsg());
                }

                result = await db.SaveCustomer(customer, this._loginUser, null);

            } catch (CustNameDuplicateException exDup) {
                result.message = "DUP";
                result.dupList = exDup.dupList;
            } catch (Exception ex) {
                result.message = ex.Message;
                result.c_num = "";
            }

            return Ok(result);
        }



        [ApiAccessLevelFilter(1)]
        [Route("api/customers/note"), HttpPost]
        [ResponseType(typeof(CustomerRepository.CustomerResult))]
        public async Task<IHttpActionResult> PostCustomerNote(Customer.Note note_info) {
            var result = new CustomerRepository.CustomerResult();
            try {
                if (!string.IsNullOrEmpty(note_info.c_num)) {
                    //ログインユーザーのカスタマーへの権限をチェック。
                    var loginUserRepo = new LoginUserRepository(db);
                    if (!loginUserRepo.CanViewCustomer(this._loginUser, note_info.c_num)) {
                        throw new InvalidOperationException("Not authorized.");
                    }
                }

                Customer customer = await db.FindAsync(note_info.c_num);
                if (customer == null) {
                    return NotFound();
                }

                //末尾の空白を除去。
                note_info.note = TypeHelper.GetStrTrim(note_info.note);

                if (note_info.edit_all) {
                    //過去の内容を修正出来るのはPROMでAccessLevelが4以上のユーザーのみ
                    if (!_loginUser.IsStaff() || _loginUser.access_level < 4) {
                        throw new InvalidOperationException("Not authorized.");
                    }

                    customer.note = note_info.note;

                } else {
                    if (string.IsNullOrEmpty(note_info.note)){
                        throw new ArgumentNullException("Note is required.");
                    }

                    //var written_date = Common.GetJapanNow().ToString("M/d HH:mm") + " JPN";
                    var serverLocation = RegionConfig.GetServerLocation();
                    var regionNow = RegionConfig.GetRegionNow(serverLocation);
                    var written_date = string.Format("{0} ({1})", regionNow.ToString("M/d HH:mm"), serverLocation);
                    var written_by = string.Format("{0} ({1}/{2})", _loginUser.UserNameJpn, _loginUser.login_id, _loginUser.sub_agent_cd);
                    customer.note = note_info.note + "\r\n" + 
                                    "--- " + written_by + " " + written_date + "\r\n\r\n" +
                                    customer.note;
                }

                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                HankakuHelper.Apply(customer);
                UpperCaseHelper.Apply(customer);
                LowerCaseHelper.Apply(customer);
                customer.last_person = this._loginUser.login_id;

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<Customer>(customer);
                if (!ModelState.IsValid) {
                    throw new Exception(buildModelStateErrorMsg());
                }

                result = await db.SaveCustomer(customer, this._loginUser, null);

            } catch (CustNameDuplicateException exDup) {
                result.message = "DUP";
                result.dupList = exDup.dupList;
            } catch (Exception ex) {
                result.message = ex.Message;
                result.c_num = "";
            }

            return Ok(result);
        }



        [Route("api/customers/{c_num}/addressinfos")]
        [ResponseType(typeof(List<AddressInfo>))]
        public async Task<IHttpActionResult> GetAddressInfos(string c_num) {
            //ログインユーザーのカスタマーへの権限をチェック。
            var loginUserRepo = new LoginUserRepository(db);
            if (!loginUserRepo.CanViewCustomer(this._loginUser, c_num)) {
                throw new InvalidOperationException("Not authorized.");
            }

            var addrInfoRepository = new AddressInfoRepository();
            var list = await addrInfoRepository.GetList(c_num);
            return Ok(list);
        }

        [Route("api/customers/addressinfos/{id:int}")]
        [ResponseType(typeof(AddressInfo))]
        public async Task<IHttpActionResult> GetAddressInfo(int id) {
            var addrInfoRepository = new AddressInfoRepository();
            var info = await addrInfoRepository.FindAsync(id);

            //ログインユーザーのカスタマーへの権限をチェック。
            var loginUserRepo = new LoginUserRepository(db);
            if (!loginUserRepo.CanViewCustomer(this._loginUser, info.c_num)) {
                throw new InvalidOperationException("Not authorized.");
            }
            
            return Ok(info);
        }

        [ApiAccessLevelFilter(3)]
        [Route("api/customers/addressinfos"), HttpPost]
        [ResponseType(typeof(AddressInfoRepository.AddressInfoResult))]
        public async Task<IHttpActionResult> PostAddressInfo(AddressInfo addressInfo) {
            var addrInfoRepository = new AddressInfoRepository(db);
            var result = new AddressInfoRepository.AddressInfoResult();
            try {
                //ログインユーザーのカスタマーへの権限をチェック。
                var loginUserRepo = new LoginUserRepository(db);
                if (!loginUserRepo.CanViewCustomer(this._loginUser, addressInfo.c_num)) {
                    throw new InvalidOperationException("Not authorized.");
                }

                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                addressInfo.last_person = this._loginUser.login_id;
                HankakuHelper.Apply(addressInfo);
                UpperCaseHelper.Apply(addressInfo);
                LowerCaseHelper.Apply(addressInfo);

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<AddressInfo>(addressInfo);
                if (!ModelState.IsValid) {
                    throw new Exception(buildModelStateErrorMsg());
                }

                await addrInfoRepository.SaveAsync(addressInfo, this._loginUser);
                result.info_id = addressInfo.info_id;
                result.message = "ok";

            } catch (DbEntityValidationException ex) {
                //ModelのValidationエラーがあった場合はそのメッセージを返す。
                var error = ex.EntityValidationErrors.First().ValidationErrors.First();
                result.info_id = 0;
                result.message = string.Format("{0}", error.ErrorMessage);
            } catch (Exception ex) {
                //その他のエラーがあった場合は最も内側のExceptionのメッセージを返す。
                result.info_id = 0;
                while (ex.InnerException != null) ex = ex.InnerException;
                result.message = ex.Message;
            }
            return Ok(result);
        }

        [ApiAccessLevelFilter(3)]
        [HttpDelete]
        [Route("api/customers/{c_num}/addressinfos/{id:int}")]
        public async Task<IHttpActionResult> DeleteAddressInfo(string c_num, int id) {
            try {
                //ログインユーザーのカスタマーへの権限をチェック。
                var loginUserRepo = new LoginUserRepository(db);
                if (!loginUserRepo.CanViewCustomer(this._loginUser, c_num)) {
                    throw new InvalidOperationException("Not authorized.");
                }

                var addrInfoRepository = new AddressInfoRepository();
                await addrInfoRepository.DeleteAsync(id, this._loginUser);
                return Ok(new { result = "ok", message = "" });
            } catch (Exception ex) {
                while (ex.InnerException != null) ex = ex.InnerException;
                return Ok(new { result = "error", message = ex.Message });
            }
        }


        [Route("api/customers/{c_num}/cosinfos")]
        [ResponseType(typeof(List<CosInfo>))]
        public async Task<IHttpActionResult> GetCosInfos(string c_num)
        {
            //ログインユーザーのカスタマーへの権限をチェック。
            var loginUserRepo = new LoginUserRepository(db);
            if (!loginUserRepo.CanViewCustomer(this._loginUser, c_num)){
                throw new InvalidOperationException("Not authorized.");
            }

            var cosInfoRepository = new CosInfoRepository();
            var list = await cosInfoRepository.GetList(c_num);
            return Ok(list);
        }

        [Route("api/customers/cosinfos/{id:int}")]
        [ResponseType(typeof(CosInfo))]
        public async Task<IHttpActionResult> GetCosInfo(int id) {
            var cosInfoRepository = new CosInfoRepository();
            var info = await cosInfoRepository.FindAsync(id);

            //ログインユーザーのカスタマーへの権限をチェック。
            var loginUserRepo = new LoginUserRepository(db);
            if (!loginUserRepo.CanViewCustomer(this._loginUser, info.c_num)){
                throw new InvalidOperationException("Not authorized.");
            }

            return Ok(info);
        }

        [ApiAccessLevelFilter(3)]
        [Route("api/customers/cosinfos"), HttpPost]
        [ResponseType(typeof(CosInfoRepository.CosInfoResult))]
        public async Task<IHttpActionResult> PostCosInfo(CosInfo cosInfo) {
            var cosInfoRepository = new CosInfoRepository(db);
            var result = new CosInfoRepository.CosInfoResult();
            try {
                //ログインユーザーのカスタマーへの権限をチェック。
                var loginUserRepo = new LoginUserRepository(db);
                if (!loginUserRepo.CanViewCustomer(this._loginUser, cosInfo.c_num)) {
                    throw new InvalidOperationException("Not authorized.");
                }

                //Model Bindingで自動で実行された検証結果を一旦クリアして半角・大文字変換を行ってから再度検証を実行。
                ModelState.Clear();
                cosInfo.last_person = this._loginUser.login_id;
                HankakuHelper.Apply(cosInfo);
                UpperCaseHelper.Apply(cosInfo);
                LowerCaseHelper.Apply(cosInfo);

                //ここで再度検証を実行。(エラーがあればエラーメッセージを返す。)
                this.Validate<CosInfo>(cosInfo);
                if (!ModelState.IsValid) {
                    throw new Exception(buildModelStateErrorMsg());
                }

                await cosInfoRepository.SaveAsync(cosInfo, this._loginUser);
                result.info_id = cosInfo.info_id;
                result.message = "ok";
            }
            catch (DbEntityValidationException ex) {
                //ModelのValidationエラーがあった場合はそのメッセージを返す。
                var error = ex.EntityValidationErrors.First().ValidationErrors.First();
                result.info_id = 0;
                result.message = string.Format("{0}", error.ErrorMessage);
            } catch (Exception ex) {
                //その他のエラーがあった場合は最も内側のExceptionのメッセージを返す。
                result.info_id = 0;
                while (ex.InnerException != null) ex = ex.InnerException;
                result.message = ex.Message;
            }
            return Ok(result);
        }

        [ApiAccessLevelFilter(3)]
        [HttpDelete]
        [Route("api/customers/{c_num}/cosinfos/{id:int}")]
        public async Task<IHttpActionResult> DeleteCosInfo(string c_num, int id) {
            try {
                //ログインユーザーのカスタマーへの権限をチェック。
                var loginUserRepo = new LoginUserRepository(db);
                if (!loginUserRepo.CanViewCustomer(this._loginUser, c_num)) {
                    throw new InvalidOperationException("Not authorized.");
                }

                var cosInfoRepository = new CosInfoRepository();
                await cosInfoRepository.DeleteAsync(id, this._loginUser);
                return Ok(new { result = "ok", message = "" });
            } catch (Exception ex) {
                while (ex.InnerException != null) ex = ex.InnerException;
                return Ok(new { result = "error", message = ex.Message });
            }
        }

        [ApiAccessLevelFilter(3)]
        [Route("api/customers/{c_num}/finalize/{isFinalize:bool}"), HttpPost]
        [ResponseType(typeof(CustomerRepository.CustomerResult))]
        public async Task<IHttpActionResult> UpdateFinalInfo(string c_num, bool isFinalize) {
            var result = new CustomerRepository.CustomerResult();
            try {
                if (!string.IsNullOrEmpty(c_num)) {
                    //ログインユーザーのカスタマーへの権限をチェック。
                    var loginUserRepo = new LoginUserRepository(db);
                    if (!loginUserRepo.CanViewCustomer(this._loginUser, c_num)) {
                        throw new InvalidOperationException("Not authorized.");
                    }
                }
                await Task.Run(() => db.UpdateFinalInfo(c_num, this._loginUser.login_id, isFinalize));
                result.c_num = c_num;
                result.message = "ok";
            }
            catch (Exception ex) {
                result.message = ex.Message;
                result.c_num = c_num;
            }
            return Ok(result);
        }

        [Route("api/customers/saveDupChkDone"), HttpPost]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> SaveChkDone(Customer.ChangedRows rows) {
            var result = "error";
            try {
                await Task.Run(() => db.SaveDupChkDone(rows, this._loginUser.login_id));
                result = "ok";
            } catch (Exception ex) {
                result = ex.Message;
            }
            return Ok(result);
        }


    }
}