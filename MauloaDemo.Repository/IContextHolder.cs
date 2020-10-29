using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MauloaDemo.Repository {

    public interface IContextHolder {
        string RegionCd { get; }
        ProjMEntities Context { get; }
    }

}


