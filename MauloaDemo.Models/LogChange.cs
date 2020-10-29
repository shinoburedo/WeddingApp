using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("log_change")]
    public class LogChange : IValidatableObject {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int log_id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime log_datetime { get; set; }

        [Required(), StringLength(15), Hankaku(), LowerCase()]
        public string login_id { get; set; }

        [StringLength(7), Hankaku()]
        public string c_num { get; set; }

        [Required(), StringLength(50), Hankaku(), LowerCase()]
        public string table_name { get; set; }

        public int? key_id { get; set; }

        [StringLength(20), Hankaku()]
        public string key_cd { get; set; }

        [Required(), StringLength(1), Hankaku(), UpperCase()]
        public string action { get; set; }          // 'I'=Insert, 'U'=Update, 'D'=Delete

        public string changes { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.login_id) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.login_id, false)) {
                yield return new ValidationResult(string.Format("login_id は半角英数字で入力してください。({0})", this.login_id), new[] { "login_id" });
            }

            if (string.IsNullOrWhiteSpace(this.action) || this.action.Length != 1) {
                yield return new ValidationResult(string.Format("action には I, U, D のいずれかをセットして下さい。({0})", this.action), new[] { "action" });
            }

            if (!"IUD".Contains(this.action)) {
                yield return new ValidationResult(string.Format("action には I, U, D のいずれかをセットして下さい。({0})", this.action), new[] { "action" });
            }
        }

    }
}
