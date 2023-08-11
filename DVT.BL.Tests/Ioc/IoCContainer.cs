using DVT.BL.Implemantation;
using DVT.BL.Interfaces;
using Lamar;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVT.BL.Tests.Ioc
{
    public static class IoCContainer
    {
        public static IContainer Init()
        {

            return new Container(x =>
            {
                x.AddTransient<IElevatorService, ElevatorService>();
                x.AddTransient<IControls, ControlsService>();
            });
        }
    }
}
