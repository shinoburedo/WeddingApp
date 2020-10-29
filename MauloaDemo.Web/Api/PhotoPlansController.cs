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
using ProjectM.Web.Controllers;
using ProjectM.Models;
using ProjectM.Models.Combined;
using ProjectM.Repository;

namespace ProjectM.Web.Api {

    public class PhotoPlansController : BaseApiController {

        private ShootInfoRepository db = new ShootInfoRepository();

        // GET api/<controller>
        public async Task<IEnumerable<ShootInfo>> GetList(string c_num) {
            return await db.GetList(c_num);
        }

        // GET api/<controller>/5
        public ShootInfo Get(int id) {
            return null;
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]ShootInfo[] value) {
            return;
        }

        // DELETE api/<controller>/5
        public void Delete(int id) {
        }

        [ResponseType(typeof(ShootInfoRepository.ShootInfoResult))]
        public async Task<IHttpActionResult> PostShootInfos(ShootInfo[] plans) {

            if (plans == null) {
                return NotFound();
            }
            try {
                List<ShootInfoRepository.ShootInfoResult> results = new List<ShootInfoRepository.ShootInfoResult>();
                foreach (var item in plans) {
		            item.last_person = _loginUser.login_id;
                    var res = await db.Save(item);
                    results.Add(res);
                }

                return Ok(results);
            } catch (Exception) {
                throw;
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