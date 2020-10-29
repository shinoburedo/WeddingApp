using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauloaDemo.Models.Combined {

    public class ItemWithPrice {

        public string info_type { get; set; }
        public string item_type { get; set; }
        public string church_cd { get; set; }
        public string item_cd { get; set; }
        public string item_name { get; set; }
        public string item_name_jpn { get; set; }
        public bool rq_default { get; set; }
        public string abbrev { get; set; }
        public decimal gross { get; set; }
        public decimal d_net { get; set; }
        public int y_net { get; set; }
        public bool tentative { get; set; }
    }
}
