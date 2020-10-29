using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("schedule_pattern_item")]
    public class SchedulePatternItem : IValidatableObject {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int row_id { get; set; }

        public int sch_pattern_id { get; set; }

        [Required(), StringLength(15), Hankaku(), UpperCase()]
        public string item_cd { get; set; }

        [Required(), StringLength(15)]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.item_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.item_cd, false)) {
                yield return new ValidationResult(string.Format("item_cd は半角英数字で入力してください。({0})", this.item_cd), new[] { "item_cd" });
            }
        }
    }
}
