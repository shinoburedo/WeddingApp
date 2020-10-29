using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("sales")]
    public class Sales : IValidatableObject {

        public const string INV_AGENT_CUST = "CUST";


        
        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int op_seq { get; set; }

        [Required(), StringLength(7), Hankaku()]
        public string c_num { get; set; }

        [Required(), StringLength(3), Hankaku(), UpperCase()]
        public string item_type { get; set; }

        [Required(), StringLength(15), Hankaku(), UpperCase()]
        public string item_cd { get; set; }

        [Required(), StringLength(6), Hankaku(), UpperCase()]
        public string agent_cd { get; set; }

        [StringLength(6), Hankaku(), UpperCase()]
        public string sub_agent_cd { get; set; }

        [StringLength(6), Hankaku(), UpperCase()]
        public string inv_agent { get; set; }

        [Required(), StringLength(50)]
        public string staff { get; set; }

        [StringLength(50)]
        public string branch_staff { get; set; }

        public short quantity { get; set; }

        public string note { get; set; }

        [IgnoreChangeDiff]
        public int? parent_op_seq { get; set; }

        [IgnoreChangeDiff]
        public int? upgrade_op_seq { get; set; }

        public bool cust_collect { get; set; }

        public bool tentative_price { get; set; }

        public decimal org_price { get; set; }

        public decimal price { get; set; }

        [IgnoreChangeDiff]
        public decimal amount { get; set; }

        [IgnoreChangeDiff]
        public bool price_changed { get; set; }

        [StringLength(200)]
        public string price_change_reason { get; set; }

        [Required(),StringLength(1),Hankaku(),UpperCase()]
        public string book_status { get; set; }

        [IgnoreChangeDiff]
        public DateTime? book_proc_date { get; set; }

        [IgnoreChangeDiff]
        [StringLength(15), Hankaku()]
        public string book_proc_by { get; set; }

        public bool jpn_cfm { get; set; }

        public DateTime? jpn_cfm_date { get; set; }

        [StringLength(15)]
        public string jpn_cfm_by { get; set; }

        public decimal? cxl_charge { get; set; }

        [IgnoreChangeDiff]
        public short? inv_seq { get; set; }

        [IgnoreChangeDiff]
        public DateTime? sales_post_date { get; set; }

        [IgnoreChangeDiff]
        public bool jnl_started { get; set; }

        [IgnoreChangeDiff, StringLength(15), Hankaku()]
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



        [ForeignKey("item_cd")]
        [IgnoreChangeDiff]
        public Item Item { get; set; }

        [NotMapped]
        public IEnumerable<Arrangement> Arrangements { get; set; }

        [NotMapped]
        public DeliveryInfo DeliveryInfo { get; set; }

        [NotMapped]
        public MakeInfo MakeInfo { get; set; }

        [NotMapped]
        public ShootInfo ShootInfo { get; set; }

        [NotMapped]
        public RcpInfo ReceptionInfo { get; set; }

        [NotMapped]
        public TransInfo TransInfo { get; set; }


        [NotMapped]
        public IEnumerable<BookingStatus> StatusList { get; set; }


        [NotMapped]
        private Sales _parent;

        [NotMapped]
        public ICollection<Sales> Children { get; set; }

        [NotMapped]
        public Sales Parent {
            get {
                return _parent;
            }
        }
        protected void SetParent(Sales parent) {
            _parent = parent;
        }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.c_num) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.c_num, false)) {
                yield return new ValidationResult(string.Format("c_num は半角英数字で入力してください。({0})", this.c_num), new[] { "c_num" });
            }

            if (!string.IsNullOrEmpty(this.item_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.item_cd, false)) {
                yield return new ValidationResult(string.Format("item_cd は半角英数字で入力してください。({0})", this.item_cd), new[] { "item_cd" });
            }

            if (!string.IsNullOrEmpty(this.agent_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.agent_cd, false)) {
                yield return new ValidationResult(string.Format("agent_cd は半角英数字で入力してください。({0})", this.agent_cd), new[] { "agent_cd" });
            }

            if (!string.IsNullOrEmpty(this.sub_agent_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.sub_agent_cd, false)) {
                yield return new ValidationResult(string.Format("sub_agent_cd は半角英数字で入力してください。({0})", this.sub_agent_cd), new[] { "sub_agent_cd" });
            }
        }
    }
}
