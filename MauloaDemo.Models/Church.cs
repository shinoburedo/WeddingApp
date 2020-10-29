using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;

namespace MauloaDemo.Models {

    [Table("church")]
    public class Church : IValidatableObject {

        [Key(), Required(), StringLength(5)]
        public string church_cd { get; set; }

        [Required, StringLength(100)]
        public string church_name { get; set; }

        [Required, StringLength(100)]
        public string church_name_jpn { get; set; }

        [StringLength(1)]
        public string plan_kind { get; set; }

        [StringLength(10)]
        public string abbrev_jpn { get; set; }

        [StringLength(10)]
        public string abbrev_eng { get; set; }

        [StringLength(3)]
        public string area_cd { get; set; }

        public bool exclusive { get; set; }

        public short? release_days { get; set; }

        [StringLength(100)]
        public string op_address1 { get; set; }

        [StringLength(100)]
        public string op_address2 { get; set; }

        [StringLength(100)]
        public string op_address3 { get; set; }

        [StringLength(100)]
        public string op_address4 { get; set; }

        [StringLength(100)]
        public string op_tel { get; set; }

        [StringLength(100)]
        public string op_fax { get; set; }

        [StringLength(100)]
        public string ac_address1 { get; set; }

        [StringLength(100)]
        public string ac_address2 { get; set; }

        [StringLength(100)]
        public string ac_address3 { get; set; }

        [StringLength(100)]
        public string ac_address4 { get; set; }

        [StringLength(100)]
        public string ac_tel { get; set; }

        [StringLength(100)]
        public string ac_fax { get; set; }

        public string note { get; set; }

        public DateTime? discon_date { get; set; }

        public short disp_seq { get; set; }

        public short default_pickup { get; set; }

        [Required]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }




        [NotMapped]
        public bool is_new { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.church_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.church_cd, false)) {
                yield return new ValidationResult(string.Format("church_cd は半角英数字で入力してください。({0})", this.church_cd), new[] { "church_cd" });
            }

            if (!string.IsNullOrEmpty(this.church_name) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.church_name, true)) {
                yield return new ValidationResult(string.Format("Church Name Eng は半角英数字で入力してください。({0})", this.church_name), new[] { "church_name" });
            }
        }

    }
}
