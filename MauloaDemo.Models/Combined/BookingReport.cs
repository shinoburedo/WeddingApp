using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauloaDemo.Models.Combined {

    public class BookingReport {


        public class SearchParam {
            public DateTime? date_from { get; set; }
            public DateTime? date_to { get; set; }
            public string agent_cd { get; set; }
            public string church_cd { get; set; }
            public string area_cd { get; set; }
            public string item_type { get; set; }
            public string vendor_cd { get; set; }
            public string item_cd { get; set; }
            public bool include_cust_cxl { get; set; }
            public bool include_sales_cxl { get; set; }
            public bool not_finalized_only { get; set; }
            //public bool isWedCxl { get; set; }
            //public bool isNotWedCxl { get; set; }
            //public bool isNotSalesCxl { get; set; }
            //public bool isNotSalesCfmd { get; set; }
            public BookingReport.SortBy? sort_by { get; set; }
        }

        public enum SortBy {
            c_num = 0,
            wed_date = 1,
            vendor_cd = 2,
            agent_cd = 3
        }

        public class Sales {

            public bool cust_cxl { get; set; }

            public string c_num { get; set; }

            public DateTime? wed_date { get; set; }
            public string wed_date_s {
                get {
                    return wed_date.HasValue ? wed_date.Value.ToString("MM/dd/yyyy") : "";
                }
            }

            public DateTime? wed_time { get; set; }
            public string wed_time_s {
                get {
                    return wed_time.HasValue ? wed_time.Value.ToString("HH:mm") : "";
                }
            }

            public string g_last { get; set; }

            public string g_first { get; set; }
            public string g_name {
                get {
                    return this.g_last + "," + this.g_first;
                }
            }

            public string b_last { get; set; }

            public string b_first { get; set; }
            public string b_name {
                get {
                    return this.b_last + "," + this.b_first;
                }
            }

            public string vendor_cd { get; set; }

            public string church_cd { get; set; }

            public string agent_cd { get; set; }

            public bool cfmd { get; set; }

            public string book_status { get; set; }
            public string book_status_n {
                get {
                    return BookingStatus.GetTextForValue(this.book_status);
                }
            }

            public string item_cd { get; set; }

            public string item_type { get; set; }

            public DateTime? final_date { get; set; }
            public bool is_finalized {
                get {
                    return final_date.HasValue ? true : false;
                }
            }
        }

    }
}
