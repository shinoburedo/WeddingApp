using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauloaDemo.Web.ViewModels {

    public class UserProfile {
        [Key()]
        public string login_id { get; set; }

        public string culture_name { get; set; }
        public string date_format { get; set; }
        //public string print_date_format { get; set; }
        public string time_format { get; set; }

        public string e_last_name { get; set; }
        public string e_first_name { get; set; }
        public string j_last_name { get; set; }
        public string j_first_name { get; set; }
        public string section { get; set; }
        public string sub_agent_cd { get; set; }
        public string e_mail { get; set; }

        public string cur_password { get; set; }
        public string new_password { get; set; }
        public string new_password_conf { get; set; }
        public DateTime? eff_to_pass { get; set; }

        public UserProfile() {
            this.login_id = string.Empty;
            this.culture_name = string.Empty;
            this.date_format = string.Empty;
            //this.print_date_format = string.Empty;
            this.time_format = string.Empty;

            this.e_last_name = string.Empty;
            this.e_first_name = string.Empty;
            this.section = string.Empty;

            this.cur_password = string.Empty;  //"*********************";
            this.new_password = string.Empty;
            this.new_password_conf = string.Empty;
        }
    }

}