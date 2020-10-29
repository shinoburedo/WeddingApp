using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MauloaDemo.Web.ViewModels {

    public class AccountLogin {

        [Required]
        [StringLength(12)]
        public string login_id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(20)]
        public string password { get; set; }
    }
}