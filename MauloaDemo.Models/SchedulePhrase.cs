using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("schedule_phrase")]
    public class SchedulePhrase {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sch_phrase_id { get; set; }

        [Required(), StringLength(7), Hankaku()]
        public string c_num { get; set; }

        public int sch_pattern_line_id { get; set; }

        public DateTime date { get; set; }

        [StringLength(5), Hankaku()]
        public string time { get; set; }

        [IgnoreChangeDiff]
        public int disp_seq { get; set; }

        [StringLength(50)]
        public string place { get; set; }

        [StringLength(50)]
        public string place_eng { get; set; }

        [StringLength(50)]
        public string title { get; set; }

        [StringLength(50)]
        public string title_eng { get; set; }

        [StringLength(200)]
        public string description { get; set; }

        [StringLength(200)]
        public string description_eng { get; set; }

        [StringLength(3), Hankaku(), UpperCase()]
        public string item_type { get; set; }

        public int op_seq { get; set; }

        [IgnoreChangeDiff]
        public bool deleted { get; set; }

        [IgnoreChangeDiff, Required(), StringLength(15)]
        public string last_person { get; set; }

        [IgnoreChangeDiff]
        public DateTime update_date { get; set; }




        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.c_num) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.c_num, false)) {
                yield return new ValidationResult(string.Format("c_num は半角英数字で入力してください。({0})", this.c_num), new[] { "c_num" });
            }

            if (!this.deleted) {
                if (!string.IsNullOrEmpty(this.time) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.time, false)) {
                    yield return new ValidationResult(string.Format("time は半角英数字で入力してください。({0})", this.time), new[] { "time" });
                }
            }
        }
    }
}
