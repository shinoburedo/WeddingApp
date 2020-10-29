using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MauloaDemo.Customer.Controllers;
using MauloaDemo.Utilities;
using MauloaDemo.Repository;

namespace MauloaDemo.Customer.ViewModels {

    [Serializable]
    public class RegionInfo {

        public RegionInfo() {
            ////デフォルトの言語をセット。
            //CurrentLanguage = Constants.LANGUAGEKEY_JAPANESE;
        }

        //現在選択されている地域のregion_cd
        public string CurrentDestination { get; set; }

        //現在選択されている地域のUTC時刻からの時差。(例： JPNは9, HWIは-10)
        public short TimeDiffFromUTC { get; set; }

        //通貨の書式 ("#,0.00", "#,0")
        public string CurrencyFormat { get; set; }

        //通貨記号 ("$", "\", など)
        public string CurrencySymbol { get; set; }

        //表示言語
        public string CurrentLanguage { get; set; }

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

        /// <summary>
        /// 決済通貨が「日本円」か否か。(沖縄、国内、およびタヒチ、バリは true を返す。)
        /// これが true の場合は海外からのオーダーであっても日本円での決済となる。
        /// false の場合は日本国内からのオーダーは日本円、海外からのオーダーは現地通貨での決済となる。
        /// </summary>
        /// <returns></returns>
        public bool IsPaymentJPY() {
            //return RegionConfig.GetPaymentCurrencyCd(this.CurrentDestination) == "JPY";
            return true;
        }

        /// <summary>
        /// 処理対象の現地側の地域が選択されているか否か。
        /// </summary>
        /// <returns></returns>
        public bool IsRegionSelected() {
            return (!string.IsNullOrWhiteSpace(this.CurrentDestination));
        }

        public string GetRegionName(bool isJpn) {
            var region_cd = this.CurrentDestination;
            var attrName = isJpn ? "region_name_jpn" : "region_name";

            return RegionConfig.GetRegionAttr(region_cd, attrName);
        }

    }
}