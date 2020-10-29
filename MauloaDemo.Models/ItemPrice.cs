using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("item_price")]
    public class ItemPrice : IValidatableObject {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int item_price_id { get; set; }

        [Required(), StringLength(15), Hankaku(), UpperCase()]
        public string item_cd { get; set; }

        [Required(), StringLength(6), Hankaku(), UpperCase()]
        public string agent_cd { get; set; }

        [StringLength(1), Hankaku, UpperCase]
        public string plan_kind { get; set; }

        public DateTime eff_from { get; set; }
        public DateTime? eff_to { get; set; }

        public decimal gross { get; set; }
        public decimal d_net { get; set; }

        public int y_net { get; set; }

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

            if (!string.IsNullOrEmpty(this.agent_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.agent_cd, false)) {
                yield return new ValidationResult(string.Format("agent_cd は半角英数字で入力してください。({0})", this.agent_cd), new[] { "agent_cd" });
            }

            if (!string.IsNullOrEmpty(this.plan_kind) && !"PW".Contains(this.plan_kind)) {
                yield return new ValidationResult(string.Format("plan_kindには'P', 'W'または空白のみ入力可能です。({0})", this.agent_cd), new[] { "plan_kind" });
            }
        }
    }
}
