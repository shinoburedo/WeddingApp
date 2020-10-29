using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using AutoMapper;
using CBAF;
using CBAF.Attributes;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;


namespace MauloaDemo.Repository {

    public class LoginUserRepository : BaseRepository<LoginUser> {

        public enum SaveLoginUserResult { 
            Error = -1,
            OK = 0,
            OK_CalendarOnly = 1,
            OK_DefaultTempPwd = 2
        }

        public class SearchParams {
            public string login_id { get; set; }
            public string user_name { get; set; }
            public string sub_agent_cd { get; set; }
        }


        public LoginUserRepository() 
            : base () {
        }

        public LoginUserRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
            AutoMapper.Mapper.CreateMap<LoginUser, LoginUser>();
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }

        public IEnumerable<LoginUser> GetList(LoginUser user) {
            var query = _context.LoginUsers.AsQueryable();

            if (user.IsAgent()) {
                query = query.Where(u => u.sub_agent_cd == user.sub_agent_cd);
            }

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public IEnumerable<LoginUser> Search(SearchParams param, LoginUser user) {
            if (param == null) {
                param = new SearchParams();
            }

            var query = Context.LoginUsers.AsQueryable();

            if (!string.IsNullOrEmpty(param.login_id)) {
                query = query.Where(t => t.login_id.StartsWith(param.login_id));
            }

            if (!string.IsNullOrEmpty(param.user_name)) {
                query = query.Where(t => t.e_first_name.Contains(param.user_name)
                                      || t.e_last_name.Contains(param.user_name)
                                      || t.j_first_name.Contains(param.user_name)
                                      || t.j_last_name.Contains(param.user_name));
            }

            if (!string.IsNullOrEmpty(param.sub_agent_cd)) {
                query = query.Where(a => a.sub_agent_cd.Equals(param.sub_agent_cd));
            }

            if (user.IsAgent()) {
                query = query.Where(u => u.sub_agent_cd == user.sub_agent_cd);
            }

            query = query.OrderBy(u => u.login_id);

            var list = query.ToList();
            ObjectReflectionHelper.TrimStrings(list);
            return list;
        }

        public async Task<IEnumerable<LoginUser>> SearchAsync(SearchParams param, LoginUser user) {
            return await Task.Run(() => this.Search(param, user));
        }

        //間違って基底クラスのFindが呼ばれた場合は例外を発生する。
        public new LoginUser Find(params object[] keyValues) {
            throw new NotImplementedException("Use Find(int id) instead.");
        }

        //間違って基底クラスのFindAsyncが呼ばれた場合は例外を発生する。
        public new async Task<LoginUser> FindAsync(params object[] keyValues) {
            await Task.Run(() => {
                //Dummy. Nothing to do actually.
            });
            throw new NotImplementedException("Use FindAsync(int id) instead.");
        }

        public LoginUser Find(string login_id) {
            var loginUser = Context.LoginUsers
                        .AsNoTracking()
                        .SingleOrDefault(e => e.login_id == login_id);

            if (loginUser != null) {
                //AgentParentテーブルからagent_cdを取得。
                var subAgentRepository = new SubAgentRepository(this);
                loginUser.agent_cd = subAgentRepository.GetAgentCd(loginUser.sub_agent_cd);

                if (!string.IsNullOrEmpty(loginUser.agent_cd)) { 
                    //Agentテーブルからstaff_required, branch_staff_requiredフラグを取得。
                    var agentRepo = new AgentRepository(this);
                    var agent = agentRepo.Find(loginUser.agent_cd);
                    if (agent != null) {
                        loginUser.staff_required = agent.staff_required;
                        loginUser.branch_staff_required = agent.branch_staff_required;
                    }
                }

                //配下とするエージェントを持っているか否かをチェックしてセット。
                loginUser.HasChildAgents = this.HasChildAgents(loginUser);

                //文字列フィールドの末尾の空白を除去。
                ObjectReflectionHelper.TrimStrings(loginUser);

                loginUser.date_format = string.IsNullOrWhiteSpace(loginUser.date_format) ? "MM/dd/yyyy" : loginUser.date_format;
                loginUser.time_format = string.IsNullOrWhiteSpace(loginUser.time_format) ? "HH:mm" : loginUser.time_format;
            }
            
            return loginUser;
        }
        public async Task<LoginUser> FindAsync(string login_id) {
            return await Task.Run(() => this.Find(login_id));
        }

        public SaveLoginUserResult SaveLoginUser(LoginUser loginUser, bool isNew, string last_person) {
            var status = SaveLoginUserResult.Error;

            //モデル側のValidateSaveを呼び出す。
            loginUser.ValidateSave();

            if (isNew) {
                CheckInvalidCharsForCode(loginUser.login_id);
            }

            using (var tran = this.Context.BeginTrans()) {
                try {
                    LoginUser oldModel = null;

                    var existing = Context.LoginUsers.Find(loginUser.login_id);
                    if (isNew) {
                        if (existing != null) {
                            throw new Exception(string.Format("User '{0}' already exists.", loginUser.login_id));
                        }
                        existing = loginUser;
                        Context.LoginUsers.Add(existing);
                    } else {
                        if (existing == null) {
                            throw new Exception(string.Format("User '{0}' cannot be found.", loginUser.login_id));
                        }
                        oldModel = Mapper.Map<LoginUser>(existing);

                        //変更可能な項目のみ更新。
                        existing.area_cd = loginUser.area_cd;
                        existing.sub_agent_cd = loginUser.sub_agent_cd;
                        existing.busho = loginUser.busho;
                        existing.section = loginUser.section;
                        existing.company = loginUser.company;
                        existing.e_last_name = loginUser.e_last_name;
                        existing.e_first_name = loginUser.e_first_name;
                        existing.j_last_name = loginUser.j_last_name;
                        existing.j_first_name = loginUser.j_first_name;
                        existing.access_level = loginUser.access_level;
                        existing.user_type = loginUser.user_type;
                        existing.e_mail = loginUser.e_mail;
                        existing.phone = loginUser.phone;
                        existing.eff_from_pass = loginUser.eff_from_pass;
                        existing.eff_to_pass = loginUser.eff_to_pass;
                        existing.locked = loginUser.locked;
                        existing.date_format = loginUser.date_format;
                        existing.time_format = loginUser.time_format;
                        existing.last_person = last_person;
                        existing.update_date = DateTime.UtcNow;
                        Context.Entry(existing).State = EntityState.Modified;
                    }

                    this.SaveChanges();
                    status = SaveLoginUserResult.OK;

                    //新規ユーザーの場合
                    //または既存ユーザーのaccess_levelを2以上から1に変えた場合
                    //または既存ユーザーのaccess_levelを1から2以上に変えた場合、
                    //デフォルトのパスワードをセットする。
                    //　access_levelが1の場合は有効期限はnullにする。
                    //　access_levelが2以上の場合は有効期限が切れた状態(eff_to_passが1日前)にする。
                    if (isNew 
                        || (oldModel != null && oldModel.access_level != 1 && existing.access_level == 1)
                        || (oldModel != null && oldModel.access_level == 1 && existing.access_level != 1)) {

                        //新規ユーザー用のデフォルトテンポラリパスワード。
                        var new_pwd = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["DefaultTemporaryPwd"]);
                        status = SaveLoginUserResult.OK_DefaultTempPwd;

                        //Access Levelが1の場合はweb.configで設定された固定のパスワードを使用。
                        if (existing.access_level == 1) {
                            var pwd_for_level1 = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["DefaultPwdForAccessLevel_1"]);
                            if (!string.IsNullOrEmpty(pwd_for_level1)) {
                                new_pwd = pwd_for_level1;
                                status = SaveLoginUserResult.OK_CalendarOnly;
                            }
                        }

                        //durationに-1を渡して eff_to_pass が過去の日付になる様にする。--> 初回ログイン時にパスワード変更を強制するため。
                        //但しaccess_levelが1の場合はeff_to_passはnullになる。--> パスワード変更を許可しない(=無期限にする)ため。
                        this.UpdatePasswordForUser(existing, new_pwd, last_person, false, -1);
                    } 

                    //変更ログを保存。
                    var log = new LogChangeRepository(this);
                    log.InsertLog(last_person, null, oldModel, existing);

                    tran.Commit();
                }
                catch (Exception) {
                    tran.Rollback();
                    throw;
                }
            }
            return status;
        }

        public void ValidateDelete(LoginUser loginUser, string delete_by) {
            if (loginUser == null) {
                throw new ArgumentNullException("login_id");
            }

            //既にsalesが入っていれば削除は不可。
            if (Context.Sales.Any(s => s.last_person == loginUser.login_id)){
                throw new InvalidOperationException(string.Format("The user '{0}' cannot be deleted because there are sales records created by this user.", loginUser.login_id));
            }

            //既にlog_changeが入っていれば削除は不可。
            if (Context.LogChanges.Any(c => c.login_id == loginUser.login_id)) {
                throw new InvalidOperationException(string.Format("The user '{0}' cannot be deleted because there are records in log_change table.", loginUser.login_id));
            }
        }

        public void DeleteLoginUser(LoginUser loginUser, string delete_by) {
            ValidateDelete(loginUser, delete_by);

            using (var tran = this.Context.BeginTrans()) {
                try {
                    //var sql = "DELETE [employee_token] WHERE [login_id] = @login_id";
                    //var prms = new SqlParamSet();
                    //prms.AddChar("@login_id", 6, loginUser.login_id);
                    //this.Context.ExecuteSQL(sql, prms.ToArray(), tran);

                    //sql = "DELETE [employee_access] WHERE [login_id] = @login_id";
                    //prms = new SqlParamSet();
                    //prms.AddChar("@login_id", 6, loginUser.login_id);
                    //this.Context.ExecuteSQL(sql, prms.ToArray(), tran);

                    var sql = "DELETE [login_user] WHERE [login_id] = @login_id";
                    var prms = new SqlParamSet();
                    prms.AddVarChar("@login_id", 15, loginUser.login_id);
                    this.Context.ExecuteSQL(sql, prms.ToArray(), tran);

                    //変更ログを保存。
                    var log = new LogChangeRepository(this);
                    log.InsertLogForDelete(delete_by, null, loginUser);

                    tran.Commit();
                }
                catch (Exception) {
                    tran.Rollback();
                    throw;
                }
            }
        }


        public LoginUser ValidateLogin(string login_id, string plain_pwd, out string error_msg) {
            error_msg = String.Empty;
            const string MSG_INVALID = "Invalid login. Please verify your User ID and Password.";

            LoginUser loginUser = null;
            try {
                //login_id単位で有効期限が切れたTokenを全て削除する。
                DeleteOldTokens(login_id);

                loginUser = this.Find(login_id);

                //login_idのチェック
                if (loginUser == null) {
                    error_msg = MSG_INVALID;
                    return null;
                }

                ////Resigned dateのチェック
                //if (loginUser.resigned_date.HasValue && loginUser.resigned_date <= Common.GetJapanDate()) {
                //    error_msg = MSG_INVALID;
                //    return null;
                //}

                //パスワードのチェック
                if (string.IsNullOrEmpty(loginUser.pwd_hash) || !Crypto.VerifyHashedPassword(loginUser.pwd_hash, plain_pwd)) {

                    //PEND: ハッキング防止のエラー回数カウントアップ処理など。

                    error_msg = MSG_INVALID;
                    return null;
                }

                //ユーザーがロックされているかどうかのチェック
                if (loginUser.locked) {
                    error_msg = "User account is locked.";
                    return null;
                }

                //ユーザーのaccess_levelが1以上かどうかのチェック。
                if (loginUser.access_level <= 0) {
                    error_msg = "User account is disabled.";
                    return null;
                }

                loginUser.Token = this.CreateToken(loginUser);

            } catch (Exception e) {
                error_msg = string.Format("{0} {1}",  e.Message, e.InnerException != null ? e.InnerException.Message : "");
                return null;
            }

            return loginUser;
        }

        public bool ValidateToken(string login_id, string token) {

            //login_id単位で有効期限が切れたTokenを全て削除する。
            DeleteOldTokens(login_id);

            //login_id, tokenが合致してかつ有効期限が切れていないTokenがあるかチェックする。
            var found = Context.LoginUserTokens
                                   .Where(m => m.login_id == login_id && m.token == token)
                                   .Any();

            return found;
        }

        /// <summary>
        ///login_id単位で有効期限が切れたTokenを全て削除する。
        /// </summary>
        /// <param name="login_id"></param>
        private void DeleteOldTokens(string login_id) {
            var japan_now = Common.GetJapanNow();
            var results = Context.LoginUserTokens
                                   .Where(m => m.login_id == login_id)
                                   .Where(m => m.expires <= japan_now)
                                   .ToList();

            foreach (var t in results) {
                Context.LoginUserTokens.Remove(t);
            }
            Context.SaveChanges();
        }

        public LoginUserToken CreateToken(LoginUser loginUser) {
            LoginUserToken token = new LoginUserToken();
            token.login_id = loginUser.login_id;
            token.token = GetTokenString();
            token.create_date = Common.GetJapanNow();

            int expiration = TypeHelper.GetInt(ConfigurationManager.AppSettings["AuthTokenExpiration"]);
            if (expiration == 0) expiration = 5;
            token.expires = Common.GetJapanNow().AddMinutes(expiration);

            Context.LoginUserTokens.Add(token);
            Context.SaveChanges();

            return token;
        }

        private string GetTokenString() {
            string s = new Guid().ToString();
            s = Crypto.HashPassword(s);
            return s;
        }

        /// <summary>
        /// 特定のTokenを削除する。
        /// </summary>
        /// <param name="login_id"></param>
        public void DeleteToken(LoginUserToken token) {
            var result = Context.LoginUserTokens
                                   .Where(m => m.login_id == token.login_id)
                                   .Where(m => m.token == token.token)
                                   .SingleOrDefault();
            if (result != null) {
                Context.LoginUserTokens.Remove(result);
                Context.SaveChanges();
            }
        }

        
        //public void SaveLoginUserProfile(LoginUser loginUser) {
        //    LoginUser user = this.Find(loginUser.login_id);
        //    if (user == null) {
        //        throw new KeyNotFoundException(loginUser.login_id);
        //    }

        //    //文字列フィールドの末尾の空白を除去 & 空白のみの場合はnullに変換。
        //    ObjectReflectionHelper.TrimStrings(loginUser, true);

        //    user.culture_name = loginUser.culture_name;
        //    user.date_format = loginUser.date_format;

        //    this.SetModified(user);
        //    this.SaveChanges();
        //}

        public int CountUsersWithNoPassword() {
            var count = GetUsersWithNoPassword()
                            .Count();
            return count;
        }

        private List<LoginUser> GetUsersWithNoPassword() {
            var list = Context.LoginUsers
                                .Where(e=> e.pwd_hash == null)
                                .ToList();
            return list;
        }

        ///// <summary>
        ///// 管理者用ページでハッシュ化されていないパスワードを一括でハッシュ化する時に呼ばれる処理。
        ///// (手動でemployeeテーブルに登録した場合など)
        ///// </summary>
        ///// <param name="last_person"></param>
        ///// <param name="includeAreaALL"></param>
        //public void ConvertAllRawPasswordToHashedPassword(string last_person, bool includeAreaALL = false) {
        //    var users = GetUsersWithNoPassword(includeAreaALL);
        //    foreach (var user in users) {
        //        UpdatePasswordForUser(user, user.password, last_person, true);
        //    }
        //    this.SaveChanges();
        //}

        /// <summary>
        /// パスワード変更処理を実行する。
        /// ユーザーがProfile画面で自分のパスワードを変更する時に呼ばれる。
        /// </summary>
        /// <param name="login_id"></param>
        /// <param name="cur_password"></param>
        /// <param name="new_password"></param>
        /// <param name="last_person"></param>
        public LoginUser.PwdChgResult ChangePassword(
                        string login_id, 
                        string cur_password, 
                        string new_password, 
                        string last_person) {

            string error_msg = String.Empty;
            LoginUser user = this.ValidateLogin(login_id, cur_password, out error_msg);
            if (user == null) {
                return LoginUser.PwdChgResult.InvalidOldPass;
            }

            //新しいパスワードがパスワード規則に合っているかチェック。
            LoginUser.PwdChgResult res = user.ValidateChangePassword(cur_password, new_password);
            if (res == LoginUser.PwdChgResult.Ok) {
                //パスワードの変更をDBに保存。
                var duration = TypeHelper.GetInt(ConfigurationManager.AppSettings["PasswordEffectiveDays"]);
                UpdatePasswordForUser(user, new_password, last_person, false, duration);
            } 

            return res;
        }

        /// <summary>
        /// </summary>
        /// <param name="user">対象ユーザー</param>
        /// <param name="new_password">新しいパスワード</param>
        /// <param name="last_person">変更を実行しているユーザーのID</param>
        /// <param name="batchMode">true=バッチモード</param>
        /// <param name="duration">
        ///     パスワードの有効期限（日数）
        ///     nullの場合はweb.configの設定に従う。
        ///     対象ユーザーのAccess Levelが1の場合は無期限(eff_to_pass=null)にする。
        /// </param>
        private void UpdatePasswordForUser(
                            LoginUser user, 
                            string new_password, 
                            string last_person, 
                            bool batchMode,
                            int duration) {

            var hashed_pwd = Crypto.HashPassword(new_password);
            user.pwd_hash = hashed_pwd;
            user.password = "***";
            user.eff_from_pass = Common.GetJapanDate();

            if (user.access_level == 1) {
                user.eff_to_pass = null;
            } else {
                user.eff_to_pass = Common.GetJapanDate().AddDays(duration);
            }

            user.last_person = last_person;
            user.update_date = DateTime.UtcNow;

            this.SetModified(user);

            if (!batchMode) { 
                this.SaveChanges(); 
            }

            //アクションログを保存。
            var actionRepo = new LogActionRepository(this);
            actionRepo.InsertAction(last_person, 
                                    LogAction.ACT_CHGPWD, 
                                    string.Format("User: {0}, Pwd Effective Until: {1}", 
                                                    user.login_id, 
                                                    user.eff_to_pass.HasValue ? user.eff_to_pass.Value.ToString("MM/dd/yyyy") : "none"),
                                                    null);
        }

        public void SetAutoPwd(string last_person) {
            using (var tran = this.Context.BeginTrans()) {
                try {
                    var list = this.GetUsersWithNoPassword();
                    foreach (var user in list) {
                        //テンポラリのパスワードで有効期限が切れた状態にする。(ユーザーはログイン後に必ずパスワード変更が必要)
                        var new_pwd = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["DefaultTemporaryPwd"]);   
                        this.UpdatePasswordForUser(user, new_pwd, last_person, true, -1);
                    }
                    this.SaveChanges();
                    tran.Commit();
                } catch (Exception) {
                    tran.Rollback();
                    throw;
                }
            }
        }

        public string ResetPassword(string target_login_id, string cur_pwd, LoginUser reset_by) {
            var target_user = this.Find(target_login_id);
            if (target_user == null) throw new Exception("Target User not found.");
            if (reset_by == null) throw new Exception("Reset By User is required.");

            //自分のAccess Levelが1以下の場合はリセット不可。
            if (reset_by.access_level <= 1) {
                throw new Exception("Not authorized.");
            }

            if (!reset_by.IsStaff() || reset_by.login_id.Equals(target_login_id)) {
                //現在のパスワードをチェック
                if (string.IsNullOrEmpty(cur_pwd)
                    || string.IsNullOrEmpty(target_user.pwd_hash)
                    || !Crypto.VerifyHashedPassword(target_user.pwd_hash, cur_pwd)) {
                    throw new Exception("Invalid credential.");
                }
            }

            //パスワードを自動生成。
            var min_length = Math.Max(TypeHelper.GetShort(ConfigurationManager.AppSettings["PasswordLengthMin"]), (short)6);
            var new_pwd = GeneratePwd(min_length);

            //ターゲットのAccess Levelが1の場合はweb.configで設定された固定のパスワードを使用。
            if (target_user.access_level == 1) {
                var pwd_for_level1 = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["DefaultPwdForAccessLevel_1"]);
                if (!string.IsNullOrEmpty(pwd_for_level1)) {
                    new_pwd = pwd_for_level1;
                }
            } 

            //パスワードを更新し、Web.configの設定にしたがって有効期限をセット。
            var duration = TypeHelper.GetInt(ConfigurationManager.AppSettings["PasswordEffectiveDays"]);
            UpdatePasswordForUser(target_user, new_pwd, reset_by.login_id, false, duration);

            return new_pwd;
        }

        public string GeneratePwd(short length) {
            if (length < 6) {
                throw new Exception("GeneratePwd: length must be 6 or more.");
            }

            var nums = "123456789";
            var lowers = "abcdefghjkmnpqrstuvwxyz";
            var uppers = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            var signs = "%&$#@!";
            var all = nums + lowers + uppers + signs;
            var r = new Random();

            var s = nums.Substring(r.Next(0, nums.Length - 1), 1)
                    + lowers.Substring(r.Next(0, lowers.Length - 1), 1)
                    + uppers.Substring(r.Next(0, uppers.Length - 1), 1)
                    + signs.Substring(r.Next(0, signs.Length - 1), 1);
            for (int i = s.Length; i < length; i++) {
                s += all.Substring(r.Next(0, all.Length - 1), 1);
            }

            var arr = s.ToCharArray();
            char[] arr2 = arr.OrderBy(i => Guid.NewGuid()).ToArray();
            var pwd = new string(arr2);
            return pwd;
        }

        /// <summary>
        /// ユーザーがカスタマーの情報にアクセス可能かどうかを返す。
        /// </summary>
        /// <param name="user"></param>
        /// <param name="c_num"></param>
        /// <returns>true:アクセス可能、false: アクセス不可</returns>
        /// 
        public bool CanViewCustomer(LoginUser user, string c_num) {
            if (user == null) return false;

            //新規ユーザーの作成はどのユーザーも可能。
            if (string.IsNullOrEmpty(c_num)) return true;

            //エージェントユーザーの場合
            if (user.IsAgent()) {
                var customerRepo = new CustomerRepository(this);
                var cust_sub_agent_cd = customerRepo.GetCustomerSubAgentCd(c_num);

                //sub_agent_cdが一致しない場合はログインユーザーのエージェントの配下のエージェントのいずれかと一致するかチェック。
                if (user.sub_agent_cd != cust_sub_agent_cd) {
                    if (user.HasChildAgents) {
                        var subAgentRepo = new SubAgentRepository(this);
                        return subAgentRepo.HasAccessTo(user.sub_agent_cd, cust_sub_agent_cd);
                    }
                    return false;
                }
            }
            return true;
        }
        public async Task<bool> CanViewCustomerAsync(LoginUser user, string c_num) {
            return await Task.Run(() => CanViewCustomer(user, c_num));
        }

        public bool HasChildAgents(LoginUser loginUser) {
            var subAgentRepository = new SubAgentRepository(this);
            return subAgentRepository.GetChildList(loginUser.sub_agent_cd)
                                     .Any(a => a.child_cd != loginUser.sub_agent_cd);
        }
        public async Task<bool> HasChildAgentsAsync(LoginUser loginUser) {
            return await Task.Run(() => HasChildAgents(loginUser));
        }

    }
}