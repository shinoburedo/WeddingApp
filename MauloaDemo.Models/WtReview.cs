//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using WatabeWedding.Utilities;

//namespace MauloaDemo.Models {

//    [Table("wt_review")]
//    public class WtReview{

//        //Status
//        public const string STATUS_PENDING = "P";
//        public const string STATUS_DONE = "D";
//        public const string STATUS_NG = "N";

//        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int review_id { get; set; }

//        public int account_id { get; set; }

//        public int wt_id { get; set; }

//        [StringLength(20), Required]
//        public string nickname { get; set; }

//        public byte star { get; set; }

//        [StringLength(4000)]
//        public string review { get; set; }

//        public DateTime create_date { get; set; }
//        public string str_create_date {
//            get {
//                return create_date == null ? null : create_date.ToString("yyyyMMdd");
//            }
//        }

//        public DateTime? proc_date { get; set; }

//        public string proc_by { get; set; }

//        public string status { get; set; }

//        [NotMapped]
//        public string trf_item_cd { get; set; }
//        [NotMapped]
//        public string trf_item_name { get; set; }
//        [NotMapped]
//        public string c_num { get; set; }
//        [NotMapped]
//        public string last_name { get; set; }
//        [NotMapped]
//        public string first_name { get; set; }

//        [NotMapped]
//        public string customer_name {
//            get {
//                return last_name + " " + first_name;
//            }
//        }

//        [NotMapped]
//        public string star_disp {
//            get {
//                switch (this.star) {
//                    case 0:
//                        return "☆☆☆☆☆";
//                    case 1:
//                        return "★☆☆☆☆";
//                    case 2:
//                        return "★★☆☆☆";
//                    case 3:
//                        return "★★★☆☆";
//                    case 4:
//                        return "★★★★☆";
//                    case 5:
//                        return "★★★★★";
//                    default:
//                        return "★★★★★";
//                }
//            }
//        }

//        [NotMapped]
//        public string status_disp {
//            get {
//                switch (this.status) {
//                    case STATUS_PENDING:
//                        return "PENDING";
//                    case STATUS_NG:
//                        return "NG";
//                    case STATUS_DONE:
//                        return "DONE";
//                    default:
//                        return "";
//                }
//            }
//        }

//        public void ValidateSave() {
//            if (account_id == 0) {
//                throw new ArgumentNullException("account_id", "Required");
//            }

//            if (wt_id == 0) {
//                throw new ArgumentNullException("wt_id", "Required");
//            }

//            if (string.IsNullOrEmpty(this.nickname)) {
//                throw new ArgumentNullException("nickname", "Required");
//            }

//            if (string.IsNullOrEmpty(this.review)) {
//                throw new ArgumentNullException("review", "Required");
//            }

//        }
//    }
//}
