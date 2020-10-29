using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CBAF;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;

namespace MauloaDemo.Repository {

    public class LogActionRepository : BaseRepository<LogAction> {

        public LogActionRepository() {
        }

        public LogActionRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
        }

        public int InsertAction(string login_id, string action_cd, string detail, int? parent_log_id) {
            var logAction = new LogAction() { 
                login_id = login_id,
                action_cd = action_cd,
                detail = detail,
                parent_log_id = parent_log_id
            };
            Context.LogActions.Add(logAction);
            this.SaveChanges();
            return logAction.log_id;
        }

        public void Login(LoginUser loginUser) {
            var detail = string.Empty;
            if (loginUser.Token != null) {
                detail = loginUser.Token.token;
            }
            this.InsertAction(loginUser.login_id, LogAction.ACT_LOGIN, detail, null);
        }

        public void Logout(LoginUser loginUser) {
            var detail = string.Empty;
            if (loginUser.Token != null) {
                detail = loginUser.Token.token;
            }
            this.InsertAction(loginUser.login_id, LogAction.ACT_LOGOUT, detail, null);
        }


    }
}
