using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using MauloaDemo.Models;
using MauloaDemo.Repository;
using MauloaDemo.Customer.ViewModels;


namespace MauloaDemo.Customer {

    /// <summary>
    /// コントローラの全てのメソッドまたは特定のメソッドについて、実行可能な配置場所を制限する為の属性。
    /// もし現在のサーバーが実行可能な場所ではない場合は指定されたサーバーにリダイレクトする。
    /// 例1）特定のコントローラを日本のサーバーでのみ実行可能にする。(JapanOnly)
    /// 例2）特定のコントローラを現地側のサーバーでのみ実行可能にする。(DestinationsOnly)
    /// 例3）特定のコントローラを例えば「沖縄」など特定の地域でのみ実行可能にする。(SpecificRegion + 第２引数に地域コードを指定)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RegionFilterAttribute : ActionFilterAttribute {

        public enum RegionRestriction {
            JapanOnly,
            DestinationsOnly,
            SpecificRegion
        }

        private RegionRestriction _regionRestriction;
        private string _region_cd;

        public RegionFilterAttribute(
                                RegionRestriction regionRestriction = RegionRestriction.DestinationsOnly,
                                string region_cd = null) {
            _regionRestriction = regionRestriction;
            _region_cd = region_cd;

            if (_regionRestriction == RegionRestriction.SpecificRegion && String.IsNullOrEmpty(_region_cd)) {
                throw new InvalidOperationException("RegionFilterAttribute: region_cd is not specified. ");
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext) {
            if (filterContext.IsChildAction) return;
            if (filterContext.RequestContext == null) return;
            if (filterContext.RequestContext.HttpContext == null) return;
            if (filterContext.RequestContext.HttpContext.Session == null) return;

            ////Actionメソッドに[AjaxUserSessionFilter]属性が付いている場合はRegionFilter属性は無視する。
            //bool skip = filterContext.ActionDescriptor.IsDefined(typeof(AjaxUserSessionFilterAttribute), inherit: true);
            //Ajaxリクエストの場合はRegionFilter属性は無視する。
            bool skip = filterContext.HttpContext.Request.IsAjaxRequest();
            if (skip) return;

            RegionInfo currentRegion = filterContext.RequestContext.HttpContext.Session[Constants.SSKEY_CURRENT_REGION] as RegionInfo;
            if (currentRegion == null) return;

            string currentServerLocation = RegionConfig.GetServerLocation();

            //if (_regionRestriction == RegionRestriction.JapanOnly) {
            //    if (!RegionConfig.REGION_ALL.Equals(currentServerLocation)) {

            //        //現地側Webサーバーで動いている場合は日本側へ飛ばす。
            //        SetRedirectResult(RegionConfig.REGION_ALL, filterContext);
            //        return;
            //    }
            //    else {

            //        //日本側サーバーで動いている場合は単にcurrentLocationをALLにする。
            //        //if (!RegionConfig.REGION_ALL.Equals(currentRegion.CurrentLocation)) {
            //        //  currentRegion.CurrentLocation = RegionConfig.REGION_ALL;
            //        //}
            //    }
            //}

            //    if (_regionRestriction == RegionRestriction.DestinationsOnly
            //            && RegionConfig.REGION_ALL.Equals(currentRegion.CurrentLocation)
            //            && String.IsNullOrEmpty(currentRegion.CurrentDestination)) {
            //        string base_url = RegionConfig.GetRegionAttr(RegionConfig.REGION_ALL, "base_url");
            //        if (base_url.EndsWith("/")) base_url = base_url.TrimEnd("/".ToCharArray());

            //        string to_url = base_url + "/Home/SelectRegion";
            //        to_url += "?ReturnUrl=" + filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery;

            //        filterContext.Result = new RedirectResult(to_url);
            //        return;
            //    }

            //    if (_regionRestriction == RegionRestriction.SpecificRegion
            //            && !String.IsNullOrEmpty(_region_cd)
            //            && _region_cd.Equals(currentRegion.CurrentLocation)) {
            //        string toServerLocation = RegionConfig.GetRegionAttr(_region_cd, "server_location");

            //        if (currentServerLocation.Equals(toServerLocation)) {
            //            currentRegion.CurrentLocation = _region_cd;
            //            currentRegion.CurrentDestination = _region_cd;
            //            filterContext.RequestContext.HttpContext.Session[Constants.SSKEY_CURRENT_REGION] = currentRegion;
            //        }
            //        else {
            //            SetRedirectResult(_region_cd, filterContext);
            //        }
            //    }
        }

        private void SetRedirectResult(string region_cd, ActionExecutingContext filterContext) {
            string base_url = RegionConfig.GetRegionAttr(region_cd, "base_url");
            if (base_url.EndsWith("/")) base_url = base_url.TrimEnd("/".ToCharArray());

            string path = filterContext.RequestContext.HttpContext.Request.Url.PathAndQuery;
            path = path.Substring(path.IndexOf("/", 1));		//「/CatWeb/Customer/54545/?xxxx...」の最初の/CatWebの部分を除去。

            string to_url = base_url + path;
            filterContext.Result = new RedirectResult(to_url);
        }
    }
}