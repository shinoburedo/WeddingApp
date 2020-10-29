using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using CBAF;

namespace MauloaDemo.Models.Combined {

    public class SchedulePatternItemInfo {

        public int row_id { get; set; }
        public int sch_pattern_id { get; set; }
        public string item_cd { get; set; }
        public string last_person { get; set; }
        public DateTime update_date { get; set; }
        public string item_type { get; set; }
        public string item_name { get; set; }
    }

    public class SchedulePatternNoteInfo {

        public int row_id { get; set; }
        public int sch_pattern_id { get; set; }
        public string template_cd { get; set; }
        public int disp_seq { get; set; }
        public string last_person { get; set; }
        public DateTime update_date { get; set; }
        public string note_jpn { get; set; }
    }

    public class SchedulePatternInfo {

        public int sch_pattern_id { get; set; }
        public string description { get; set; }
        public string last_person { get; set; }
        public DateTime update_date { get; set; }

        public virtual List<SchedulePatternLine> Lines { get; set; }
        public virtual List<SchedulePatternItem> Items { get; set; }
        public virtual List<SchedulePatternNote> Notes { get; set; }
    }

}
