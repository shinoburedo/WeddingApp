using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("item_type")]
    public class ItemType : IValidatableObject {

        public ItemType() {
            this.update_date = DateTime.UtcNow;
        }

        [Key(), Required(), StringLength(3), Hankaku(), UpperCase()]
        public string item_type { get; set; }

        [Hankaku(), StringLength(100)]
        public string desc_eng { get; set; }

        [StringLength(100)]
        public string desc_jpn { get; set; }

        [StringLength(3), Hankaku(), UpperCase()]
        public string info_type { get; set; }

        [Required(), StringLength(15), Hankaku()]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        [NotMapped]
        public bool is_new { get; set; }



        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.item_type) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.item_type, false)) {
                yield return new ValidationResult(string.Format("item_type は半角英数字で入力してください。({0})", this.item_type), new[] { "item_type" });
            }

            if (!string.IsNullOrEmpty(this.info_type) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.info_type, false)) {
                yield return new ValidationResult(string.Format("info_type は半角英数字で入力してください。({0})", this.info_type), new[] { "info_type" });
            }
        }
    }
}
