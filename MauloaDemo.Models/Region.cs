using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {
    public class Region : IValidatableObject {

        [Key(),Required(), StringLength(3), Hankaku(), UpperCase()]
        public string region_cd { get; set; }

        public string region_name { get; set; }

        public string region_name_jpn { get; set; }

        public string server_location { get; set; }

        public bool active { get; set; }

        public string region_group { get; set; }

        public bool overseas { get; set; }

        public short TimeDiffFromUTC { get; set; }

        public string SvrTimeZone { get; set; }

        public string base_url { get; set; }

        public string svc_url { get; set; }

        public string tax_type { get; set; }

        public bool enable_OKA_CSC { get; set; }

        public string dbname { get; set; }

        public string dbserver { get; set; }

        public string paper_size { get; set; }

        public string currency_format { get; set; }

        public string currency_symbol { get; set; }

        public string currency_format_with_symbol { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.region_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.region_cd, false)) {
                yield return new ValidationResult(string.Format("region_cd は半角英数字で入力してください。({0})", this.region_cd), new[] { "region_cd" });
            }
        }
    }
}


