using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("notice")]
    public class Notice : IValidatableObject {

        [Key]
        public int notice_id { get; set; }

        public DateTime from_date { get; set; }

        public DateTime? to_date { get; set; }

        public string agent_cd { get; set; }

        [StringLength(80)]
        public string title_jpn { get; set; }

        [StringLength(80)]
        public string title_eng { get; set; }

        public string notice_jpn { get; set; }

        public string notice_eng { get; set; }

        public int disp_seq { get; set; }

        [Required(), StringLength(3)]
        public string notice_type { get; set; }

        [Required(), StringLength(15)]
        public string last_person { get; set; }

        [IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        [NotMapped]
        public bool is_new { get; set; }


        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.agent_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.agent_cd, false)) {
                yield return new ValidationResult(string.Format("agent_cdは半角英数字で入力してください。({0})", this.agent_cd), new[] { "agent_cd" });
            }
        }
    }
}
