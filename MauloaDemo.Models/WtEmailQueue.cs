//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using WatabeWedding.Utilities;

//namespace MauloaDemo.Models {

//    [Table("wt_email_queue")]
//    public class WtEmailQueue{

//        //キュー内のステータス
//        public static class Status {
//            public const string New = "NEW";
//            public const string Processing = "PRC";
//            public const string Done = "DNE";
//            public const string Error = "ERR";
//            public const string Cancelled = "CXL";
//        }


//        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int queue_id { get; set; }

//        public int? account_temp_id { get; set; }

//        public int? account_id { get; set; }

//        [StringLength(7), Hankaku]
//        public string c_num { get; set; }

//        [StringLength(20), Hankaku, UpperCase]
//        public string template_cd { get; set; }

//        [StringLength(100)]
//        public string subject {get; set;}

//        public string body { get; set; }

//        [StringLength(200)]
//        public string from_addr { get; set; }

//        [StringLength(200)]
//        public string to_addr { get; set; }

//        [StringLength(200)]
//        public string bcc_addr { get; set; }

//        [StringLength(200)]
//        public string replyto_addr { get; set; }

//        [StringLength(3)]
//        public string status { get; set; }

//        public DateTime create_date { get; set; }

//        public DateTime? send_date { get; set; }

//        public int send_count { get; set; }

//        public string send_log { get; set; }


//        public WtEmailQueue() {
//            this.status = Status.New;
//        }

//    }
//}
