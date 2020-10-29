using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using AutoMapper;
using CBAF;
using MauloaDemo.Repository;
using MauloaDemo.Models;
using MauloaDemo.Web.ViewModels;


namespace MauloaDemo.Web.Controllers {

    /// <summary>
    /// ログイン処理用コントローラ
    /// このコントローラだけは例外的にBaseControllerを継承しない。
    /// </summary>
    //[LoggingFilter]
    [UserCultureFilter]
    [JsonDateFormatFilter]
    public class AccountController : Controller {

        private MauloaDemo.Web.ViewModels.RegionInfo _currentRegion;

        public AccountController() {
            Mapper.CreateMap<LoginUser, UserProfile>()
                    .ForMember(dest => dest.cur_password, opt => opt.Ignore())
                    .ForMember(dest => dest.new_password, opt => opt.Ignore())
                    .ForMember(dest => dest.new_password_conf, opt => opt.Ignore());
            Mapper.AssertConfigurationIsValid();
        }

        public MauloaDemo.Web.ViewModels.RegionInfo CurrentRegionInfo {
            get {
                return _currentRegion;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {

            //RegionInfoをセッションからインスタンス変数にセット。(無ければ生成してセット。)
            _currentRegion = Session[Constants.SSKEY_CURRENT_REGION] as MauloaDemo.Web.ViewModels.RegionInfo;
            if (_currentRegion == null) {
                _currentRegion = new MauloaDemo.Web.ViewModels.RegionInfo();
                _currentRegion.CurrentDestination = RegionConfig.DEFAULT_REGION;
                Session[Constants.SSKEY_CURRENT_REGION] = _currentRegion;
            }

            if (!String.IsNullOrWhiteSpace(_currentRegion.CurrentDestination)) {
                //現地のUTCからの時差
                _currentRegion.TimeDiffFromUTC = RegionConfig.GetRegionTimeDiffFromUTC(_currentRegion.CurrentDestination);

                //現地通貨の書式および通貨記号
                _currentRegion.CurrencyFormat = RegionConfig.GetCurrencyFormat(_currentRegion.CurrentDestination);
                _currentRegion.CurrencySymbol = RegionConfig.GetCurrencySymbol(_currentRegion.CurrentDestination);
                _currentRegion.CurrencyFormatWithSymbol = RegionConfig.GetCurrencyFormatWithSymbol(_currentRegion.CurrentDestination);
            }

            //RegionInfoをViewBagに格納。(Viewから使いやすい様に。)
            ViewBag.CurrentRegionInfo = _currentRegion;

            base.OnActionExecuting(filterContext);
        }

        //ModelStateから全てのエラーメッセージを結合して取得する。
        protected string GetModelStateErrors(string separator = "\n") {
            return string.Join(separator, ModelState.SelectMany(p => p.Value.Errors).Select(p => p.ErrorMessage));
        }


        //
        // GET: /Account/

        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AccountLogin model, string returnUrl, string rgn) {
            if (!ModelState.IsValid) {
                return ReturnInvalid(model);
            }

            var repo = new LoginUserRepository();
            string error_msg = null;
            var loginUser = repo.ValidateLogin(model.login_id, model.password, out error_msg);
            if (loginUser == null) {
                //Keyは意図的にlogin_idにしている。攻撃者にlogin_idとpasswordのどちらが間違っているのかの情報を与えない様に。
                ModelState.AddModelError("login_id", error_msg ?? "Invalid login.");       
                return ReturnInvalid(model);
            }

            //ログイン成功
            Session[Constants.SSKEY_LOGIN_USER] = loginUser;
            FormsAuthentication.SetAuthCookie(loginUser.login_id, false);

            //DBにログを保存。
            try {
                var actionRepo = new MauloaDemo.Repository.LogActionRepository(repo);
                actionRepo.Login(loginUser);
            } catch (Exception) {
                //Ignore errors.
            }

            if (String.IsNullOrEmpty(returnUrl) || returnUrl.ToLower().Contains("/account/login")) {
                returnUrl = FormsAuthentication.DefaultUrl;
            }

            return Redirect(returnUrl);
        }


        private ActionResult ReturnInvalid(AccountLogin model) {

            //クラッキング防止の為時間を空ける。
            System.Threading.Thread.Sleep(3000);

            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Logout() {
            LoginUser loginUser = UserHelper.LoginUser;

            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();

            if (loginUser != null) {
                try {
                    if (loginUser.Token != null) {
                        //トークンを削除。
                        var loginUserRepo = new LoginUserRepository();
                        loginUserRepo.DeleteToken(loginUser.Token);
                    }

                    //ログを保存。
                    var actionRepo = new MauloaDemo.Repository.LogActionRepository();
                    actionRepo.Logout(loginUser);
                } catch (Exception) {
                    //Ignore errors.
                }
            }

            return Redirect(FormsAuthentication.DefaultUrl);
        }

        [HttpGet]
        [ActionName("Profile")]
        [Authorize]
        public ActionResult UserProfile() {
            if (UserHelper.LoginUser == null) {
                return HttpNotFound();
            }

            //Access Levelが1以下のユーザーはProfile画面を開けない様にする。
            if (UserHelper.LoginUser.access_level <= 1) {
                return HttpNotFound();
            }

            var login_id = UserHelper.LoginUser.login_id;
            var repository = new LoginUserRepository();
            var user = repository.Find(login_id);
            if (user == null) {
                return HttpNotFound();
            }
            var userProfile = Mapper.Map<LoginUser, UserProfile>(user);
            ViewBag.CultureNameList = Culture.GetCultureList().ToList();
            ViewBag.DateFormatList = Culture.GetDateFormats().ToList();

            ViewBag.Tab = TypeHelper.GetStrTrim(Request.QueryString["tab"]);

            return View("UserProfile", userProfile);
        }

        [HttpPost]
        [Authorize]
        [AjaxUserSessionFilter]
        public JsonResult AfterUpdateProfile(UserProfile userProfile) {
            var sessionUser = UserHelper.LoginUser;
            if (sessionUser == null) {
                return Json(new { result = "error" });
            }
            var repository = new LoginUserRepository();
            var user = repository.Find(sessionUser.login_id);
            if (user == null) {
                return Json(new { result = "error" });
            }

            //セッションの情報を更新。
            UserHelper.LoginUser = user;
            
            return Json(new { result = "ok" });
        }


        [AjaxUserSessionFilter]
        public JsonResult GetDateFormatList() {
            var dateFormats = Culture.GetDateFormats();
            return Json(dateFormats.ToList(), JsonRequestBehavior.AllowGet);
        }

        [AjaxUserSessionFilter]
        public JsonResult GetTimeFormatList() {
            var timeFormats = Culture.GetTimeFormats();
            return Json(timeFormats.ToList(), JsonRequestBehavior.AllowGet);
        }


        //[HttpPost]
        //[Authorize]
        //[AjaxUserSessionFilter]
        //public ActionResult UpdatePassword(UserProfile userProfile) {
        //    var hash = new Dictionary<string, object>();

        //    try {
        //        if (userProfile == null) {
        //            throw new Exception(this.Resource("MSG_ERROR_UNKNOWN"));
        //        }
        //        if (string.IsNullOrEmpty(userProfile.cur_password)){
        //            throw new Exception(this.Resource("MSG_PWDCHG_CURPWDBLANKERR"));
        //        }
        //        if (string.IsNullOrEmpty(userProfile.new_password)) {
        //            throw new Exception(this.Resource("MSG_PWDCHG_NEWPWDBLANKERR"));
        //        }

        //        if (!string.Equals(userProfile.new_password, userProfile.new_password_conf)) {
        //            throw new Exception(this.Resource("MSG_PWDCHG_NEWCONF"));
        //        }

        //        var repository = new LoginUserRepository();
        //        var result = repository.ChangePassword(
        //                                    userProfile.login_id, 
        //                                    userProfile.cur_password,
        //                                    userProfile.new_password,
        //                                    UserHelper.LoginUser.login_id);

        //        if (result != LoginUser.PwdChgResult.Ok) {
        //            throw new Exception(getChangePasswordErrMsg(result));
        //        }

        //        //セッションのログインユーザー情報を更新。
        //        var loginUser = repository.Find(UserHelper.LoginUser.login_id);
        //        Session[Constants.SSKEY_LOGIN_USER] = loginUser;

        //        hash.Add("Result", "success");
        //        hash.Add("EffToPass", loginUser.eff_to_pass);
        //        hash.Add("Message", this.Resource("MSG_OK_SAVED"));
        //        return Json(hash);
        //    }
        //    catch (Exception ex) {
        //        hash.Add("Result", "failure");
        //        hash.Add("Message", ex.Message);
        //    }
        //    return Json(hash, JsonRequestBehavior.AllowGet);

        //}


        //private string getChangePasswordErrMsg(LoginUser.PwdChgResult result) {

        //    ////文字列からEnum型に変換。
        //    //if (!Enum.TryParse<LoginUser.PwdChgResult>(result, true, out chgRes)) {
        //    //    //変換出来ない場合は文字列をそのまま返す。
        //    //    return msg;
        //    //}

        //    var msg = string.Empty;
        //    switch (result) { 
        //        case LoginUser.PwdChgResult.LengthErr:
        //            var minLength = TypeHelper.GetInt(ConfigurationManager.AppSettings["PasswordLengthMin"]);
        //            var maxLength = TypeHelper.GetInt(ConfigurationManager.AppSettings["PasswordLengthMax"]);
        //            msg = this.Resource("MSG_PWDCHG_LENGTHERR", minLength, maxLength);
        //            break;
        //        default:
        //            msg = this.Resource("MSG_PWDCHG_" + Enum.GetName(typeof(LoginUser.PwdChgResult), result).ToUpper());
        //            if (string.IsNullOrEmpty(msg)) {
        //                msg = string.Format("Error. ({0})", result);
        //            }
        //            break;
        //    }

        //    return msg;
        //}


    }
}