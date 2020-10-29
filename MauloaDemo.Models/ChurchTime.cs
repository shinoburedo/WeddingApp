using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;

namespace MauloaDemo.Models {

    [Table("church_time")]
    public class ChurchTime : IValidatableObject {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int church_time_id { get; set; }

        [Required(), StringLength(5)]
        public string church_cd { get; set; }

        [Required()]
        public DateTime start_time { get; set; }

        [StringLength(15)]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.church_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.church_cd, false)) {
                yield return new ValidationResult(string.Format("church_cd は半角英数字で入力してください。({0})", this.church_cd), new[] { "church_cd" });
            }
        }
    }
}
