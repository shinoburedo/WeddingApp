using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MauloaDemo.Models {

    [Serializable]
    [Table("login_user_token")]
    public class LoginUserToken {

        [Key(), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string login_id {get; set;}
        public string token {get; set;}
        public DateTime create_date {get; set;}
	    public DateTime expires {get; set; }
    }
}
