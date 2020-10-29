using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace MauloaDemo.Web.Hubs {
    public class HomeHub : Hub {
        public void Hello() {
            Clients.All.hello();
        }
    }
}