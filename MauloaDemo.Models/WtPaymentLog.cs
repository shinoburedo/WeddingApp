//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//namespace MauloaDemo.Models {

//    [Table("wt_payment_log")]
//    public class WtPaymentLog {

//        //決済種別
//        public const string TELE_KIND_AUTH = "020"; //オーソリ
//        public const string TELE_KIND_AUTH_CAN = "021"; //オーソリキャンセル
//        public const string TELE_KIND_AUTH_RVS = "028"; //オーソリ補正
//        public const string TELE_KIND_PAY = "022"; //売上
//        public const string TELE_KIND_PAY_CAN = "023"; //売上キャンセル
//        public const string TELE_KIND_PAY_RVS = "029"; //売上補正
//        public const string TELE_KIND_AUTH_F = "180"; //オーソリ(多通貨)
//        public const string TELE_KIND_AUTH_CAN_F = "181"; //オーソリキャンセル(多通貨)
//        public const string TELE_KIND_AUTH_RVS_F = "184"; //オーソリ補正(多通貨)
//        public const string TELE_KIND_PAY_F = "182"; //売上(多通貨)
//        public const string TELE_KIND_PAY_CAN_F = "183"; //売上キャンセル(多通貨)
//        public const string TELE_KIND_PAY_RVS_F = "185"; //売上補正(多通貨)


//        [Key]
//        public int payment_log_id { get; set; }

//        public int wt_id { get; set; }

//        public int account_id { get; set; }

//        public string telegram_kind { get; set; }

//        public string payment_id { get; set; }

//        public Decimal? payment_amount { get; set; }

//        public string currency_code { get; set; }

//        public string result_status { get; set; }

//        public string response_cd { get; set; }

//        public string response_detail { get; set; }

//        public DateTime update_date { get; set; }

//    }
//}