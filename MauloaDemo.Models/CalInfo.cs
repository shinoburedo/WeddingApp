using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectM.Models {

    [Table("cal_info")]
    public class CalInfo {

        [Key()]
        public int cal_id { get; set; }

        public string cal_type { get; set; }
        public System.DateTime cal_datetime { get; set; }
        public string cal_desc { get; set; }
        public string last_person { get; set; }
        public System.DateTime update_date { get; set; }
    }
}
