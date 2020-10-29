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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using CBAF;
using MauloaDemo.Web.Controllers;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using MauloaDemo.Repository;
using MauloaDemo.Repository.Exceptions;

namespace MauloaDemo.Web.Api {
    public class WeddingPlansController : BaseApiController {

        private WedInfoRepository db = new WedInfoRepository();

        // GET api/weddingplans
        /// <summary>
        /// カスタマーの保存済みプラン一覧を取得。
        /// </summary>
        /// <param name="c_num"></param>
        /// <param name="plan_type">P=フォトプラン、W=ウェディングプラン、null=両方を取得</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/weddingplans")]
        public IEnumerable<WedInfo> GetList(string c_num, string plan_type) {
            //ログインユーザーのカスタマーへの権限をチェック。
            var loginUserRepo = new LoginUserRepository(db);
            if (!loginUserRepo.CanViewCustomer(this._loginUser, c_num)) {
                throw new InvalidOperationException("Not authorized.");
            }

            //プラン一覧を取得。
            var list = Task.Run(() => db.GetList(c_num, plan_type, this._loginUser)).Result.ToList();

            //ユーザーのTime Zoneを取得。
            var time_zone = this._loginUser.time_zone;

            //登録日時、更新日時について、UTC時刻からユーザーにとっての現地時刻に変換して返す。
            list.ForEach(i => {
                i.create_date =  i.create_date.AddHours(time_zone);
                i.update_date = i.update_date.AddHours(time_zone);
            });
            return list;
        }

        // POST api/weddingplans
        /// <summary>
        /// 複数のPlanを一度に保存。
        /// （カスタマー画面でプランの保存ボタンを押した時はこれが呼ばれる。)
        /// </summary>
        /// <param name="plans"></param>
        /// <returns></returns>
        [ApiAccessLevelFilter(3)]
        [HttpPost]
        [Route("api/weddingplans")]
        [ResponseType(typeof(List<WedInfoRepository.WedInfoResult>))]
        public async Task<IHttpActionResult> PostWedInfos(WedInfo[] plans) {
            if (plans == null || plans.Length == 0) {
                return NotFound();
            }

            List<WedInfoRepository.WedInfoResult> results 
                = await Task.Run(() => db.SavePlans(plans, this._loginUser, null));

            return Ok(results);
        }

        // POST api/weddingplans/CreateWithCustomer
        /// <summary>
        /// カスタマー登録とプラン保存を一度に実行。
        /// （カレンダー画面で日時をクリックして保存した場合はこちらが呼ばれる。）
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [ApiAccessLevelFilter(3)]
        [HttpPost]
        [Route("api/weddingplans/CreateWithCustomer")]
        [ResponseType(typeof(WedInfoRepository.WedInfoResult))]
        public async Task<IHttpActionResult> CreateWithCustomer(CustomerInfo info) {
            if (info == null ||  info.Customer == null || info.WeddingPlans == null || info.WeddingPlans.Count == 0) {
                return NotFound();
            }
            var result = new WedInfoRepository.WedInfoResult();

            try {
                result = await db.CreateWithCustomer(info.Customer, info.WeddingPlans[0], _loginUser);

            } catch (CustNameDuplicateException exDup) {
                result.status = "DUP";
                result.dupList = exDup.dupList;
            } catch (Exception ex) {
                result.status = "error";
                result.message = ex.Message;
            }

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