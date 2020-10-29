using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("cos_info")]
    public class CosInfo : IValidatableObject {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int info_id { get; set; }

        //public int op_seq { get; set; }

        [Required(), StringLength(7), Hankaku()]
        public string c_num { get; set; }

        [Required(), StringLength(1), Hankaku(), UpperCase()]
        public string pax_type { get; set; }

        //public DateTime? fitting_date { get; set; }

        //public DateTime? fitting_time { get; set; }

        //public string fitting_place { get; set; }

        //public string style_cd { get; set; }

        //public string size { get; set; }

        //public string color { get; set; }

        [StringLength(15)]
        public string height { get; set; }

        [StringLength(15)]
        public string chest { get; set; }

        [StringLength(15)]
        public string waist { get; set; }

        [StringLength(15)]
        public string cloth_size { get; set; }

        [StringLength(15)]
        public string shoe_size { get; set; }

        public string note { get; set; }

        [IgnoreChangeDiff, StringLength(15), Hankaku(), LowerCase()]
        public string create_by { get; set; }

        [IgnoreChangeDiff]
        public DateTime create_date { get; set; }

        [IgnoreChangeDiff, Required(), StringLength(15), Hankaku(), LowerCase()]
        public string last_person { get; set; }

        [IgnoreChangeDiff]
        public DateTime update_date { get; set; }

        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.c_num) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.c_num, false)) {
                yield return new ValidationResult(string.Format("c_num は半角英数字で入力してください。({0})", this.c_num), new[] { "c_num" });
            }
        }
    }
}
