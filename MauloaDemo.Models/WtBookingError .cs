//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using WatabeWedding.Utilities;

//namespace MauloaDemo.Models {

//    [Table("wt_booking_error")]
//    public class WtBookingError {

//        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int wt_booking_error_id { get; set; }

//        public int? wt_id { get; set; }

//        public string c_num { get; set; }

//        public string region_cd { get; set; }

//        public string area_cd { get; set; }

//        public string item_cd { get; set; }

//        public string message { get; set; }

//        public DateTime err_date { get; set; }

//    }
//}
