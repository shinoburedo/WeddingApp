using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauloaDemo.Models.Combined {

    public class VendorOrderSheetInfo {
        public Church Church { get; set; }
        //public WedInfo WedInfo { get; set; }
        public Customer Customer { get; set; }
        public Sales Sales { get; set; }
        public Item Item { get; set; }
        public Vendor Vendor { get; set; }
        public Arrangement Arrangement { get; set; }
    }
}
