using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("log_change_archive")]
    public class LogChangeArchive : IValidatableObject {

        public class ChangedRows { 
            public ICollection<int> ids_on {get; set; }
            public ICollection<int> ids_off { get; set; }
        }

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int archive_id { get; set; }

        [Required()]
        public int log_id { get; set; }

        [Required(), StringLength(6), Hankaku()]
        public string sub_agent_cd { get; set; }

        [Required(), StringLength(15), Hankaku(), LowerCase()]
        public string archive_by { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime archive_datetime { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.archive_by) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.archive_by, false)) {
                yield return new ValidationResult(string.Format("archive_by は半角英数字で入力してください。({0})", this.archive_by), new[] { "archive_by" });
            }

            if (!string.IsNullOrEmpty(this.sub_agent_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.sub_agent_cd, false)) {
                yield return new ValidationResult(string.Format("sub_agent_cd は半角英数字で入力してください。({0})", this.sub_agent_cd), new[] { "sub_agent_cd" });
            }
        }
    }
}
