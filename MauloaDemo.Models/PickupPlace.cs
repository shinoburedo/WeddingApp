using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("pickup_place")]
    public class PickupPlace : IValidatableObject {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int place_id { get; set; }

        [Required(), StringLength(50)]
        public string place_name { get; set; }

        [StringLength(50)]
        public string place_name_eng { get; set; }

        public short place_order { get; set; }

        [StringLength(3), Hankaku(), UpperCase()]
        public string hotel_cd { get; set; }

        public string create_by { get; set; }
        public DateTime create_date { get; set; }

        [Required(), StringLength(15), Hankaku()]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }

        [NotMapped]
        public bool is_new { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.hotel_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.hotel_cd, false)) {
                yield return new ValidationResult(string.Format("hotel_cd は半角英数字で入力してください。({0})", this.hotel_cd), new[] { "hotel_cd" });
            }
        }
    }
}
