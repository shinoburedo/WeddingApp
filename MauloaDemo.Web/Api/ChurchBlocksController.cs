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
using MauloaDemo.Web.Controllers;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using MauloaDemo.Repository;


namespace MauloaDemo.Web.Api {

    public class ChurchBlocksController : BaseApiController {

        private ChurchBlockRepository db = new ChurchBlockRepository();

        // GET api/ChurchBlocks?church_cd={CODE}&block_date=MM/dd/yyyy&plan_kind=W
        public async Task<IEnumerable<ChurchBlockInfo>> GetBlocks(string church_cd, DateTime block_date, string plan_kind) {
            var list = await db.GetBlocksForDay(church_cd, block_date, plan_kind, null, _loginUser);
            return list;
        }

        [HttpPost]
        [Route("api/ChurchBlocks/GetAvailList")]
        [ResponseType(typeof(Dictionary<string, object>))]
        public IHttpActionResult GetAvailList(ChurchBlockRepository.SearchParams param) {

            var startDate = new DateTime(param.year.Year, param.month.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // GridのHeader取得
            var headers = db.GetHeaderList(param.church_cd, "HH:mm");
            var blocks = db.GetList(param.church_cd, param.year, param.month, param.fdHol, param.stHol, param.sun, "HH:mm");

            ////ChurchBlockにレコードが存在するかフラグ
            //var is_block_exist = db.GetList(param.church_cd, param.year, param.month).Count > 0 ? true : false;

            var result = new Dictionary<string, object>();
            result.Add("headers", headers);
            result.Add("blocks", blocks);
            result.Add("minDate", startDate);
            result.Add("maxDate", endDate);
            result.Add("times", headers.Where(m => m.start_time.HasValue).Select(m => new { m.start_time }).ToList());
            //result.Add("blockExist", is_block_exist);

            return Ok(result);
        }

        [HttpPost]
        [Route("api/ChurchBlocks/SaveAvailList")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> SaveAvailList(ChurchBlockRepository.SaveParams param) {

            var status = string.Empty;

            try {
                await db.SaveChurchAvailListAsync(param.church_cd, param.avails, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }
            return Ok(status);
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}