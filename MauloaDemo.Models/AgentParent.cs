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

    [Table("agent_parent")]
    public class AgentParent : IValidatableObject {

        [Key(), Required()]
        public string child_cd { get; set; }

        public string parent_cd { get; set; }
        public string inv_agent { get; set; }
        public string invoice_type { get; set; }
        public bool indep_flg { get; set; }
        public string contact_name { get; set; }
        public string company_name { get; set; }
        public string company_name_eng { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string tel1 { get; set; }
        public string tel2 { get; set; }
        public string fax { get; set; }

        [StringLength(50), Hankaku(), LowerCase()]
        public string email { get; set; }

        public string agent_area_cd { get; set; }
        //public string cos_kbn { get; set; }
        //public string logi_kbn { get; set; }
        //public string stock_agent { get; set; }
        public DateTime? discon_date { get; set; }

        [Required()]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }

        [NotMapped]
        public bool is_new { get; set; }

        public string GetParentCd() {
            return TypeHelper.GetStrTrim(string.IsNullOrEmpty(this.parent_cd) ? this.child_cd : this.parent_cd);
        }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.child_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.child_cd, false)) {
                yield return new ValidationResult(string.Format("child_cd は半角英数字で入力してください。({0})", this.child_cd), new[] { "child_cd" });
            }
        }

        public static string GetOwnSubAgentCd() {
            var cd = TypeHelper.GetStrTrim(ConfigurationManager.AppSettings["OwnSubAgentCd"]);
            return cd;
        }


    }
}
