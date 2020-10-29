using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;

namespace MauloaDemo.Models {

    [Table("area")]
    public class Area : IValidatableObject {

        [Key(), Required(), StringLength(3)]
        public string area_cd { get; set; }

        [StringLength(30)]
        public string desc_eng { get; set; }

        [StringLength(30)]
        public string desc_jpn { get; set; }

        public int area_seq { get; set; }

        [Required(), StringLength(50)]
        public string address_name { get; set; }

        [Required(), StringLength(50)]
        public string add_name_jpn { get; set; }

        [Required(), StringLength(50)]
        public string main_add1 { get; set; }

        [StringLength(50)]
        public string main_add2 { get; set; }

        [StringLength(50)]
        public string main_add3 { get; set; }

        [StringLength(50)]
        public string main_add4 { get; set; }

        [StringLength(20)]
        public string main_tel { get; set; }

        [StringLength(20)]
        public string main_fax { get; set; }

        [StringLength(20)]
        public string emg_contact1 { get; set; }

        [StringLength(20)]
        public string emg_tel1 { get; set; }

        [StringLength(20)]
        public string emg_contact2 { get; set; }

        [StringLength(20)]
        public string emg_tel2 { get; set; }

        [Required(), StringLength(15)]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        [NotMapped]
        public bool is_new { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.area_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.area_cd, false)) {
                yield return new ValidationResult(string.Format("Code は半角英数字で入力してください。({0})", this.area_cd), new[] { "area_cd" });
            }

            if (!string.IsNullOrEmpty(this.desc_eng) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.desc_eng, true)) {
                yield return new ValidationResult(string.Format("Description Eng は半角英数字で入力してください。({0})", this.desc_eng), new[] { "desc_eng" });
            }

        }

    }
}
