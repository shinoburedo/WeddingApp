using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CBAF;

namespace MauloaDemo.Models {

    [Table("mgm_report_combolist")]
    public class MgmReportComboList {


        public class ValueTextPair {
            public string value { get; set; }
            public string text { get; set; }

            public ValueTextPair() { 
            }
            public ValueTextPair(string _value, string _text) {
                this.value = _value;
                this.text = _text;
            }
        }


        [Key(), Required(), StringLength(20)]
        public string list_cd { get; set; }

        [StringLength(200)]
        public string description { get; set; }

        [Required()]
        public string query { get; set; }

        [Required(), StringLength(15)]
        public string last_person { get; set; }

        [CBAF.Attributes.IgnoreChangeDiff]
        public DateTime update_date { get; set; }



        [NotMapped]
        public bool is_new { get; set; }


        public static List<ValueTextPair> ParseString(string list_cd_or_query) {
            var list = new List<ValueTextPair>();
            if (list_cd_or_query.StartsWith("[")) {
                var str_list_cd = list_cd_or_query.Replace("[", "").Replace("]", "");
                var arr = str_list_cd.Split("|".ToCharArray());
                arr.ToList().ForEach(s => {
                    list.Add(new ValueTextPair(s, s));
                });
            }
            return list;
        }

    }
}
