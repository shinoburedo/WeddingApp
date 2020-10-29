//using System;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using WatabeWedding.Utilities;

//namespace MauloaDemo.Models {

//    [Table("wt_email_limit")]
//    public class WtEmailLimit {

//        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
//        public int row_id { get; set; }

//        [Required, StringLength(50), Hankaku]
//        public string src_ip_address { get; set; }

//        public DateTime create_date { get; set; }

//        [Required]
//        public int queue_id { get; set; }

//    }
//}
