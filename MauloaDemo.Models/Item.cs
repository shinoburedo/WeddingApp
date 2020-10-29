using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;
using CBAF.Attributes;

namespace MauloaDemo.Models {

    [Table("item")]
    public class Item : IValidatableObject {

        [Key(), Required(), StringLength(15), Hankaku(), UpperCase()]
        public string item_cd { get; set; }

        [Hankaku(), StringLength(100)]
        public string item_name { get; set; }

        public string item_name_jpn { get; set; }

        [Required(), StringLength(3), Hankaku(), UpperCase()]
        public string item_type { get; set; }

        public DateTime? discon_date { get; set; }

        public bool auto_ok { get; set; }

        public bool special { get; set; }

        public bool selective { get; set; }

        [StringLength(5), Hankaku(), UpperCase()]
        public string church_cd { get; set; }

        public short? unit { get; set; }

        [StringLength(5), Hankaku(), UpperCase()]
        public string abbrev { get; set; }

        public string note { get; set; }

        public bool open_to_japan { get; set; }

        public bool rq_default { get; set; }

        [Required(), StringLength(15), Hankaku(), LowerCase()]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }


        [ForeignKey("item_type")]
        public ItemType ItemType { get; set; }


        //コンストラクタ
        public Item() {
            this.open_to_japan = true;
        }

        [NotMapped]
        public bool is_new { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.item_cd) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.item_cd, false)) {
                yield return new ValidationResult(string.Format("item_cd は半角英数字で入力してください。({0})", this.item_cd), new[] { "item_cd" });
            }

            if (!string.IsNullOrEmpty(this.item_type) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.item_type, false)) {
                yield return new ValidationResult(string.Format("item_type は半角英数字で入力してください。({0})", this.item_type), new[] { "item_type" });
            }
        }
    }
}
