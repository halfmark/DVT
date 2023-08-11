using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVT.Domain.Enums
{
    public enum Status
    {
        Moving = 0,
        Waiting =1,
        NotWorking=2,
        Loading=4,
        Unloading=5
    }
}
