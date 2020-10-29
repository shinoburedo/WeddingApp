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
using MauloaDemo.Repository;
using CBAF.Attributes;


namespace MauloaDemo.Web.Api {
    public class ChurchTimesController : BaseApiController {

        private ChurchTimeRepository db = new ChurchTimeRepository();

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [Route("api/ChurchTimes/Search")]
        [ResponseType(typeof(IEnumerable<ChurchTime>))]
        public async Task<IHttpActionResult> Search(ChurchTimeRepository.SearchParams param) {
            var list = await db.SearchAsync(param);
            return Ok(list);
        }

        [HttpPost]
        [Route("api/ChurchTimes/savelist")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> SaveList(ChurchTimeRepository.SaveParams param) {
            var status = string.Empty;
            try {
                await db.SaveListAsync(param.list, this._loginUser);
                status = "ok";

            } catch (Exception ex) {
                status = buildExceptionErrorMsg(ex);
            }

            return Ok(status);
        }

        [HttpPost]
        [Route("api/ChurchTimes/Delete/{id}")]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> Delete(int id) {
            var status = string.Empty;
            try {
                await db.DeleteAsync(id, this._loginUser);
                status = "ok";
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                status = "error: " + ex.Message;
            }
            return Ok(status);
        }

        //[HttpPost]
        //[Route("api/ChurchTimes/DeleteAll/{id}")]
        //[ResponseType(typeof(string))]
        //public async Task<IHttpActionResult> DeleteAll(string id) {
        //    var status = string.Empty;
        //    try {
        //        await db.DeleteByChurchCdAsync(id, this._loginUser);
        //        status = "ok";
        //    } catch (Exception ex) {
        //        while (ex.InnerException != null) { ex = ex.InnerException; }
        //        status = "error: " + ex.Message;
        //    }
        //    return Ok(status);
        //}

    }
}