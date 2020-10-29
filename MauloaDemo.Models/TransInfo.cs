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

    [Table("trans_info")]
    public class TransInfo : IValidatableObject {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int info_id { get; set; }

        public int op_seq { get; set; }

        [Required(), StringLength(7), Hankaku()]
        public string c_num { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime? pickup_date { get; set; }

        [DataType(DataType.Time), DisplayFormat(DataFormatString = "HH:mm")]
        public DateTime? pickup_time { get; set; }

        [StringLength(3), Hankaku, UpperCase]
        public string pickup_hotel { get; set; }

        [NotMapped]
        public string pickup_hotel_name { get; set; }

        [StringLength(50)]
        public string pickup_place { get; set; }

        [DataType(DataType.Time), DisplayFormat(DataFormatString = "HH:mm")]
        public DateTime? dropoff_time { get; set; }

        [StringLength(3), Hankaku(), UpperCase()]
        public string dropoff_hotel { get; set; }

        [NotMapped]
        public string dropoff_hotel_name { get; set; }

        [StringLength(50)]
        public string dropoff_place { get; set; }

        public string note { get; set; }

        [IgnoreChangeDiff, Required(), StringLength(15), Hankaku()]
        public string create_by { get; set; }

        [IgnoreChangeDiff, 
         DataType(DataType.DateTime),
         DisplayFormat(DataFormatString = "MM/dd/yyyy HH:mm:ss"),
         DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime create_date { get; set; }

        [IgnoreChangeDiff, Required(), StringLength(15), Hankaku(), LowerCase()]
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
