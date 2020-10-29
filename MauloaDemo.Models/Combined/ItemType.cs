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

    public class ItemTypeC {

        public string item_type { get; set; }

        [NotMapped]
        public bool is_pkg
        {
            get
            {
                if (this.item_type == "PHP" || this.item_type == "PHS" || this.item_type == "PKG")
                {
                    return true;
                }
                return false;
            }
        }

        public string description { get; set; }

    }

}
