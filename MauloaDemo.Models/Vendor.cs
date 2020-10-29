using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("vendor")]
    public class Vendor : IValidatableObject {

        [Key(), Required(), StringLength(8), Hankaku(), UpperCase()]
        public string vendor_cd { get; set; }

        [StringLength(100), Hankaku()]
        public string vendor_name { get; set; }

        [StringLength(100)]
        public string vendor_name_j { get; set; }

        [StringLength(5), Hankaku(), UpperCase()]
        public string vendor_type { get; set; }

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

        [StringLength(100)]
        public string ac_contact { get; set; }

        [StringLength(15), Hankaku(), UpperCase()]
        public string acct_cd { get; set; }


        [StringLength(100)]
        public string def_bank_cd { get; set; }


        [StringLength(30), Hankaku()]
        public string tax_id { get; set; }

        //public bool f1099_flag { get; set; }

        [StringLength(3), Hankaku(), UpperCase()]
        public string region { get; set; }

        [StringLength(3), Hankaku(), UpperCase()]
        public string area_cd { get; set; }

        public DateTime? discon_date { get; set; }

        public bool employee {get; set;}

        [Required(), StringLength(15), Hankaku()]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        [NotMapped]
        public bool is_new { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.vendor_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.vendor_cd, false)) {
                yield return new ValidationResult(string.Format("vendor_cd は半角英数字で入力してください。({0})", this.vendor_cd), new[] { "vendor_cd" });
            }
        }
    }
}
