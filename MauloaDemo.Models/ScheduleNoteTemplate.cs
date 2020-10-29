using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("schedule_note_template")]
    public class ScheduleNoteTemplate : IValidatableObject {

        [Key, UpperCase, Hankaku]
        public string template_cd { get; set; }

        [StringLength(80)]
        public string title_jpn { get; set; }

        [StringLength(80)]
        public string title_eng { get; set; }

        public string note_jpn { get; set; }

        public string note_eng { get; set; }

        [Required(), StringLength(15)]
        public string last_person { get; set; }

        [IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        [NotMapped]
        public bool is_new { get; set; }


        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.template_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.template_cd, false)) {
                yield return new ValidationResult(string.Format("template_cdは半角英数字で入力してください。({0})", this.template_cd), new[] { "template_cd" });
            }
        }
    }
}
