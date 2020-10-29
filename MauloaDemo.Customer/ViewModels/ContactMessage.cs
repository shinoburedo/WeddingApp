using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MauloaDemo.Customer.ViewModels {

    public class ContactMessage {

        [StringLength(30)]
        public string Name { get; set; }

        [StringLength(100)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [StringLength(3)]
        public string area_cd { get; set; }

        [StringLength(80)]
        public string Subject { get; set; }

        [StringLength(4000)]
        public string Message { get; set; }
    }
}