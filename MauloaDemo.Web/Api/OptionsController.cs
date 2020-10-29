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

    public class OptionsController : BaseApiController {

        private SalesRepository db = new SalesRepository();

        public async Task<IEnumerable<SalesListItem>> GetOptions(string c_num) {
            return await db.GetOptions(c_num);
        }


        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}