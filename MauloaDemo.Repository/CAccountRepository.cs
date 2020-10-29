using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Configuration;
using System.Threading.Tasks;
using CBAF;
using MauloaDemo.Models;
using MauloaDemo.Models.Combined;
using MauloaDemo.Models.Helpers;
using MauloaDemo.Repository.Exceptions;

namespace MauloaDemo.Repository {

    public class CAccountRepository : BaseRepository<CAccount> {

        public class SearchParams {
            public string c_num { get; set; }
            public DateTime? date_from { get; set; }
            public DateTime? date_to { get; set; }

            public SearchParams() {
                date_from = null;
                date_to = null;
            }
        }

        public CAccountRepository() {
        }
        public CAccountRepository(IContextHolder sourceRepository)
            : base(sourceRepository) {
        }

        internal override void init(string region_cd) {
            base.init(region_cd);

            //CreateMapするならここで。
        }

        //********************************************************* 
        //******* ASP.NET Indentity 2.0 対応
        //********************************************************* 
        /// <summary>
        /// Finds the by identifier.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>WtAccount.</returns>
        public CAccount FindById(string email)
        {
            return Context.WtAccounts
                        .AsNoTracking()
                        .FirstOrDefault(u => u.email == email);
        }

        /// <summary>
        /// Finds the name of the by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>WtAccount.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public CAccount FindByName(string name)
        {
            //return Context.WtAccounts
            //            .AsNoTracking()
            //            .FirstOrDefault(u => u.UserName == name);
            throw new NotImplementedException();
        }

        /// <summary>
        /// find by identifier as an asynchronous operation.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns>Task&lt;WtAccount&gt;.</returns>
        public Task<CAccount> FindByIdAsync(string email)
        {
            return Task.FromResult(FindById(email));
        }

        /// <summary>
        /// find by name as an asynchronous operation.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Task&lt;WtAccount&gt;.</returns>
        public Task<CAccount> FindByNameAsync(string name)
        {
            return Task.FromResult(FindByName(name));
        }

        ///// <summary>
        ///// Creates the asynchronous.
        ///// </summary>
        ///// <param name="user">The user.</param>
        ///// <returns>Task.</returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public Task CreateAsync(WtAccount user)
        //{
        //    return Task.FromResult(this.Create(user, "J"));
        //}

        /// <summary>
        /// delete as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>Task.</returns>
        public Task DeleteAsync(CAccount user)
        {
            var target = this.FindByIdAsync(user.Id).Result;
            if (target != null)
            {
                Context.WtAccounts.Remove(target);
            }
            return Task.Delay(0);
        }

        ///// <summary>
        ///// validate login as an asynchronous operation.
        ///// </summary>
        ///// <param name="email">The email.</param>
        ///// <param name="plain_pwd">The plain_pwd.</param>
        ///// <returns>Task&lt;WtAccount&gt;.</returns>
        ///// <exception cref="System.Exception">
        ///// Invalid login.
        ///// or
        ///// Invalid login.
        ///// </exception>
        //public Task<WtAccount> ValidateLoginAsync(string email, string plain_pwd)
        //{
        //    var WtAccount = this.FindByIdAsync(email).Result;

        //    //Emailの存在チェック
        //    if (WtAccount == null)
        //    {
        //        throw new Exception("Invalid login.");
        //    }

        //    //パスワードのチェック
        //    if (!Crypto.VerifyHashedPassword(WtAccount.pwd_hash, plain_pwd))
        //    {
        //        throw new Exception("Invalid login.");
        //    }

        //    return Task.FromResult(WtAccount);
        //}

        ///// <summary>
        ///// Updates the asynchronous.
        ///// </summary>
        ///// <param name="user">The user.</param>
        ///// <returns>Task.</returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public Task UpdateAsync(WtAccount user)
        //{
        //    throw new NotImplementedException();
        //}

        ///// <summary>
        ///// get password hash as an asynchronous operation.
        ///// </summary>
        ///// <param name="user">The user.</param>
        ///// <returns>Task&lt;System.String&gt;.</returns>
        //public Task<string> GetPasswordHashAsync(WtAccount user)
        //{
        //    return Task.FromResult(new PasswordHasher().HashPassword(user.password));
        //}

        ///// <summary>
        ///// has password as an asynchronous operation.
        ///// </summary>
        ///// <param name="user">The user.</param>
        ///// <returns>Task&lt;System.Boolean&gt;.</returns>
        //public Task<bool> HasPasswordAsync(WtAccount user)
        //{
        //    return Task.FromResult(!string.IsNullOrEmpty(user.password));
        //}

        ///// <summary>
        ///// set password hash as an asynchronous operation.
        ///// </summary>
        ///// <param name="user">The user.</param>
        ///// <param name="passwordHash">The password hash.</param>
        ///// <returns>Task.</returns>
        //public Task SetPasswordHashAsync(WtAccount user, string passwordHash)
        //{
        //    EnsurePasswordIsHashed(user);
        //    return Task.Delay(0);
        //}
        ////********************************************************* 
        ////******* ↑↑↑ ASP.NET Indentity 2.0 対応 ↑↑↑ ここまで
        ////********************************************************* 


        ////public WtAccount Find(int account_id) {
        ////    var wtAccount = Context.WtAccounts
        ////                         .Where(i => i.account_id == account_id)
        ////                         .SingleOrDefault();
        ////    ObjectReflectionHelper.TrimStrings(wtAccount);
        ////    return wtAccount;
        ////}

        ////public WtAccount Find(string email, string password) {
        ////    var account = Context.WtAccounts
        ////                         .Where(m => m.email == email && m.password == password)
        ////                         .FirstOrDefault();
        ////    return account;
        ////}

        //public WtAccount FindByEmail(string email)
        //{
        //    var account = Context.WtAccounts
        //                         .Where(m => m.email == email)
        //                         .FirstOrDefault();
        //    if (account != null)
        //    {
        //        account.MaskPassword();
        //        ObjectReflectionHelper.TrimStrings(account);
        //    }
        //    return account;
        //}

        //public WtAccount FindByPwdResetKey(string pwd_reset_key)
        //{
        //    var account = Context.WtAccounts
        //                         .Where(m => m.pwd_reset_key == pwd_reset_key)
        //                         .FirstOrDefault();
        //    if (account != null)
        //    {
        //        account.MaskPassword();
        //        ObjectReflectionHelper.TrimStrings(account);
        //    }
        //    return account;
        //}

        ///// <summary>
        ///// Gets the list by c_num.
        ///// </summary>
        ///// <param name="c_num">The c_num.</param>
        ///// <returns>List&lt;WtAccount&gt;.</returns>
        //public List<WtAccount> GetListByCNum(string c_num)
        //{
        //    var list = Context.WtAccounts
        //                    .Where(a => a.c_num == c_num)
        //                    .ToList();

        //    list = list.Select(a => a.MaskPassword()).ToList();
        //    ObjectReflectionHelper.TrimStrings(list);
        //    return list;
        //}


        ///// <summary>
        ///// Creates the specified model.
        ///// </summary>
        ///// <param name="model">The model.</param>
        ///// <param name="lang">The language.</param>
        ///// <returns>WtAccount.</returns>
        ///// <exception cref="System.ArgumentException">Invalid data.;temp_id</exception>
        //public WtAccount Create(WtAccount model, string lang)
        //{
        //    model.last_person = "WEB";
        //    UpperCaseHelper.Apply(model);
        //    HankakuHelper.Apply(model);

        //    if (model.is_japan)
        //    {
        //        model.culture_name = "ja-JP";
        //        model.date_format = "yyyy/MM/dd";
        //    }
        //    else
        //    {
        //        model.culture_name = "en-US";
        //        model.date_format = "MM/dd/yyyy";
        //    }
        //    model.time_format = "HH:mm";

        //    //Tempテーブルよりメールアドレス取得
        //    var temp = Context.WtAccountTemps.Find(model.temp_id);
        //    if (temp == null)
        //    {
        //        throw new ArgumentException("Invalid data.", "temp_id");
        //    }
        //    model.email = temp.email;
        //    model.primary_id = temp.primary_id;
        //    model.create_date = RegionConfig.GetJapanNow();
        //    model.update_date = model.create_date;
        //    model.ValidateSave(true, null, lang);

        //    //生のパスワードをハッシュ化されたものに変更。
        //    EnsurePasswordIsHashed(model);
        //    this.Add(model);

        //    //Tempテーブルのアカウント登録日更新
        //    temp.account_regist_date = RegionConfig.GetJapanNow();
        //    temp.update_date = RegionConfig.GetJapanNow();
        //    var repository = new WtAccountTempRepository(this);
        //    repository.SetModified(temp);
        //    this.SaveChanges();

        //    //変更後の状態をDBから再取得。EFのキャッシュを使わない様にするためにAsNoTracking()が必要。
        //    var saved = Context.WtAccounts
        //                    .AsNoTracking()
        //                    .Where(a => a.account_id == model.account_id)
        //                    .SingleOrDefault();

        //    //変更後のオブジェクトを返す前に不要なプロパティの値をマスクする。
        //    if (saved != null) saved = saved.MaskPassword();
        //    ObjectReflectionHelper.TrimStrings(saved);
        //    return saved;
        //}

        ///// <summary>
        ///// Updates the specified model.
        ///// </summary>
        ///// <param name="model">The model.</param>
        ///// <param name="lang">The language.</param>
        ///// <param name="edit_prm">'Email' or 'Password'.</param>
        ///// <returns>WtAccount.</returns>
        //public WtAccount Update(WtAccount model, string lang, string edit_prm = null)
        //{

        //    var wtAccount = this.Find(model.account_id);
        //    if (wtAccount == null)
        //    {
        //        throw new KeyNotFoundException();
        //    }

        //    UpperCaseHelper.Apply(model);
        //    HankakuHelper.Apply(model);

        //    model.ValidateSave(false, edit_prm, lang);
        //    this.Update(model);

        //    if (edit_prm == "Email")
        //    {
        //        if (!string.IsNullOrEmpty(model.c_num))
        //        {
        //            //iw_personのEmailを更新。
        //            var japanCustomerRepo = new JapanCustomerRepository();
        //            japanCustomerRepo.UpdateEmail(model);

        //            //現地側jpn_infoテーブルのe_mail項目も変更
        //            var list = new WtBookingRepository().GetRegionListByCNum(model.c_num);
        //            foreach (var region_cd in list)
        //            {
        //                var jpnInfoRepo = new JpnInfoRepository(region_cd);
        //                jpnInfoRepo.UpdateEmail(model, null);
        //            }
        //        }
        //    }

        //    //変更後の状態をDBから再取得して返す。
        //    return Reload(model.account_id);
        //}

        //private void Update(WtAccount model)
        //{

        //    //生のパスワードをハッシュ化されたものに変更。
        //    EnsurePasswordIsHashed(model);

        //    using (var tran = Context.BeginTrans())
        //    {
        //        try
        //        {
        //            //passwordがnullでなければpasswordとpwd_hashを更新。
        //            //e_last_nameまたはe_first_nameがnullでなければe_last_nameとe_first_nameを更新。
        //            //emailがnullでなければemailを更新。
        //            string procname = "usp_wt_update_account";
        //            var prms = new SqlParamSet();

        //            prms.AddInt("@account_id", model.account_id);
        //            prms.AddChar("@last_person", 6, model.last_person);
        //            prms.AddVarChar("@password", 15, model.password);
        //            prms.AddVarChar("@pwd_hash", 100, model.pwd_hash);
        //            prms.AddVarChar("@e_last_name", 30, model.e_last_name);
        //            prms.AddVarChar("@e_first_name", 30, model.e_first_name);
        //            prms.AddVarChar("@email", 200, model.email);

        //            Context.ExecuteStoredProcedure(procname, prms.ToArray(), tran);

        //            tran.Commit();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw RollbackAndGetException(ex, tran);
        //        }
        //    }
        //}

        ////生のパスワードをハッシュ化されたものに変更。
        //private void EnsurePasswordIsHashed(WtAccount model)
        //{
        //    const string dummyPwd = "********";

        //    if (!string.IsNullOrEmpty(model.password) && model.password != dummyPwd)
        //    {
        //        model.pwd_hash = Crypto.HashPassword(model.password);
        //        model.password = dummyPwd;
        //    }
        //    else
        //    {
        //        model.pwd_hash = null;
        //        model.password = null;
        //    }
        //}

        ///// <summary>
        ///// Saves the customer information.
        ///// </summary>
        ///// <param name="accountInfo">The customer.</param>
        ///// <param name="last_person">The last_person.</param>
        //public WtAccount UpdateAccountInfo(WtAccountInfo accountInfo, IDbTransaction tr)
        //{

        //    UpperCaseHelper.Apply(accountInfo);
        //    HankakuHelper.Apply(accountInfo);

        //    ////customer, wt_accountそれぞれのModelオブジェクト作成
        //    //var wtAccount = this.Find(accountInfo.account_id);
        //    //if (wtAccount == null) {
        //    //    throw new KeyNotFoundException();
        //    //}

        //    bool need_commit = false;
        //    if (tr == null)
        //    {
        //        tr = Context.BeginTrans();
        //        need_commit = true;
        //    }
        //    try
        //    {
        //        var prms = new SqlParamSet();
        //        var sql =
        //            @" UPDATE [wt_account] SET 
        //                    e_last_name = @e_last_name, 
        //                    e_first_name = @e_first_name, 
        //                    is_groom = @is_groom,
        //                    email= @email,
        //                    c_num = @c_num,
        //                    update_date = dbo.fnc_getdate()
        //                WHERE (account_id = @account_id) ";
        //        prms.AddVarChar("@e_last_name", 20, accountInfo.last);
        //        prms.AddVarChar("@e_first_name", 20, accountInfo.first);
        //        prms.AddBit("@is_groom", accountInfo.is_groom);
        //        prms.AddVarChar("@email", 200, accountInfo.email);
        //        prms.AddChar("@c_num", 7, accountInfo.c_num);
        //        prms.AddInt("@account_id", accountInfo.account_id);

        //        Context.ExecuteSQL(sql, prms.ToArray(), tr);
        //        if (need_commit) tr.Commit();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw RollbackAndGetException(ex, tr, need_commit);
        //    }

        //    //変更後の状態をDBから再取得して返す。
        //    return Reload(accountInfo.account_id);
        //}

        //private WtAccount Reload(int account_id)
        //{
        //    //変更後の状態をDBから再取得。EFのキャッシュを使わない様にするためにAsNoTracking()が必要。
        //    var model = Context.WtAccounts
        //                    .AsNoTracking()
        //                    .Where(a => a.account_id == account_id)
        //                    .SingleOrDefault();

        //    //セキュリティ上不要なプロパティの値をマスクする。
        //    if (model != null) model = model.MaskPassword();
        //    ObjectReflectionHelper.TrimStrings(model);
        //    return model;
        //}

        ///// <summary>
        ///// Checks for reregist.
        ///// </summary>
        ///// <param name="model">The model.</param>
        ///// <param name="lang">The language.</param>
        ///// <returns>WtAccount.</returns>
        //public WtAccount CheckForReregist(WtAccount model, string lang)
        //{

        //    UpperCaseHelper.Apply(model);
        //    HankakuHelper.Apply(model);

        //    model.ValidateForReregist(lang);
        //    model = this.Context.WtAccounts.Where(m => m.email == model.email).SingleOrDefault();
        //    model.pwd_reset_key = CreateUrlKey();
        //    UpdatePwdResetKey(model, null);
        //    ObjectReflectionHelper.TrimStrings(model);
        //    return model;
        //}

        //private string CreateUrlKey()
        //{

        //    var created = false;
        //    var url_key = "";

        //    List<char> list = new List<char>() { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
        //        'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
        //    while (!created)
        //    {
        //        Random rnd = new Random();
        //        url_key = new string(Enumerable.Range(0, 64).Select(n =>
        //            list[rnd.Next(list.Count)]).ToArray());
        //        //すでに同じkeyが存在しないかチェック
        //        var exist = Context.WtAccounts.Any(m => m.pwd_reset_key == url_key);
        //        if (!exist)
        //        {
        //            created = true;
        //            return url_key;
        //        }
        //    }
        //    return url_key;
        //}

        ///// <summary>
        ///// Updates the password reset key.
        ///// </summary>
        ///// <param name="model">The model.</param>
        ///// <param name="tr">The tr.</param>
        //private void UpdatePwdResetKey(WtAccount model, IDbTransaction tr)
        //{

        //    bool need_commit = false;
        //    if (tr == null)
        //    {
        //        tr = Context.BeginTrans();
        //        need_commit = true;
        //    }
        //    try
        //    {
        //        var prms = new SqlParamSet();
        //        var sql = @" UPDATE [wt_account] 
        //                    SET update_date = dbo.fnc_getdate(), pwd_reset_key = @pwd_reset_key
        //                    WHERE (account_id = @account_id) ";

        //        prms.AddVarChar("@pwd_reset_key", 80, model.pwd_reset_key);
        //        prms.AddInt("@account_id", model.account_id);
        //        Context.ExecuteSQL(sql, prms.ToArray(), tr);
        //        if (need_commit)
        //        {
        //            tr.Commit();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (need_commit)
        //        {
        //            RollbackAndGetException(ex, tr);
        //        }
        //        throw Common.GetMostInnerException(ex);
        //    }
        //    finally
        //    {
        //        if (need_commit && tr.Connection != null && tr.Connection.State != ConnectionState.Closed)
        //        {
        //            tr.Connection.Close();
        //        }
        //    }
        //}

        ///// <summary>
        ///// Updates the password.
        ///// </summary>
        ///// <param name="model">The model.</param>
        ///// <param name="tr">The tr.</param>
        //private void UpdatePwd(WtAccount model, IDbTransaction tr)
        //{

        //    bool need_commit = false;
        //    if (tr == null)
        //    {
        //        tr = Context.BeginTrans();
        //        need_commit = true;
        //    }
        //    try
        //    {
        //        var prms = new SqlParamSet();
        //        var sql = @" UPDATE [wt_account] 
        //                    SET update_date = dbo.fnc_getdate(), password = @password, pwd_hash = @pwd_hash, pwd_reset_key = @pwd_reset_key
        //                    WHERE (account_id = @account_id) ";

        //        prms.AddVarChar("@password", 15, model.password);
        //        prms.AddVarChar("@pwd_hash", 100, model.pwd_hash);
        //        prms.AddVarChar("@pwd_reset_key", 80, null);
        //        prms.AddInt("@account_id", model.account_id);
        //        Context.ExecuteSQL(sql, prms.ToArray(), tr);
        //        if (need_commit)
        //        {
        //            tr.Commit();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw RollbackAndGetException(ex, tr, need_commit);
        //    }
        //    finally
        //    {
        //        if (need_commit && tr.Connection != null && tr.Connection.State != ConnectionState.Closed)
        //        {
        //            tr.Connection.Close();
        //        }
        //    }
        //}

        ///// <summary>
        ///// Updates the password.
        ///// </summary>
        ///// <param name="model">The model.</param>
        ///// <param name="lang">The language.</param>
        ///// <returns>WtAccount.</returns>
        //public WtAccount UpdatePassword(WtAccount model, string lang)
        //{

        //    UpperCaseHelper.Apply(model);
        //    HankakuHelper.Apply(model);

        //    model.ValidateForResetPassword(lang);
        //    //生のパスワードをハッシュ化されたものに変更。
        //    EnsurePasswordIsHashed(model);

        //    var account = this.GetList()
        //        .Where(m => m.pwd_reset_key == model.pwd_reset_key
        //            && m.e_last_name == model.e_last_name
        //            && m.e_first_name == model.e_first_name)
        //        .SingleOrDefault();
        //    if (account == null)
        //    {
        //        throw new KeyNotFoundException();
        //    }
        //    model.account_id = account.account_id;
        //    UpdatePwd(model, null);

        //    //変更後の状態をDBから再取得して返す。
        //    return Reload(model.account_id);
        //}


        ///// <summary>
        ///// Releases unmanaged and - optionally - managed resources.
        ///// </summary>
        ///// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        // ここにManaged resourceの解放処理を書く。
        //        if (_context != null)
        //        {
        //            _context.Dispose();
        //            _context = null;
        //        }
        //    }
        //}

        /////// <summary>
        /////// Selects the by identifier.
        /////// </summary>
        /////// <param name="account_id">The account_id.</param>
        /////// <returns>WtAccount.</returns>
        ////private WtAccount SelectById(int account_id) {
        ////    var sql = @"SELECT * FROM wt_account WHERE (account_id = @account_id) ";
        ////    var prms = new SqlParamSet();
        ////    prms.AddInt("@account_id", account_id);
        ////    var account = Context.Database.SqlQuery<WtAccount>(sql, prms.ToArray()).SingleOrDefault();
        ////    ObjectReflectionHelper.TrimStrings(account);
        ////    return account;
        ////}

        ////ALL側WtAccountの氏名を更新。(同一のc_numで新郎・新婦、その他複数のレコードが存在する可能性があるのでループ処理。)
        ///// <summary>
        ///// Updates the account names.
        ///// </summary>
        ///// <param name="customer">The customer.</param>
        ///// <param name="is_japan">if set to <c>true</c> [is_japan].</param>
        //public void UpdateAccountNames(Customer customer, bool is_japan)
        //{
        //    var accounts = this.GetListByCNum(customer.c_num);
        //    foreach (var a in accounts)
        //    {
        //        if (a.is_groom)
        //        {
        //            a.e_last_name = customer.g_last;
        //            a.e_first_name = customer.g_first;
        //        }
        //        else
        //        {
        //            a.e_last_name = customer.b_last;
        //            a.e_first_name = customer.b_first;
        //        }
        //        this.Update(a, is_japan ? "J" : "E", null);
        //    }
        //}


    }
}
