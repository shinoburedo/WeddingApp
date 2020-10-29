using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("log_action")]
    public class LogAction : IValidatableObject {

        public const string ACT_LOGIN = "LOGIN";
        public const string ACT_LOGOUT = "LOGOUT";
        public const string ACT_CHGPWD = "CHGPWD";
        public const string ACT_VIEW = "VIEW";
        public const string ACT_PRINT = "PRINT";


        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int log_id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime log_datetime { get; set; }

        [Required(), StringLength(15), Hankaku(), LowerCase()]
        public string login_id { get; set; }

        [Required(), StringLength(20), Hankaku(), UpperCase()]
        public string action_cd { get; set; }

        [StringLength(200)]
        public string detail { get; set; }

        public int? parent_log_id { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.login_id) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.login_id, false)) {
                yield return new ValidationResult(string.Format("login_id は半角英数字で入力してください。({0})", this.login_id), new[] { "login_id" });
            }

            if (!string.IsNullOrEmpty(this.action_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.action_cd, false)) {
                yield return new ValidationResult(string.Format("action_cd は半角英数字で入力してください。({0})", this.action_cd), new[] { "action_cd" });
            }
        }

    }
}
