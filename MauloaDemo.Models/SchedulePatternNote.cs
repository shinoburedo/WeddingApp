using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("schedule_pattern_note")]
    public class SchedulePatternNote: IValidatableObject {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int row_id { get; set; }

        public int sch_pattern_id { get; set; }

        [Required(), StringLength(20)]
        public string template_cd { get; set; }

        public int disp_seq { get; set; }

        [Required(), StringLength(15)]
        public string last_person { get; set; }

        [IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.template_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.template_cd, false)) {
                yield return new ValidationResult(string.Format("template_cd は半角英数字で入力してください。({0})", this.template_cd), new[] { "template_cd" });
            }
        }
    }
}
