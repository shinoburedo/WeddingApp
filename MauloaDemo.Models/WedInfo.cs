using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MauloaDemo.Models.Combined;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("wed_info")]
    public class WedInfo : IValidatableObject {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int info_id { get; set; }

        public int op_seq { get; set; }

        [Required(), StringLength(7), Hankaku()]
        public string c_num { get; set; }

        [Required(), StringLength(5), Hankaku(), UpperCase()]
        public string req_church_cd { get; set; }

        public DateTime req_wed_date { get; set; }

        public DateTime req_wed_time { get; set; }

        public bool is_irregular_time { get; set; }

        public string note { get; set; }

        [IgnoreChangeDiff, Required(), StringLength(15), Hankaku()]
        public string create_by { get; set; }

        [IgnoreChangeDiff]
        public DateTime create_date { get; set; }

        [IgnoreChangeDiff, Required(), StringLength(15), Hankaku()]
        public string last_person { get; set; }

        [IgnoreChangeDiff]
        public DateTime update_date { get; set; }

        /// 排他制御のための更新日時比較で使うための値。ユーザーのTime Zoneによる補正処理からは除外される。
        [NotMapped]
        [IgnoreChangeDiff]
        public DateTime update_date_stamp { get; set; }


        //From sales, church and item tables.
        public string item_type { get; set; }
        public string item_cd { get; set; }
        public string item_name { get; set; }
        public string item_name_jpn { get; set; }
        public bool rq_default { get; set; }

        public string church_name { get; set; }
        public string church_name_jpn { get; set; }

        public string agent_cd { get; set; }
        public string sub_agent_cd { get; set; }
        public string inv_agent { get; set; }
        public bool cust_collect { get; set; }
        public short quantity { get; set; }
        public decimal price { get; set; }
        public decimal amount { get; set; }
        public string book_status { get; set; }
        [UpperCase()]
        public string staff { get; set; }
        [UpperCase()]
        public string branch_staff { get; set; }

        [NotMapped]
        public bool is_sunset { get; set; }

        [NotMapped]
        public List<Sales> PlanItems { get; set; }

        [NotMapped]
        public List<BookingStatus> StatusList { get; set; }

        public WedInfo() {
            this.PlanItems = new List<Sales>();
        }

        [NotMapped]
        public string req_wed_time_s {
            get {
                return is_sunset ? "Sunset" : req_wed_time.ToString("HH:mm");
            }
        }

        [NotMapped]
        public bool isPhoto {
            get {
                return this.item_type == "PHP";
            }
        }

        [NotMapped]
        public bool isWedding {
            get {
                return !isPhoto;
            }
        }

        public void ValidateSave() {
            if (string.IsNullOrEmpty(this.item_cd)) {
                throw new Exception("Please input an item code for the plan.");
            }
            if (string.IsNullOrEmpty(this.sub_agent_cd)) {
                throw new Exception("Please input a sub agent code for the plan.");
            }
            if (string.IsNullOrEmpty(this.req_church_cd)) {
                throw new Exception("Please select a church.");
            }
            if (string.IsNullOrEmpty(this.book_status)) {
                throw new Exception("Please select a booking status.");
            }
            if (this.req_wed_date < new DateTime(2010, 1, 1)) {
                throw new Exception(string.Format("Wedding Date is not valid. ({0:yyyy/MM/dd HH:mm:ss}) ", this.req_wed_date));
            }
            if (this.req_wed_time < new DateTime(2010, 1, 1)) {
                throw new Exception(string.Format("Wedding Time is not valid. ({0:yyyy/MM/dd HH:mm:ss}) ", this.req_wed_time));
            }
            if (string.IsNullOrEmpty(this.item_cd)) {
                throw new Exception("Please select a plan.");
            }
            if (string.IsNullOrEmpty(this.last_person)) {
                throw new Exception("User id is required for the plan.");
            }
            if (string.IsNullOrEmpty(this.staff)) {
                throw new Exception("Staff name is required.");
            }

        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.c_num) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.c_num, false)) {
                yield return new ValidationResult(string.Format("c_num は半角英数字で入力してください。({0})", this.c_num), new[] { "c_num" });
            }
        }
    }
}
