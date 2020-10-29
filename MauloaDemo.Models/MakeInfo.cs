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

    [Table("make_info")]
    public class MakeInfo : IValidatableObject {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int info_id { get; set; }

        public int op_seq { get; set; }

        [Required(), StringLength(7), Hankaku()]
        public string c_num { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime? make_date { get; set; }

        [DataType(DataType.Time), DisplayFormat(DataFormatString = "HH:mm")]
        public DateTime? make_time { get; set; }

        [StringLength(50)]
        public string make_place { get; set; }

        public DateTime? make_in_time { get; set; }

        public string note { get; set; }

        [IgnoreChangeDiff]
        public string create_by { get; set; }

        [IgnoreChangeDiff, 
         DataType(DataType.DateTime),
         DisplayFormat(DataFormatString = "MM/dd/yyyy HH:mm:ss"),
         DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime create_date { get; set; }

        [IgnoreChangeDiff, Required(), Hankaku(), LowerCase()]
        public string last_person { get; set; }

        [IgnoreChangeDiff, 
         DataType(DataType.DateTime),
         DisplayFormat(DataFormatString = "MM/dd/yyyy HH:mm:ss"),
         DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime update_date { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.c_num) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.c_num, false)) {
                yield return new ValidationResult(string.Format("c_num は半角英数字で入力してください。({0})", this.c_num), new[] { "c_num" });
            }
        }
    }
}
