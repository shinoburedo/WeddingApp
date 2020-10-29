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

    [Table("shoot_info")]
    public class ShootInfo : IValidatableObject {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int info_id { get; set; }

        public int op_seq { get; set; }

        [Required(), StringLength(7), Hankaku()]
        public string c_num { get; set; }

        [DataType(DataType.Date), DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime? shoot_date { get; set; }

        [DataType(DataType.Time), DisplayFormat(DataFormatString = "HH:mm")]
        public DateTime? shoot_time { get; set; }

        [StringLength(50)]
        public string shoot_place { get; set; }

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


        //public virtual Sales Sales { get; set; }

        ////From sales, church and item tables.
        //public string item_cd { get; set; }
        //public string item_name { get; set; }
        //public string item_name_jpn { get; set; }
        //public string agent_cd { get; set; }
        //public string sub_agent_cd { get; set; }
        //public bool cust_collect { get; set; }
        //public short quantity { get; set; }
        //public decimal price { get; set; }
        //public decimal amount { get; set; }
        //public string book_status { get; set; }
        //public string staff { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            if (!string.IsNullOrEmpty(this.c_num) && !HankakuZenkaku.isHankakuAlphaNumericOrSymbol(this.c_num, false)) {
                yield return new ValidationResult(string.Format("c_num は半角英数字で入力してください。({0})", this.c_num), new[] { "c_num" });
            }
        }
    }
}
