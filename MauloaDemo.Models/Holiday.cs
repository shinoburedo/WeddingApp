using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("holiday")]
    public class Holiday : IValidatableObject {
        [Key()]
        public DateTime holiday { get; set; }

        public string description { get; set; }
        public bool st_flag { get; set; }

        [Required(), Hankaku(), StringLength(15)]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }

        [NotMapped]
        public bool is_new { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.last_person) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.last_person, false)) {
                yield return new ValidationResult(string.Format("last_person は半角英数字で入力してください。({0})", this.last_person), new[] { "last_person" });
            }
        }
    }
}
