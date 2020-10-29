using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("item_cost")]
    public class ItemCost : IValidatableObject {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int item_cost_id { get; set; }

        [Required(), StringLength(15), Hankaku(), UpperCase()]
        public string item_cd { get; set; }

        [Required(), StringLength(8), Hankaku(), UpperCase()]
        public string vendor_cd { get; set; }

        [Required(), StringLength(5), Hankaku(), UpperCase()]
        public string church_cd { get; set; }

        public decimal cost { get; set; }

        public DateTime? eff_from { get; set; }
        public DateTime? eff_to { get; set; }

        [Required(), StringLength(15), Hankaku()]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }

        [NotMapped]
        public bool is_new { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.item_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.item_cd, false)) {
                yield return new ValidationResult(string.Format("item_cd は半角英数字で入力してください。({0})", this.item_cd), new[] { "item_cd" });
            }

            if (!string.IsNullOrEmpty(this.vendor_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.vendor_cd, false)) {
                yield return new ValidationResult(string.Format("vendor_cd は半角英数字で入力してください。({0})", this.vendor_cd), new[] { "vendor_cd" });
            }

            if (!string.IsNullOrEmpty(this.church_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.church_cd, false)) {
                yield return new ValidationResult(string.Format("church_cd は半角英数字で入力してください。({0})", this.church_cd), new[] { "church_cd" });
            }
        }
    }
}
