using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauloaDemo.Models.Combined {

    public class GridResult<T>  
        where T: class {

        public IEnumerable<T> data { get; set; }
        public int total;
    }
}
