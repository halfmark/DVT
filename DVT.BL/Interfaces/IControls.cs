using DVT.BL.Models;
using DVT.Domain.Enums;
using DVT.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVT.BL.Interfaces
{
    public interface IControls
    {
        void Initialize(List<Floor> floors, List<IElevatorService> elevatorServices);
        void RequestElevator(ElevatorRequest request);
        void ProcessElevatorRequests();
        Direction GetDirectionsByFloor(int floor);
        void AddQueue(ElevatorRequest request);

    }
}
