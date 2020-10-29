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

    public class ItemGrouping {

            public string item_cd { get; set; }
            public string child_cd { get; set; }
            //public string last_person { get; set; }
            //public DateTime update_date { get; set; }
            public string item_name { get; set; }
            public string item_name_jpn { get; set; }
            public string item_type { get; set; }

    }

    public class ItemGroupingListParam {

        public string item_cd { get; set; }
        public List<ItemOption> list { get; set; }
        public bool is_new{ get; set; }

    }
}
