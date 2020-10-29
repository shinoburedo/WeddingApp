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


namespace MauloaDemo.Web.Api
{
    public class ArrangementsController : BaseApiController
    {
        private ArrangementRepository db = new ArrangementRepository();

        // GET api/Arrangements/{id}
        [ResponseType(typeof(Arrangement))]
        public async Task<IHttpActionResult> GetArragement(int id) {
            Arrangement arr = await db.FindAsync(id);
            if (arr == null) {
                return NotFound();
            }
            return Ok(arr);
        }

        [HttpGet]
        [Route("api/Arrangements/ByOpSeq")]
        [ResponseType(typeof(List<Arrangement>))]
        public async Task<IHttpActionResult> GetArragementsByOpSeq(int op_seq) {
            var list = await db.GetArrangementsByOpSeqAsync(op_seq);
            return Ok(list);
        }

        [HttpGet]
        [Route("api/Arrangements/ForVendorConfirmation")]
        [ResponseType(typeof(List<VendorConfirmation>))]
        public async Task<IHttpActionResult> GetArragementsForVendorConfirmation(DateTime date_from, DateTime date_to)
        {
            //検索条件date_toはtimeを考慮しプラス1日しておく
            date_to = date_to.AddDays(1);
            var list = await db.GetArragementsForVendorConfirmationAsync(date_from, date_to);
            return Ok(list);
        }

        // POST api/Arrangements
        [ApiAccessLevelFilter(3)]
        [HttpPost]
        [ResponseType(typeof(Arrangement))]
        public async Task<IHttpActionResult> PostArrangement(Arrangement arr) {
            if (arr == null) {
                return NotFound();
            }

            arr.last_person = _loginUser.login_id;
            var result = await Task.Run(() => db.Save(arr, null));
            if ("ok".Equals(result.status)) {
                var saved = db.Find(result.arrangement_id);
                return Ok(saved);
            } else {
                return Ok();
            }
        }



        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
