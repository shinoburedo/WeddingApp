using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;

namespace MauloaDemo.Models {

    [Table("arrangement")]
    public class Arrangement : IValidatableObject {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int arrangement_id { get; set; }

        [Required()]
        public int op_seq { get; set; }

        [Required()]
        public string c_num { get; set; }

        public string vendor_cd { get; set; }

        public bool cfmd { get; set; }

        public string cfmd_by { get; set; }

        public DateTime? cfmd_date { get; set; }

        public bool cxl { get; set; }

        public string cxl_vend_by { get; set; }

        public DateTime? cxl_date { get; set; }

        public string note { get; set; }

        public short quantity { get; set; }

        public decimal cost { get; set; }

        public bool jnl_started { get; set; }

        public string create_by { get; set; }

        public DateTime create_date { get; set; }

        [Required()]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.c_num) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.c_num, false)) {
                yield return new ValidationResult(string.Format("c_num は半角英数字で入力してください。({0})", this.c_num), new[] { "c_num" });
            }
        }
    }
}
