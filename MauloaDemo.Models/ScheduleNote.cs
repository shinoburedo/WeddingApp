using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("schedule_note")]
    public class ScheduleNote {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int sch_note_id { get; set; }

        [Required(), StringLength(7), Hankaku()]
        public string c_num { get; set; }

        [StringLength(20), UpperCase, Hankaku]
        public string template_cd { get; set; }

        [StringLength(80)]
        public string title_jpn { get; set; }

        [StringLength(80)]
        public string title_eng { get; set; }

        public string note_jpn { get; set; }

        public string note_eng { get; set; }

        public int disp_seq { get; set; }

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
                if (!string.IsNullOrEmpty(this.template_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.template_cd, false)) {
                    yield return new ValidationResult(string.Format("template_cd は半角英数字で入力してください。({0})", this.template_cd), new[] { "template_cd" });
                }
            }
        }
    }
}
