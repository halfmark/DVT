using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVT.Domain.Enums
{
    public enum Options
    {
        [Description("Start")]
        Start = 1,
        [Description("AddElevator")]
        AddElevator = 2,
        [Description("Quit")]
        Quit = 3
    }
}
