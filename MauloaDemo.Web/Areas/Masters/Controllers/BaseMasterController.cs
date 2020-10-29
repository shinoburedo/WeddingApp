using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MauloaDemo.Web.Controllers;

namespace MauloaDemo.Web.Areas.Masters.Controllers
{
    [StaffOnlyFilter]
    [AccessLevelFilter(4)]
    public class BaseMasterController : BaseController
    {
    }
}