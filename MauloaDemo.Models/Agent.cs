using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;
using System.Configuration;

namespace MauloaDemo.Models {

    [Table("agent")]
    public class Agent : IValidatableObject { 

        [Key(), Required(), StringLength(6), Hankaku(), UpperCase()]
        public string agent_cd { get; set; }

        [StringLength(100), Hankaku()]
        public string agent_name { get; set; }

        [StringLength(100)]
        public string agent_name_jpn { get; set; }

        [StringLength(100)]
        public string op_address1 { get; set; }

        [StringLength(100)]
        public string op_address2 { get; set; }

        [StringLength(100)]
        public string op_address3 { get; set; }

        [StringLength(100)]
        public string op_address4 { get; set; }

        [StringLength(100)]
        public string op_tel { get; set; }

        [StringLength(100)]
        public string op_fax { get; set; }

        [StringLength(100)]
        public string ac_address1 { get; set; }

        [StringLength(100)]
        public string ac_address2 { get; set; }

        [StringLength(100)]
        public string ac_address3 { get; set; }

        [StringLength(100)]
        public string ac_address4 { get; set; }

        [StringLength(100)]
        public string ac_tel { get; set; }

        [StringLength(100)]
        public string ac_fax { get; set; }

        [StringLength(100)]
        public string ac_contact { get; set; }

        [StringLength(1), Hankaku(), UpperCase()]
        public string agent_fit { get; set; }

        [StringLength(100)]
        public string ac_contact1 { get; set; }

        [StringLength(15), Hankaku(),UpperCase()]
        public string acct_cd { get; set; }

        [StringLength(15), Hankaku(), UpperCase()]
        public string comm_acct_cd { get; set; }

        [StringLength(100)]
        public string def_bank { get; set; }

        [StringLength(3), Hankaku(), UpperCase()]
        public string region { get; set; }

        [StringLength(3), Hankaku(), UpperCase()]
        public string area_cd { get; set; }

        public short? block_release { get; set; }

        public DateTime? discon_date { get; set; }

        public bool staff_required { get; set; }

        public bool branch_staff_required { get; set; }

        [Required(), StringLength(15), Hankaku()]
        public string last_person { get; set; }

        [IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        [NotMapped]
        public bool is_new { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.agent_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.agent_cd, false)) {
                yield return new ValidationResult(string.Format("agent_cd は半角英数字で入力してください。({0})", this.agent_cd), new[] { "agent_cd" });
            }

            if (!string.IsNullOrEmpty(this.agent_name) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.agent_name, true)) {
                yield return new ValidationResult(string.Format("agent_name は半角英数字で入力してください。({0})", this.agent_name), new[] { "agent_name" });
            }
        }

        public static string GetOwnAgentCd() {
            var cd = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["OwnAgentCd"]);
            return cd;
        }

    }
}
