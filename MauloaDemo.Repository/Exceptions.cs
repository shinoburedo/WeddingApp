using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MauloaDemo.Models;

namespace MauloaDemo.Repository.Exceptions {

    public class OptimisticLockException: Exception {

        private const string MSG = "The same data ({0}) has been updated by {1}.\n\n" + 
                                   "Please refresh the screen and try again.";

        //public OptimisticLockException()
        //    : base() {
        //}

        public OptimisticLockException(string message, string last_person) : 
            base(string.Format(MSG, 
                               message, 
                               string.IsNullOrEmpty(last_person) ? "someone" 
                                                                 : string.Format("'{0}'", last_person))) { 
        }

    }

    public class CustNameDuplicateException: Exception {

        public IEnumerable<Customer> dupList;

        public CustNameDuplicateException(IEnumerable<Customer> dup_list) {
            this.dupList = dup_list;
        }

    }

}
