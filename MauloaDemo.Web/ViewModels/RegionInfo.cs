using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MauloaDemo.Repository;
using MauloaDemo.Web.Controllers;

namespace MauloaDemo.Web.ViewModels {

    [Serializable]
    public class RegionInfo {

        ////現在位置している地域のregion_cd
        //public string CurrentLocation { get; set; }

        //現在の現地側操作対象のregion_cd
        //（CurrentLocationがALL以外の場合はCurrentLocationと必ず一致する。）
        public string CurrentDestination { get; set; }

        //現在選択されている地域のUTC時刻からの時差。(例： JPNは9, HWIは-10)
        public short TimeDiffFromUTC { get; set; }

        //通貨の書式 ("#,0.00", "#,0")
        public string CurrencyFormat { get; set; }

        //通貨記号 ("$", "\", など)
        public string CurrencySymbol { get; set; }

        //通貨記号と書式 ("$#,0.00", "#,0 XPF", など)
        private string _currentyFormatWithSymbol = null;

        public string CurrencyFormatWithSymbol {
            get {
                string s = this.CurrencySymbol + this.CurrencyFormat;
                if (!string.IsNullOrEmpty(this._currentyFormatWithSymbol)) {
                    s = this._currentyFormatWithSymbol;
                }
                return s; 
            }
            set {
                this._currentyFormatWithSymbol = value;
            }
        }


        
        ///// <summary>
        ///// 地域として「ALL（国内）」が選択されているか否か。
        ///// </summary>
        ///// <returns></returns>
        //public bool IsLocationAll() {
        //    return (RegionConfig.REGION_ALL.Equals(this.CurrentLocation));
        //}

        ///// <summary>
        ///// 処理対象の現地側の地域が選択されているか否か。
        ///// </summary>
        ///// <returns></returns>
        //public bool IsRegionSelected() {
        //    return (!String.IsNullOrWhiteSpace(this.CurrentDestination));
        //}
    }
}