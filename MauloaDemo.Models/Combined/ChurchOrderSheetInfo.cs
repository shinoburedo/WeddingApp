using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MauloaDemo.Models.Combined {

    public class ChurchOrderSheetInfo {
        public Church Church { get; set; }
        public WedInfo WedInfo { get; set; }
        public Customer Customer { get; set; }
    }
}
