using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectM.Models {

    [Table("cal_type")]
    public class CalType {

        [Key()]
        public string cal_type { get; set; }

        public string desc_eng { get; set; }
        public string desc_jpn { get; set; }
        public Nullable<short> disp_seq { get; set; }
        public string last_person { get; set; }
        public System.DateTime update_date { get; set; }
    }
}
