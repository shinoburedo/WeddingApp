using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauloaDemo.Models.Combined {

    public class SalesListItem {

        public int op_seq { get; set; }

        public string c_num { get; set; }

        public string item_type { get; set; }

        public string item_cd { get; set; }

        public string agent_cd { get; set; }

        public string sub_agent_cd { get; set; }

        public string staff { get; set; }

        public string branch_staff { get; set; }

        public short quantity { get; set; }

        //public string note { get; set; }

        public int? parent_op_seq { get; set; }

        //public int? upgrade_op_seq { get; set; }

        public bool cust_collect { get; set; }

        //public bool tentative_price { get; set; }

        //public decimal org_price { get; set; }

        public decimal price { get; set; }

        public decimal amount { get; set; }

        //public bool price_changed { get; set; }

        //public string price_change_reason { get; set; }

        public string book_status { get; set; }

        public DateTime? book_proc_date { get; set; }

        public string book_proc_by { get; set; }

        //public bool jpn_cfm { get; set; }

        //public DateTime? jpn_cfm_date { get; set; }

        //public string jpn_cfm_by { get; set; }

        //public decimal? cxl_charge { get; set; }

        public short? inv_seq { get; set; }

        //public DateTime? sales_post_date { get; set; }

        //public bool jnl_started { get; set; }

        //public string create_by { get; set; }

        //public DateTime create_date { get; set; }

        public string last_person { get; set; }

        public DateTime update_date { get; set; }



        //Fields from other tables:
        public string item_name { get; set; }
        public string item_name_jpn { get; set; }
        public string info_type { get; set; }

        public string BookStatus {
            get {
                return BookingStatus.GetTextForValue(this.book_status);
            }
        }
        public List<Arrangement> arrangements { get; set; }
        public string vendor_cd { get; set; }
        public bool cfmd { get; set; }
        public string note { get; set; }

    }
}
