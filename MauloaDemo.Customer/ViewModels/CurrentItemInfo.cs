using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MauloaDemo.Customer.Controllers;
using MauloaDemo.Utilities;

namespace MauloaDemo.Customer.ViewModels {

    [Serializable]
    public class CurrentItemInfo {

        public CurrentItemInfo() {
            ////デフォルトの言語をセット。
            //CurrentLanguage = Constants.LANGUAGEKEY_JAPANESE;
        }

        public string area_cd { get; set; }
        public string item_type { get; set; }
        public string item_cd { get; set; }
        public DateTime wed_date { get; set; }
        public DateTime min_date { get; set; }
        public DateTime? image_upload_date { get; set; }

        public string str_wed_date
        {
            get
            {
                return wed_date == null ? null : wed_date.ToString("yyyyMMdd");
            }
        }
        public string str_min_date
        {
            get
            {
                return min_date == null ? null : min_date.ToString("yyyyMMdd");
            }
        }
        public bool session_prior { get; set; }

    }
}