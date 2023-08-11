using DVT.Domain.Enums;
using DVT.Domain.Models;

namespace DVT.BL.Interfaces
{
    public interface IElevatorRequest
    {
        Direction Direction { get; }
        List<int> Floor { get; }
        int NumberOfPeople { get; set; }
        List<Person> Requesters { get; set; }
    }
}