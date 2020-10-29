using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Globalization;
using MauloaDemo.Repository;
using MauloaDemo.Models;
using MauloaDemo.Web.ViewModels;


namespace MauloaDemo.Web.Controllers {

    //[Authorize(Order = 90)]           //←これはUserSessionFilter属性でやっているので不要。
    //[UserCultureFilter(Order = 30)]   //←これだとOnActionExecutingのタイミングでしかCultureをセット出来ない。ModelBindingはその前に実行されるので、間に合わない。--> BaseControllerのInitializeメソッドの中でやる事に変更。
    [LoggingFilter(Order = 10)]
    [UserSessionFilter(Order = 20)]
    [AjaxUserSessionFilter(Order = 30)]
    [JsonDateFormatFilter(Order=90)]           //JsonResultを返す時にサーバーとブラウザのTimeZoneが異なると日時がずれる問題の対処。
    public class BaseController : Controller {
        private MauloaDemo.Web.ViewModels.RegionInfo _currentRegion;
        private LoginUser _loginUser;

        public MauloaDemo.Web.ViewModels.RegionInfo CurrentRegionInfo {
            get {
                return _currentRegion;
            }
        }

        public LoginUser CurrentLoginUser {
            get {
                return _loginUser;
            }
        }

        //UserCultureFilter属性だとOnActionExecutingのタイミングでしかCultureをセット出来ない。
        //ModelBindingはその前に実行されるので、間に合わない。(en-AUの場合にModelの日付のバリデーションが正しく動かない。)
        //そのため BaseControllerのInitializeメソッドの中でやる事に変更。
        protected override void Initialize(System.Web.Routing.RequestContext requestContext) {
            base.Initialize(requestContext);

            if (Session == null) return;

            //セッションからログインユーザー情報を取得。
            _loginUser = Session[Constants.SSKEY_LOGIN_USER] as LoginUser;
            if (_loginUser != null) {
                //ユーザーの言語・カルチャー設定をスレッド全体に反映する。(これによってActiveReportsにも反映される。)
                var ci = CultureInfo.CreateSpecificCulture(_loginUser.culture_name);
                Thread.CurrentThread.CurrentCulture = ci;
                Thread.CurrentThread.CurrentUICulture = ci;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext) {
            
            //LoginUserをセッションからインスタンス変数にセット。
            _loginUser = Session[Constants.SSKEY_LOGIN_USER] as LoginUser;

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

            //LoginUser, RegionInfoをViewBagに格納。(Viewから使いやすい様に。)
            ViewBag.CurrentLoginUser = _loginUser;
            ViewBag.CurrentRegionInfo = _currentRegion;

            base.OnActionExecuting(filterContext);
        }

        //ModelStateから全てのエラーメッセージを結合して取得する。
        protected string GetModelStateErrors(string separator = "\n") {
            return string.Join(separator, ModelState.SelectMany(p => p.Value.Errors).Select(p => p.ErrorMessage));
        }

    }
}