using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MauloaDemo.Models.Combined {

    public class CostumeOrderSheetInfo {
        public CosInfo CosInfo { get; set; }
        public Customer Customer { get; set; }
        public string church_name { get; set; }
        public string hotel_name { get; set; }
        public string agent_name { get; set; }
        public string item_name { get; set; }
        public string plan_name { get; set; }
        public List<Vendor> MksVendorList { get; set; }
        public List<CostumeOrderSheetDeliveryInfo> FlwDlvList { get; set; }
        public Vendor CosVendor { get; set; }
    }

    public class CostumeOrderSheetDeliveryInfo {
        public DateTime? delivery_date { get; set; }
        public DateTime? delivery_time { get; set; }
        public string item_cd { get; set; }
        public string item_name { get; set; }

    }

}
