using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("schedule_pattern_line")]
    public class SchedulePatternLine : IValidatableObject {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sch_pattern_line_id { get; set; }

        public int sch_pattern_id { get; set; }

        public int min_offset { get; set; }

        [Required(), StringLength(50)]
        public string title { get; set; }

        [StringLength(50)]
        public string title_eng { get; set; }

        [StringLength(200)]
        public string description { get; set; }

        [StringLength(200)]
        public string description_eng { get; set; }

        [StringLength(3), Hankaku(), UpperCase()]
        public string item_type { get; set; }

        [Required(), StringLength(15)]
        public string last_person { get; set; }

        [IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.item_type) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.item_type, false)) {
                yield return new ValidationResult(string.Format("item_type は半角英数字で入力してください。({0})", this.item_type), new[] { "item_type" });
            }
        }
    }
}
