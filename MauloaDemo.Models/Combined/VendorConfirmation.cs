using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MauloaDemo.Models.Combined {

    public class VendorConfirmation {
        public string vendor_cd { get; set; }
        public int count { get; set; }

    }
    public class VendorConfirmationReport
    {
        public string c_num { get; set; }
        public string g_last { get; set; }
        public string g_first { get; set; }
        public string b_last { get; set; }
        public string b_first { get; set; }
        public DateTime? wed_date { get; set; }
        public DateTime? wed_time { get; set; }
        public string hotel_cd { get; set; }
        public string hotel_name { get; set; }
        public string church_cd { get; set; }
        public string church_name { get; set; }
        public int? op_seq { get; set; }
        public string item_type { get; set; }
        public string info_type { get; set; }
        public string note { get; set; }
        public string vendor_cd { get; set; }
        public string vendor_name { get; set; }
        public string op_tel { get; set; }
        public string op_fax { get; set; }
        public string item_cd { get; set; }
        public string item_name { get; set; }
        public DateTime? delivery_date { get; set; }
        public DateTime? delivery_time { get; set; }
        public string delivery_place { get; set; }
        public string delivery_note { get; set; }
        public DateTime? make_date { get; set; }
        public DateTime? make_time { get; set; }
        public string make_place { get; set; }
        public DateTime? make_in_time { get; set; }
        public string make_note { get; set; }
        public DateTime? party_date { get; set; }
        public DateTime? party_time { get; set; }
        public string rest_cd { get; set; }
        public string rcp_note { get; set; }
        public DateTime? shoot_date { get; set; }
        public DateTime? shoot_time { get; set; }
        public string shoot_place { get; set; }
        public string shoot_note { get; set; }
        public DateTime? pickup_date { get; set; }
        public DateTime? pickup_time { get; set; }
        public string pickup_place { get; set; }
        public DateTime? dropoff_time { get; set; }
        public string dropoff_place { get; set; }
        public string pickup_hotel { get; set; }
        public string dropoff_hotel { get; set; }
        public string trans_note { get; set; }

        public string report_group_key {
            get {
                return this.vendor_cd + this.info_type;
            }
        }

    }
}
