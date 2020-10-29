using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("hotel")]
    public class Hotel : IValidatableObject {

        [Key(), Required(), StringLength(3), Hankaku(), UpperCase()]
        public string hotel_cd { get; set; }

        [StringLength(100), Hankaku()]
        public string hotel_name { get; set; }

        [StringLength(100)]
        public string hotel_name_jpn { get; set; }

        [StringLength(100)]
        public string tel { get; set; }

        [StringLength(3), Hankaku(), UpperCase()]
        public string area_cd { get; set; }

        public short sort_order { get; set; }

        public DateTime? discon_date { get; set; }

        [Required(), StringLength(15), Hankaku()]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }

        [NotMapped]
        public bool is_new { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.hotel_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.hotel_cd, false)) {
                yield return new ValidationResult(string.Format("Hotel Code は半角英数字で入力してください。({0})", this.hotel_cd), new[] { "hotel_cd" });
            }

            if (!string.IsNullOrEmpty(this.hotel_name) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.hotel_name, true)) {
                yield return new ValidationResult(string.Format("Hotel Name Eng は半角英数字で入力してください。({0})", this.hotel_name), new[] { "hotel_name" });
            }

            if (!string.IsNullOrEmpty(this.area_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.area_cd, false)) {
                yield return new ValidationResult(string.Format("Area Code は半角英数字で入力してください。({0})", this.area_cd), new[] { "area_cd" });
            }

        }
    }
}
