using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("item_option")]
    public class ItemOption : IValidatableObject {

        [Key(), Column(Order = 10), Required(), StringLength(15), Hankaku(), UpperCase()]
        public string item_cd { get; set; }

        [Key(), Column(Order = 20), Required(), StringLength(15), Hankaku(), UpperCase()]
        public string child_cd { get; set; }

        [Required(), StringLength(15), Hankaku()]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }

        [NotMapped]
        public string item_name { get; set; }
        [NotMapped]
        public string item_name_jpn { get; set; }
        [NotMapped]
        public string item_type { get; set; }

        [NotMapped]
        public bool is_new { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.item_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.item_cd, false)) {
                yield return new ValidationResult(string.Format("item_cd は半角英数字で入力してください。({0})", this.item_cd), new[] { "item_cd" });
            }

            if (!string.IsNullOrEmpty(this.child_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.child_cd, false)) {
                yield return new ValidationResult(string.Format("child_cd は半角英数字で入力してください。({0})", this.child_cd), new[] { "child_cd" });
            }
        }
    }
}
