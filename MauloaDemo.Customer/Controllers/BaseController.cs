using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MauloaDemo.Customer.ViewModels;
using MauloaDemo.Models;
using log4net;
using MauloaDemo.Utilities;
using MauloaDemo.Repository;
//using WtbApi.Proxy.All;

namespace MauloaDemo.Customer.Controllers {

    [MyRequireHttps] 
    [JsonDateFormatFilter(Order=90)]           //JsonResultを返す時にサーバーとブラウザのTimeZoneが異なると日時がずれる問題の対処。
    public class BaseController : Controller {
        private RegionInfo _currentRegion;
        private CAccount _loginUser;
        private CurrentItemInfo _selectItem;

        private static ILog log = LogManager.GetLogger(typeof(MvcApplication));

        public RegionInfo CurrentRegionInfo
        {
            get
            {
                return _currentRegion;
            }
        }

        public CAccount CurrentLoginUser
        {
            get
            {
                return _loginUser;
            }
        }

        public CurrentItemInfo CurrentSelectItem
        {
            get
            {
                return _selectItem;
            }
        }

        //********************************************************* 
        //******* ASP.NET Indentity 2.0 対応 
        //********************************************************* 
        private ApplicationUserManager _userManager;

        public BaseController()
        {
        }

        public BaseController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            protected set
            {
                _userManager = value;
            }
        }

        protected IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        //********************************************************* 
        //******* ↑↑↑ ASP.NET Indentity 2.0 対応 ↑↑↑ ここまで
        //********************************************************* 



        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            //UserCultureFilter属性だとOnActionExecutingのタイミングでしかCultureをセット出来ない。
            //ModelBindingはその前に実行されるので、間に合わない。(en-AUの場合にModelの日付のバリデーションが正しく動かない。)
            //そのため BaseControllerのInitializeメソッドの中でやる事に変更。
            //if (Session != null) {
            //    //セッションからログインユーザー情報を取得。
            //    Employee user = Session[Constants.SSKEY_LOGIN_USER] as Employee;
            //    if (user != null) {
            //        //ユーザーの言語・カルチャー設定をスレッド全体に反映する。(これによってActiveReportsにも反映される。)
            //        System.Threading.Thread.CurrentThread.CurrentCulture
            //            = System.Globalization.CultureInfo.CreateSpecificCulture(user.culture_name);

            //        System.Threading.Thread.CurrentThread.CurrentUICulture
            //            = System.Globalization.CultureInfo.CreateSpecificCulture(user.culture_name);
            //    }
            //}

        }

        private static string GetMaintenanceFilePath()
        {
            string path = ConfigurationManager.AppSettings["MaintenanceFilePath"];
            path = string.IsNullOrWhiteSpace(path) ? String.Empty : path;

            if (path.Contains("%ROOT%"))
            {
                path = path.Replace("%ROOT%", RegionConfig.GetApplicationRootPath());
            }

            if (String.Empty.Equals(path))
            {
                path = RegionConfig.GetApplicationRootPath();
            }

            if (!path.EndsWith("\\"))
            {
                path += "\\";
            }
            return path;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (!filterContext.IsChildAction)
            {
                //メンテナンス中の場合、メンテナンス中画面に遷移
                var maintenance_file_name = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["MaintenanceFileName"]);
                if (System.IO.File.Exists(GetMaintenanceFilePath() + maintenance_file_name))
                {
                    var url = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["BaseURL"]);
                    if (!url.EndsWith("/")) url += "/";
                    url += maintenance_file_name;
                    filterContext.Result = RedirectToLocal(url);
                    return;
                }
            }

            if (User != null && User.Identity != null && User.Identity.IsAuthenticated)
            {
                //LoginUserをセッションからインスタンス変数にセット。
                _loginUser = Session[Constants.SSKEY_LOGIN_USER] as CAccount;
                if (_loginUser == null)
                {
                    //セッションから取得出来ない場合はDBから読み込んでセッションにセット。
                    var userEmail = User.Identity.GetUserId();
                    var repo = new CAccountRepository();
                    _loginUser = repo.FindById(userEmail);
                    if (_loginUser == null)
                    {
                        AuthenticationManager.SignOut();
                    }
                    Session[Constants.SSKEY_LOGIN_USER] = _loginUser;
                }
            }

            //RegionInfoをセッションからインスタンス変数にセット。(無ければ生成してセット。)
            if (Session[Constants.SSKEY_CURRENT_REGION] != null && Session[Constants.SSKEY_CURRENT_REGION] is RegionInfo)
            {
                _currentRegion = Session[Constants.SSKEY_CURRENT_REGION] as RegionInfo;
            }
            if (_currentRegion == null)
            {
                _currentRegion = new RegionInfo();
                _currentRegion.CurrentDestination = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["DefaultRegion"]);
            }
            Session[Constants.SSKEY_CURRENT_REGION] = _currentRegion;
            SetCurrentRegionInfo();

            //表示言語がセッションから取得出来ない場合はCookieから取得する。
            if (String.IsNullOrEmpty(_currentRegion.CurrentLanguage))
            {
                var cookie = Request.Cookies.Get(Constants.COOKIE_LANG);
                //Cookieがセットされており、かつ値が'J'または'E'のいずれかであればそれを保持する。J,E以外の不正な値は無視する。
                if (cookie != null && !string.IsNullOrEmpty(cookie.Value)
                        && "JE".Contains(cookie.Value) && cookie.Value.Length == 1)
                {
                    _currentRegion.CurrentLanguage = cookie.Value;
                }
            }

            //表示言語がSessionにもCookieにも設定されていない場合はブラウザの言語設定(Accept-Languageヘッダ)を反映する。
            //  1. 言語指定が一つも無い場合は日本語。（検索エンジンのロボットなど）
            //  2. 'ja*'があれば日本語。
            //  3. 上記以外の場合は英語。
            if (String.IsNullOrEmpty(_currentRegion.CurrentLanguage))
            {
                var is_japan = (Request.UserLanguages == null)
                                || Request.UserLanguages.Any(i => i.StartsWith("ja"));
                _currentRegion.CurrentLanguage = is_japan ? Constants.LANGUAGEKEY_JAPANESE : Constants.LANGUAGEKEY_ENGLISH;
            }

            //selectItemをセッションからインスタンス変数にセット。
            if (Session[Constants.SSKEY_SELECT_ITEM] != null && Session[Constants.SSKEY_SELECT_ITEM] is CurrentItemInfo)
            {
                _selectItem = Session[Constants.SSKEY_SELECT_ITEM] as CurrentItemInfo;
            }
            if (_selectItem == null)
            {
                _selectItem = new CurrentItemInfo();
                //                _selectItem.area_cd = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["DefaultArea"]);
                //                this.CurrentSelectItem.wed_date = RegionConfig.GetRegionToday(_currentRegion.CurrentDestination).AddMonths(2);
                this.CurrentSelectItem.wed_date = RegionConfig.GetRegionToday(_currentRegion.CurrentDestination).AddMonths(2);
            }
            Session[Constants.SSKEY_SELECT_ITEM] = _selectItem;

            ////LoginUser, RegionInfoをViewBagに格納。(Viewから使いやすい様に。)
            //ViewBag.CurrentLoginUser = _loginUser;
            //ViewBag.CurrentRegionInfo = _currentRegion;
            //ViewBag.CurrentSelectItem = _selectItem;
            //ViewBag.ClientIP = this.GetClientIPAddress();

            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            //LoginUser, RegionInfoをViewBagに格納。(Viewから使いやすい様に。)
            ViewBag.CurrentLoginUser = _loginUser;
            ViewBag.CurrentRegionInfo = _currentRegion;
            ViewBag.CurrentSelectItem = _selectItem;
            ViewBag.ClientIP = this.GetClientIPAddress();

            base.OnActionExecuted(filterContext);
        }

        //Cookieに言語設定を保存する。
        //(LocalStorageだと１回めのリクエストと同時に送る事が出来ないので、この用途にはCookieの方が良い。)
        protected void SetLanguageCookie(string language)
        {
            var cookie = new HttpCookie(Constants.COOKIE_LANG);
            cookie.Value = language;
            cookie.Expires = DateTime.Today.AddYears(20);
            Response.Cookies.Add(cookie);
        }

        //CurrentRegionInfoを設定する。
        protected void SetCurrentRegionInfo()
        {
            if (_currentRegion != null && !string.IsNullOrEmpty(_currentRegion.CurrentDestination))
            {
                //現地のUTCからの時差
                _currentRegion.TimeDiffFromUTC = RegionConfig.GetRegionTimeDiffFromUTC(_currentRegion.CurrentDestination);

                //現地通貨の書式および通貨記号
                _currentRegion.CurrencyFormat = RegionConfig.GetCurrencyFormat(_currentRegion.CurrentDestination);
                _currentRegion.CurrencySymbol = RegionConfig.GetCurrencySymbol(_currentRegion.CurrentDestination);
                _currentRegion.CurrencyFormatWithSymbol = RegionConfig.GetCurrencyFormatWithSymbol(_currentRegion.CurrentDestination);
            }
        }

        //決済サーバメンテナンス時に決済処理関連ボタンをDisabledにする
        protected void SetPaymentMaintInf()
        {
            //決済処理メンテナンス期間取得
            var maintenance_from = TypeHelper.GetDateTime(ConfigurationManager.AppSettings["PaymentMaintenanceFrom"]);
            var maintenance_to = TypeHelper.GetDateTime(ConfigurationManager.AppSettings["PaymentMaintenanceTo"]);
            var today = RegionConfig.GetRegionToday(_currentRegion.CurrentDestination);
            if (maintenance_from <= today && today <= maintenance_to)
            {
                ViewBag.PaymentMaintenanceFrom = maintenance_from;
                ViewBag.PaymentMaintenanceTo = maintenance_to;
            }
        }

        //ModelStateから全てのエラーメッセージを結合して取得する。
        protected string GetModelStateErrors(string separator = "\n")
        {
            return string.Join(separator, ModelState.SelectMany(p => p.Value.Errors).Select(p => p.ErrorMessage));
        }

        public bool IsJPN()
        {
            return this.CurrentRegionInfo.CurrentLanguage == Constants.LANGUAGEKEY_JAPANESE;
        }

        public string L(string str_jpn, string str_eng)
        {
            return this.IsJPN() ? str_jpn : str_eng;
        }

        protected ActionResult RedirectToLocal(string returnUrl)
        {
            var useHttps = TypeHelper.GetBool(ConfigurationManager.AppSettings["UseHttps"]);
            if (useHttps) returnUrl = returnUrl.ToLower().Replace("http://", "https://");

            returnUrl = TypeHelper.GetStr(returnUrl);       //nullの可能性を排除する。
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            var baseUrl = TypeHelper.GetStr(ConfigurationManager.AppSettings["BaseURL"]);
            if (useHttps) baseUrl = baseUrl.ToLower().Replace("http://", "https://");

            if (string.IsNullOrEmpty(baseUrl))
            {
                if (returnUrl.StartsWith("http://localhost") || returnUrl.StartsWith("https://localhost"))
                {
                    return Redirect(returnUrl);
                }
            }
            else if (returnUrl.StartsWith(baseUrl))
            {
                return Redirect(returnUrl);
            }

            //自サイト以外へのリダイレクトは強制的にトップページへのリダイレクトに変更。
            return RedirectToAction("Index", "Home");
        }

        protected string GetXForwardedForAddress()
        {
            var strAddr = "";
            foreach (var key in Request.Headers.AllKeys)
            {
                if ("X-Forwarded-For".ToUpper().Equals(key.ToUpper()))
                {
                    strAddr = Request.Headers[key];

                    if (!string.IsNullOrEmpty(strAddr))
                    {
                        if (strAddr.Contains(":"))
                        {
                            strAddr = strAddr.Substring(0, strAddr.IndexOf(":"));
                        }
                    }

                    return strAddr;
                }
            }
            return string.Empty;
        }

        protected string GetClientIPAddress()
        {
            var clientAddress = GetXForwardedForAddress();
            if (string.IsNullOrEmpty(clientAddress))
            {
                clientAddress = Request.UserHostAddress;
            }
            return clientAddress;
        }

        protected bool IsJapaneseClient()
        {
            //ローカル環境でのデバッグ中は下のgetCountry()でエラーになるのでtrueを返す。
            if (Request.Url.ToString().Contains("localhost")) return true;

            var checkIp = TypeHelper.GetBool(ConfigurationManager.AppSettings["GetGlobalIp"]);
            if (!checkIp) return true;          //常に日本からのアクセスとする

            try
            {
                //クライアントIPアドレス取得
                var ipa = GetClientIPAddress();

                //クライアントのIPアドレスが取得できない場合はtrueを返す。
                if (string.IsNullOrEmpty(ipa)) return true;

                var path = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["IpDataPath"]);
                var ls = new LookupService(path, LookupService.GEOIP_MEMORY_CACHE);       //open the database
                var c = ls.getCountry(ipa);     //get country of the ip address
                var countryCode = c.getCode();

                //国コードが'JP'もしくは不明の場合にtrueを返す。
                return (countryCode == "JP" || countryCode == "--");

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            return true;
        }

        protected bool IsJpnClientOrJpnAccount()
        {
            if (this.CurrentLoginUser == null)
            {
                //未ログインの場合はIPアドレスから所在地が日本国内かどうかを判断。
                return this.IsJapaneseClient();
            }
            else
            {
                //ログイン済の場合はアカウント作成場所が日本国内かどうかを判断。
                return this.CurrentLoginUser.is_japan;
            }
        }

    }
}