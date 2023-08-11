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
    public interface IElevatorService
    {
        Direction GetDirection();
        Status GetStatus();
        int GetCurrentLevel();
        void MoveDown();
        void MoveUp();
        List<Person> Load();
        List<Person> Unload();
        void FindLowestDestinationFloor();
        void FindHighestDestinationFloor();
        bool HasCapacity(int people);
        int GetCapacity();
        void MoveElevator(ElevatorRequest request);
        void AddNumberofPeople(List<Person> people);
        Guid GetElevatorId();

    }
}
