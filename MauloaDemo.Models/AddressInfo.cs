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

    [Table("address_info")]
    public class AddressInfo : IValidatableObject {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int info_id { get; set; }

        [Required(), StringLength(7)]
        public string c_num { get; set; }

        [Required(), StringLength(1), UpperCase()]
        public string pax_type { get; set; }

        [Required(), StringLength(100), UpperCase()]
        public string pax_name { get; set; }

        [StringLength(10), Hankaku()]
        public string jpn_zip { get; set; }

        [StringLength(100)]
        public string addr_kana1 { get; set; }

        [StringLength(100)]
        public string addr_kana2 { get; set; }

        [StringLength(100)]
        public string addr_kana3 { get; set; }

        [StringLength(100)]
        public string addr_kanji1 { get; set; }

        [StringLength(100)]
        public string addr_kanji2 { get; set; }

        [StringLength(100)]
        public string addr_kanji3 { get; set; }

        [StringLength(20), Hankaku()]
        public string home_tel { get; set; }

        [StringLength(20), Hankaku()]
        public string work_tel { get; set; }

        [StringLength(20), Hankaku()]
        public string cell_tel { get; set; }

        [EmailAddress(), StringLength(50), Hankaku(), LowerCase()]
        public string e_mail { get; set; }

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
