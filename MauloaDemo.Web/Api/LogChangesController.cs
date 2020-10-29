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

namespace MauloaDemo.Web.Api {

    public class LogChangesController : BaseApiController {

        private LogChangeRepository db = new LogChangeRepository();

        // GET api/LogChanges/5
        [ResponseType(typeof(LogChange))]
        public async Task<IHttpActionResult> GetLogChange(int id) {
            LogChange LogChange = await db.FindAsync(id);
            if (LogChange == null) {
                return NotFound();
            }
            return Ok(LogChange);
        }

        [Route("api/LogChanges/SaveArchived"), HttpPost]
        [ResponseType(typeof(string))]
        public async Task<IHttpActionResult> SaveArchived(LogChangeArchive.ChangedRows rows) {
            var result = "error";
            try {
                await Task.Run(() => db.ArchiveOrUnArchive(rows, this._loginUser));
                result = "ok";
            } catch (Exception ex) {
                while (ex.InnerException != null) { ex = ex.InnerException; }
                result = ex.Message;
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