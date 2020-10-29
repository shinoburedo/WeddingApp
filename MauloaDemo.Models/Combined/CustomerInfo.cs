using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauloaDemo.Models.Combined {

    public class CustomerInfo {

        public Customer Customer { get; set; }

        public List<CosInfo> CosInfoList { get; set; }
        public List<AddressInfo> Addresses { get; set; }
        public List<WedInfo> WeddingPlans { get; set; }
        public List<ShootInfo> PhotoPlans { get; set; }
        //public List<SalesListItem> Options { get; set; }

        public CustomerInfo() {
            this.Customer = new Customer();

            this.CosInfoList = new List<CosInfo>();
            this.Addresses = new List<AddressInfo>();
            //this.WeddingPlans = new List<WedInfo>();
            //this.PhotoPlans = new List<ShootInfo>();
            //this.Options = new List<SalesListItem>();
        }
    }

    public class DailyMovement {
        public string sort_order { get; set; }
        public string c_num { get; set; }
        public string church_cd { get; set; }
        public DateTime? wed_date { get; set; }
        public string wed_time { get; set; }
        public string htl_pick { get; set; }
        public string agent_cd { get; set; }
        public string pkg_cd { get; set; }
        public string pkg_name { get; set; }
        public string gname { get; set; }
        public string bname { get; set; }
        public string hotel_cd { get; set; }
        public string room_number { get; set; }
        public DateTime? checkin_date { get; set; }
        public DateTime? checkout_date { get; set; }
        public string att_hotel1 { get; set; }
        public short? attend_count { get; set; }
        public string staff { get; set; }
        public string g_custtype { get; set; }
        public string b_custtype { get; set; }
        public string note { get; set; }
        public string o_note { get; set; }
        public string pkg { get; set; }
        public string rest_cd { get; set; }
        public DateTime? party_date { get; set; }
        public DateTime? party_from { get; set; }
        public string cat_id { get; set; }
        public string item_type { get; set; }
        public string item_cd { get; set; }
        public int op_seq { get; set; }
        public string item_name { get; set; }
        public short? quantity { get; set; }
        public string vendor_cd { get; set; }
        public string sort_key { get; set; }
    }

}
