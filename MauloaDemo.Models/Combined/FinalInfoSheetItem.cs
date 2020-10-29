using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauloaDemo.Models.Combined {

    public class FinalInfoSheetInfo {
        public List<SalesListItem> list { get; set; }
        public Customer Customer { get; set; }
        public string agent_name { get; set; }
        public string church_name { get; set; }
        public List<AddressInfo> AddressInfos { get; set; }
        public List<CosInfo> CosInfos { get; set; }
        public WedInfo WedPlan { get; set; }
        public List<SalesListItem> WedPlanItems { get; set; }
        public WedInfo PhotoPlan { get; set; }
        public List<SalesListItem> PhotoPlanItems { get; set; }
        public ScheduleSheetInfo Schedule { get; set; }
    }

    public class FinalInfoSheetItem {
        //public string 	c_num			{ get; set; }
        //public string   cust_name       { get; set; }
        //public string   cust_name_kanji { get; set; }
        //public string 	cust_phone		{ get; set; }

        //public DateTime? wed_date		{ get; set; }
        //public DateTime? wed_time		{ get; set; }
        //public string 	church_cd		{ get; set; }
        //public string 	church_name		{ get; set; }
        //public string 	hotel_cd		{ get; set; }
        //public string 	hotel_name		{ get; set; }

        //public int?     plan_op_seq     { get; set; }
        //public string 	plan_item_type	{ get; set; }
        //public string 	plan_item_cd	{ get; set; }
        //public string 	plan_item_name	{ get; set; }
        //public DateTime? plan_date      { get; set; }
        //public DateTime? plan_time      { get; set; }

        //public int      op_seq          { get; set; }
        //public bool     is_option       { get; set; }
        //public DateTime? info_date      { get; set; }
        //public DateTime? info_time		{ get; set; }
        //public string 	item_type		{ get; set; }
        //public string 	item_cd			{ get; set; }
        //public string 	item_name		{ get; set; }
        //public short    quantity        { get; set; }
        //public string 	vendor_cd		{ get; set; }
        //public string 	vendor_name		{ get; set; }
        //public string   note            { get; set; }

        public int op_seq { get; set; }

        public string c_num { get; set; }

        public string item_type { get; set; }

        public string item_cd { get; set; }

        public string agent_cd { get; set; }

        public string sub_agent_cd { get; set; }

        public string staff { get; set; }

        public string branch_staff { get; set; }

        public short quantity { get; set; }

        public int? parent_op_seq { get; set; }

        public bool cust_collect { get; set; }

        public decimal price { get; set; }

        public decimal amount { get; set; }

        public string book_status { get; set; }

        public DateTime? book_proc_date { get; set; }

        public string book_proc_by { get; set; }

        public short? inv_seq { get; set; }

        public string last_person { get; set; }

        public DateTime update_date { get; set; }

        //Fields from other tables:
        public string item_name { get; set; }
        public string item_name_jpn { get; set; }
        public string info_type { get; set; }
        public string vendor_cd { get; set; }
        public string vendor_name { get; set; }

        public string BookStatus {
            get {
                return BookingStatus.GetTextForValue(this.book_status);
            }
        }
    }
}
