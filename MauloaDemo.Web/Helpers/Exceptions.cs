using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MauloaDemo.Web {

    public class AccessLevelErrorException : Exception {
        public int RequiredAccess_evel { get; set; }
        public int ActualAccessLevel { get; set; }

        public AccessLevelErrorException(int required_access_level, int actual_access_level) {
            this.RequiredAccess_evel = required_access_level;
            this.ActualAccessLevel = actual_access_level;
        }
    }

    public class CustomerNotFoundException : Exception {
        public string c_num { get; set; }

        public CustomerNotFoundException(string c_num) {
            this.c_num = c_num;
        }
    }
}